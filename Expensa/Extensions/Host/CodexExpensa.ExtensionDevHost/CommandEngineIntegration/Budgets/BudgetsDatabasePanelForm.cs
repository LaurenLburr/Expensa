using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Diagnostics;
using System.Globalization;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed partial class BudgetsDatabasePanelForm : DatabasePanelTemplate
{
    private const string AddinId = "BudgetsAddin";

    private readonly AddinRuntimeDatabasePathService _pathService = new();
    private readonly AddinRuntimeDatabaseCopyService _copyService = new();

    private string _currentDatabasePath = string.Empty;
    private string _lastCopyMessage = string.Empty;
    private AddinMemoryDatabaseSession? _databaseSession;

    public BudgetsDatabasePanelForm()
    {
        _currentDatabasePath =
            _pathService.GetRuntimeDatabaseLocation(AddinId).DatabasePath;

        ConfigureCurrentDatabasePanel();
        LoadDataSummary();
    }

    protected override void OnDatabasePathLinkClicked()
    {
        string folder =
            Path.GetDirectoryName(_currentDatabasePath) ?? string.Empty;

        if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
        {
            MessageBox.Show(
                this,
                $"Database folder was not found:{Environment.NewLine}{folder}",
                "Budgets Database",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return;
        }

        Process.Start(
            new ProcessStartInfo
            {
                FileName = folder,
                UseShellExecute = true
            });
    }

    protected override void OnUpdateFromProdClicked()
    {
        ReplaceRuntimeDatabaseFromSource(
            "Prod",
            () => _copyService.ReplaceRuntimeDatabaseFromProd(AddinId));
    }

    protected override void OnUpdateFromDevClicked()
    {
        ReplaceRuntimeDatabaseFromSource(
            "Dev",
            () => _copyService.ReplaceRuntimeDatabaseFromDev(AddinId));
    }

    private void ReplaceRuntimeDatabaseFromSource(
        string sourceLabel,
        Func<AddinRuntimeDatabaseCopyResult> copyAction)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceLabel);
        ArgumentNullException.ThrowIfNull(copyAction);

        try
        {
            ReleaseMemoryDatabase();

            AddinRuntimeDatabaseCopyResult result =
                copyAction();

            _currentDatabasePath =
                result.RuntimeDatabasePath;

            _lastCopyMessage =
                $"Updated add-in runtime database from {sourceLabel}.{Environment.NewLine}" +
                $"Source:{Environment.NewLine}{result.SourceDatabasePath}{Environment.NewLine}{Environment.NewLine}" +
                $"Add-in database:{Environment.NewLine}{result.RuntimeDatabasePath}{Environment.NewLine}{Environment.NewLine}" +
                $"Active runtime pointer:{Environment.NewLine}{result.ActiveRuntimePathFile}";

            ConfigureCurrentDatabasePanel();
            LoadDataSummary();
        }
        catch (Exception exception)
        {
            SetSummaryText(exception.ToString());
            SetRowStatus($"Failed to update Budgets database from {sourceLabel}.");

            MessageBox.Show(
                this,
                exception.Message,
                "Budgets Database",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void ConfigureCurrentDatabasePanel()
    {
        ConfigureDatabasePanel(
            AddinId,
            "Budgets Database",
            Path.GetFileName(_currentDatabasePath),
            _currentDatabasePath);
    }

    private void LoadDataSummary()
    {
        SetSummaryText(string.Empty);
        SetGridDataSource(null);
        SetRowStatus("Loading Budgets...");

        try
        {
            if (!File.Exists(_currentDatabasePath))
            {
                ShowMessage(
                    $"Add-in database file was not found:{Environment.NewLine}{_currentDatabasePath}{Environment.NewLine}{Environment.NewLine}" +
                    "Use Update Data from Prod or Update Data from Dev to create the add-in runtime copy.");
                return;
            }

            ReleaseMemoryDatabase();

            _databaseSession =
                AddinMemoryDatabaseSession.LoadFromFile(_currentDatabasePath);

            SqliteConnection connection =
                _databaseSession.Connection;

            string? tableName =
                FindTableName(connection);

            if (string.IsNullOrWhiteSpace(tableName))
            {
                ShowMessage(
                    "No Budgets table was found." + Environment.NewLine + Environment.NewLine +
                    "Expected one of:" + Environment.NewLine +
                    "Website" + Environment.NewLine +
                    "Websites");
                return;
            }

            DataTable data =
                LoadRows(connection, tableName);

            SetGridDataSource(data);

            SetSummaryText(
                "Budgets database page" + Environment.NewLine + Environment.NewLine +
                "Database is loaded into memory from the add-in runtime copy." + Environment.NewLine +
                "The source database file is closed after the memory load completes." + Environment.NewLine + Environment.NewLine +
                $"Add-in database source file:{Environment.NewLine}{_currentDatabasePath}{Environment.NewLine}{Environment.NewLine}" +
                FormatLastCopyMessage() +
                $"Table: {tableName}{Environment.NewLine}" +
                $"Rows shown: {data.Rows.Count}{Environment.NewLine}");

            SetRowStatus($"Loaded {data.Rows.Count} row(s).");
        }
        catch (Exception exception)
        {
            SetSummaryText(exception.ToString());
            SetRowStatus("Failed to load Budgets.");
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        ReleaseMemoryDatabase();
        base.OnFormClosed(e);
    }

    private void ReleaseMemoryDatabase()
    {
        _databaseSession?.Dispose();
        _databaseSession = null;
    }

    private string FormatLastCopyMessage()
    {
        if (string.IsNullOrWhiteSpace(_lastCopyMessage))
        {
            return string.Empty;
        }

        return _lastCopyMessage + Environment.NewLine + Environment.NewLine;
    }

    private void ShowMessage(string message)
    {
        SetSummaryText(message);
        SetRowStatus(message.Split(Environment.NewLine, StringSplitOptions.None).FirstOrDefault() ?? message);
    }

    private static string? FindTableName(SqliteConnection connection)
    {
        using SqliteCommand command =
            connection.CreateCommand();

        command.CommandText =
            """
            SELECT [name]
            FROM [sqlite_master]
            WHERE [type] = 'table'
              AND [name] IN ('Budget', 'Budgets', 'BudgetMonth', 'BudgetMonths')
            ORDER BY
                CASE [name]
                    WHEN 'Budget' THEN 1
                    WHEN 'Budgets' THEN 2
                    WHEN 'BudgetMonth' THEN 3
                    WHEN 'BudgetMonths' THEN 4
                    ELSE 100
                END
            LIMIT 1;
            """;

        return Convert.ToString(command.ExecuteScalar(), CultureInfo.InvariantCulture);
    }

    private static DataTable LoadRows(SqliteConnection connection, string tableName)
    {
        using SqliteCommand command =
            connection.CreateCommand();

        command.CommandText =
            $"""
            SELECT *
            FROM [{tableName}]
            LIMIT 500;
            """;

        return LoadDataTable(command);
    }

    private static DataTable LoadDataTable(SqliteCommand command)
    {
        using SqliteDataReader reader =
            command.ExecuteReader();

        DataTable table = new();
        table.Load(reader);
        return table;
    }

}
