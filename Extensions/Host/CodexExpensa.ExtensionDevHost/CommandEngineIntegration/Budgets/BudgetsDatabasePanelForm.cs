using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Globalization;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed partial class BudgetsDatabasePanelForm : DatabasePanelTemplate
{
    private const string AddinId = "BudgetsAddin";

    private readonly AddinRuntimeDatabasePathService _pathService = new();
    private readonly AddinRuntimeDatabaseCopyService _copyService = new();
    private readonly AddinDevDatabaseUiCoordinator _devDatabaseUi = new();
    private readonly BudgetMonthTemplateSeedService _templateSeedService = new();
    private readonly BudgetsTestDataSeedService _testDataSeedService = new();

    private string _currentDatabasePath = string.Empty;
    private string _lastCopyMessage = string.Empty;
    private AddinMemoryDatabaseSession? _databaseSession;

    public BudgetsDatabasePanelForm()
    {
        _currentDatabasePath =
            _pathService.GetRuntimeDatabaseLocation(AddinId).DatabasePath;

        ConfigureDatabaseContextMenus(
            () => ReloadAddinDatabaseIntoMemory(),
            () => OpenProdDatabaseFolder(),
            () => CreateOrReplaceDevDatabaseFromAddinDatabase(),
            () => OpenDevDatabaseFolder());

        ConfigureDatabaseActionLink(
            "Reset Budget Test Data",
            ResetBudgetTestData);

        SafeDataGridViewBinding.Attach(gridDataView);
        ConfigureCurrentDatabasePanel();
        LoadDataSummary();
    }

    protected override void ReloadAddinDatabaseIntoMemory()
    {
        _lastCopyMessage =
            "Reloaded the existing add-in runtime database file into memory." +
            Environment.NewLine +
            "No database was copied from Prod or Dev.";

        ConfigureCurrentDatabasePanel();
        LoadDataSummary();
    }

    protected override void OpenProdDatabaseFolder()
    {
        FolderLauncher.OpenContainingFolder(
            _pathService.GetProdDatabasePath());
    }

    protected override void CreateOrReplaceDevDatabaseFromAddinDatabase()
    {
        AddinDevDatabaseCopyResult? result =
            _devDatabaseUi.CreateOrReplaceDevDatabase(this, AddinId);

        if (result is null)
        {
            return;
        }

        _lastCopyMessage =
            $"Created Dev database from the add-in runtime database.{Environment.NewLine}" +
            $"Source:{Environment.NewLine}{result.RuntimeDatabasePath}{Environment.NewLine}{Environment.NewLine}" +
            $"Dev database:{Environment.NewLine}{result.DevDatabasePath}";

        ConfigureCurrentDatabasePanel();
        LoadDataSummary();
    }

    protected override void OpenDevDatabaseFolder()
    {
        FolderLauncher.OpenContainingFolder(
            _pathService.GetDevCurrentDatabasePath(AddinId));
    }

    protected override void OnDatabasePathLinkClicked()
    {
        FolderLauncher.OpenContainingFolder(_currentDatabasePath);
    }

    protected override void OnUpdateFromProdClicked()
    {
        ReplaceRuntimeDatabaseFromSource(
            "Prod",
            () => _copyService.ReplaceRuntimeDatabaseFromProd(AddinId));
    }

    protected override void OnUpdateFromDevClicked()
    {
        if (!_devDatabaseUi.EnsureDevDatabaseExists(this, AddinId))
        {
            return;
        }

        ReplaceRuntimeDatabaseFromSource(
            "Dev",
            () => _copyService.ReplaceRuntimeDatabaseFromDev(AddinId));
    }

    private void ResetBudgetTestData()
    {
        DialogResult confirmation =
            MessageBox.Show(
                this,
                "This will reset the controlled Budgets test records in the Dev database,"
                + Environment.NewLine
                + "copy the updated Dev database to the add-in runtime database,"
                + Environment.NewLine
                + "and reload the runtime database into memory."
                + Environment.NewLine
                + Environment.NewLine
                + "Prod is not modified."
                + Environment.NewLine
                + Environment.NewLine
                + "Continue?",
                "Reset Budget Test Data",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

        if (confirmation != DialogResult.Yes)
        {
            return;
        }

        try
        {
            if (!_devDatabaseUi.EnsureDevDatabaseExists(this, AddinId))
            {
                return;
            }

            ReleaseMemoryDatabase();

            string devDatabasePath =
                _pathService.GetDevCurrentDatabasePath(AddinId);

            string scriptPath =
                Path.Combine(
                    Path.GetDirectoryName(devDatabasePath)
                        ?? throw new InvalidOperationException(
                            "The Budgets Dev database folder could not be determined."),
                    "BudgetsTestData.sql");

            BudgetsTestDataSeedResult seedResult =
                _testDataSeedService.Reset(
                    devDatabasePath,
                    scriptPath);

            ReplaceRuntimeDatabaseFromSource(
                "Dev test data",
                () => _copyService.ReplaceRuntimeDatabaseFromDev(AddinId));

            _lastCopyMessage =
                "Reset deterministic Budgets test data."
                + Environment.NewLine
                + $"SQL file:{Environment.NewLine}{seedResult.ScriptPath}"
                + Environment.NewLine
                + Environment.NewLine
                + $"Dev database:{Environment.NewLine}{seedResult.DevDatabasePath}"
                + Environment.NewLine
                + Environment.NewLine
                + $"Budget months: {seedResult.BudgetMonthCount}"
                + Environment.NewLine
                + $"Budget rows: {seedResult.BudgetMonthRowCount}"
                + Environment.NewLine
                + $"Test transactions: {seedResult.TransactionCount}"
                + Environment.NewLine
                + $"Statuses found: {string.Join(", ", seedResult.Statuses)}";

            ConfigureCurrentDatabasePanel();
            LoadDataSummary();

            MessageBox.Show(
                this,
                "The Budgets test data was reset successfully."
                + Environment.NewLine
                + Environment.NewLine
                + "The Dev database was updated, copied to the runtime database,"
                + Environment.NewLine
                + "and reloaded into memory.",
                "Reset Budget Test Data",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception exception)
        {
            SetSummaryText(exception.ToString());
            SetRowStatus("Failed to reset Budget test data.");

            MessageBox.Show(
                this,
                exception.Message,
                "Reset Budget Test Data",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
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
