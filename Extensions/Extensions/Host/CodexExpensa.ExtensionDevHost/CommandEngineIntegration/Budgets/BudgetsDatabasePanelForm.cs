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
    private readonly BudgetMonthTemplateSeedService _templateSeedService = new();

    private string _currentDatabasePath = string.Empty;
    private string _lastCopyMessage = string.Empty;
    private AddinMemoryDatabaseSession? _databaseSession;

    public BudgetsDatabasePanelForm()
    {
        _currentDatabasePath =
            _pathService.GetRuntimeDatabaseLocation(AddinId).DatabasePath;

        SafeDataGridViewBinding.Attach(gridDataView);
        ConfigureSourceFolderContextMenus();
        ConfigureCurrentDatabasePanel();
        LoadDataSummary();
    }

    private void ConfigureSourceFolderContextMenus()
    {
        linkUpdate_from_Prod.ContextMenuStrip =
            CreateSourceFolderContextMenu(
                "Open Prod folder location",
                "Prod",
                _pathService.GetProdDatabasePath,
                OnUpdateFromProdClicked);

        linkUpdate_from_Dev.ContextMenuStrip =
            CreateSourceFolderContextMenu(
                "Open Dev folder location",
                "Dev",
                () => _pathService.GetDevCurrentDatabasePath(AddinId),
                OnUpdateFromDevClicked);
    }

    private ContextMenuStrip CreateSourceFolderContextMenu(
        string menuText,
        string sourceLabel,
        Func<string> databasePathFactory,
        Action reloadAction)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(menuText);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceLabel);
        ArgumentNullException.ThrowIfNull(databasePathFactory);
        ArgumentNullException.ThrowIfNull(reloadAction);

        ContextMenuStrip menu = new();
        ToolStripMenuItem reloadItem = new("Reload");
        ToolStripMenuItem openFolderItem = new(menuText);

        reloadItem.Click +=
            (_, _) => reloadAction();

        openFolderItem.Click +=
            (_, _) => OpenSourceDatabaseFolder(sourceLabel, databasePathFactory());

        menu.Items.Add(reloadItem);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(openFolderItem);
        return menu;
    }

    private void OpenSourceDatabaseFolder(
        string sourceLabel,
        string databasePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceLabel);
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);

        string folder =
            Path.GetDirectoryName(databasePath) ?? string.Empty;

        if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
        {
            MessageBox.Show(
                this,
                $"{sourceLabel} database folder was not found:{Environment.NewLine}{folder}",
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

            _databaseSession =
                AddinMemoryDatabaseSession.LoadFromFile(_currentDatabasePath);

            BudgetMonthTemplateSeedResult seedResult =
                _templateSeedService.GenerateBudgetMonths(
                    _databaseSession.Connection,
                    [2025, 2026]);

            if (seedResult.ChangedDatabase)
            {
                _databaseSession.SaveToFile(_currentDatabasePath);
            }

            _lastCopyMessage =
                $"Updated add-in runtime database from {sourceLabel}.{Environment.NewLine}" +
                $"Source:{Environment.NewLine}{result.SourceDatabasePath}{Environment.NewLine}{Environment.NewLine}" +
                $"Add-in database:{Environment.NewLine}{result.RuntimeDatabasePath}{Environment.NewLine}{Environment.NewLine}" +
                $"Active runtime pointer:{Environment.NewLine}{result.ActiveRuntimePathFile}{Environment.NewLine}{Environment.NewLine}" +
                FormatTemplateSeedMessage(seedResult);

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
                    "BudgetMonth table was not found." + Environment.NewLine + Environment.NewLine +
                    "Expected table:" + Environment.NewLine +
                    "BudgetMonth");
                return;
            }

            DataTable data =
                LoadRows(connection, tableName);

            SetGridDataSource(SafeDataGridViewBinding.Sanitize(data));

            SetSummaryText(
                "Budgets database page" + Environment.NewLine + Environment.NewLine +
                "Database is loaded into memory from the add-in runtime copy." + Environment.NewLine +
                "The source database file is closed after the memory load completes." + Environment.NewLine + Environment.NewLine +
                $"Add-in database source file:{Environment.NewLine}{_currentDatabasePath}{Environment.NewLine}{Environment.NewLine}" +
                FormatLastCopyMessage() +
                $"Table: {tableName}{Environment.NewLine}" +
                "SQL query used:" + Environment.NewLine +
                BuildSummarySql(tableName) + Environment.NewLine + Environment.NewLine +
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

    private static string FormatTemplateSeedMessage(BudgetMonthTemplateSeedResult seedResult)
    {
        ArgumentNullException.ThrowIfNull(seedResult);

        string years =
            seedResult.Years.Count == 0
                ? "(none)"
                : string.Join(", ", seedResult.Years);

        string message =
            "Generated missing BudgetMonth/BudgetMonthRow rows from BudgetTemplateRow." + Environment.NewLine +
            $"Target years: {years}{Environment.NewLine}" +
            $"Active BudgetTemplateRow rows found: {seedResult.TemplateRowCount}{Environment.NewLine}" +
            $"Month/header/detail rows inserted: {seedResult.InsertedRowCount}{Environment.NewLine}" +
            $"Months skipped because BudgetMonthId already existed: {seedResult.SkippedMonthCount}";

        if (seedResult.Warnings.Count > 0)
        {
            message += Environment.NewLine +
                "Warnings:" + Environment.NewLine +
                string.Join(Environment.NewLine, seedResult.Warnings);
        }

        return message;
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
              AND [name] = 'BudgetMonth'
            LIMIT 1;
            """;

        return Convert.ToString(command.ExecuteScalar(), CultureInfo.InvariantCulture);
    }

    private static DataTable LoadRows(SqliteConnection connection, string tableName)
    {
        using SqliteCommand command =
            connection.CreateCommand();

        command.CommandText =
            BuildSummarySql(tableName);

        return LoadDataTable(command);
    }

    private static string BuildSummarySql(string tableName)
    {
        return
            $"""
            SELECT *
            FROM [{tableName}]
            ORDER BY [BudgetMonthId]
            LIMIT 500;
            """;
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
