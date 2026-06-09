using Microsoft.Data.Sqlite;
using System.Data;
using System.Diagnostics;
using System.Globalization;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed partial class BudgetsDatabasePanelForm : DatabasePanelTemplate
{
    private string currentDatabasePath = string.Empty;

    public BudgetsDatabasePanelForm()
    {
        currentDatabasePath = GetDefaultExpensaDatabasePath();

        ConfigureDatabasePanel(
            "BudgetsAddin",
            "Budgets Database",
            Path.GetFileName(currentDatabasePath),
            currentDatabasePath);

        LoadDataSummary();
    }

    protected override void OnDatabasePathLinkClicked()
    {
        string folder = Path.GetDirectoryName(currentDatabasePath) ?? string.Empty;

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

        Process.Start(new ProcessStartInfo { FileName = folder, UseShellExecute = true });
    }

    protected override void OnUpdateFromProdClicked()
    {
        LoadDataSummary();
    }

    protected override void OnUpdateFromDevClicked()
    {
        LoadDataSummary();
    }

    private void LoadDataSummary()
    {
        SetSummaryText(string.Empty);
        SetGridDataSource(null);
        SetRowStatus("Loading Budgets...");

        try
        {
            if (!File.Exists(currentDatabasePath))
            {
                ShowMessage($"Database file was not found:{Environment.NewLine}{currentDatabasePath}");
                return;
            }

            using SqliteConnection connection = new($"Data Source={currentDatabasePath};Mode=ReadOnly");
            connection.Open();

            string? tableName = FindTableName(connection);

            if (string.IsNullOrWhiteSpace(tableName))
            {
                ShowMessage(
                    "No Budgets table was found." + Environment.NewLine + Environment.NewLine +
                    "Expected one of:" + Environment.NewLine +
                    "Budget" + Environment.NewLine +
                    "Budgets" + Environment.NewLine +
                    "BudgetMonth" + Environment.NewLine +
                    "BudgetMonths");
                return;
            }

            DataTable data = LoadRows(connection, tableName);

            SetGridDataSource(data);

            SetSummaryText(
                "Budgets database page" + Environment.NewLine + Environment.NewLine +
                $"Database:{Environment.NewLine}{currentDatabasePath}{Environment.NewLine}{Environment.NewLine}" +
                $"Table: {tableName}{Environment.NewLine}" +
                $"Rows shown: {data.Rows.Count}{Environment.NewLine}{Environment.NewLine}" +
                "The grid shows the raw table columns. Budget tables do not need to expose a display-name column.");

            SetRowStatus($"Loaded {data.Rows.Count} row(s).");
        }
        catch (Exception exception)
        {
            SetSummaryText(exception.ToString());
            SetRowStatus("Failed to load Budgets.");
        }
    }

    private void ShowMessage(string message)
    {
        SetSummaryText(message);
        SetRowStatus(message.Split(Environment.NewLine, StringSplitOptions.None).FirstOrDefault() ?? message);
    }

    private static string GetDefaultExpensaDatabasePath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CodexExpensa",
            "db",
            "codexexpensa.db");
    }

    private static string? FindTableName(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();

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
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            $"""
            SELECT *
            FROM [{tableName}]
            LIMIT 500;
            """;

        using SqliteDataReader reader = command.ExecuteReader();

        DataTable table = new();
        table.Load(reader);

        return table;
    }
}
