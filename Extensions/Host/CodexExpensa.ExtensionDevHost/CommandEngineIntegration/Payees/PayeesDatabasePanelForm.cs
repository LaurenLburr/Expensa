using Microsoft.Data.Sqlite;
using System.Data;
using System.Diagnostics;
using System.Globalization;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Payees;

public sealed partial class PayeesDatabasePanelForm : DatabasePanelTemplate
{
    private string currentDatabasePath = string.Empty;

    public PayeesDatabasePanelForm()
    {
        currentDatabasePath = GetDefaultExpensaDatabasePath();

        ConfigureDatabasePanel(
            "PayeesAddin",
            "Payees Database",
            Path.GetFileName(currentDatabasePath),
            currentDatabasePath);

        LoadPayeeSummary();
    }

    protected override void OnDatabasePathLinkClicked()
    {
        string folder =
            Path.GetDirectoryName(currentDatabasePath) ?? string.Empty;

        if (string.IsNullOrWhiteSpace(folder) ||
            !Directory.Exists(folder))
        {
            MessageBox.Show(
                this,
                $"Database folder was not found:{Environment.NewLine}{folder}",
                "Payees Database",
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
        LoadPayeeSummary();
    }

    protected override void OnUpdateFromDevClicked()
    {
        LoadPayeeSummary();
    }

    private void LoadPayeeSummary()
    {
        SetSummaryText(string.Empty);
        SetGridDataSource(null);
        SetRowStatus("Loading Payees...");

        try
        {
            if (string.IsNullOrWhiteSpace(currentDatabasePath))
            {
                ShowMessage("No database path is selected.");
                return;
            }

            if (!File.Exists(currentDatabasePath))
            {
                ShowMessage($"Database file was not found:{Environment.NewLine}{currentDatabasePath}");
                return;
            }

            using SqliteConnection connection =
                new($"Data Source={currentDatabasePath};Mode=ReadOnly");

            connection.Open();

            string? payeeTableName =
                FindPayeeTableName(connection);

            if (string.IsNullOrWhiteSpace(payeeTableName))
            {
                ShowMessage(
                    "No Payee table was found." + Environment.NewLine + Environment.NewLine +
                    "Expected one of:" + Environment.NewLine +
                    "Payee" + Environment.NewLine +
                    "Payees");
                return;
            }

            DataTable payees =
                LoadPayees(connection, payeeTableName);

            SetGridDataSource(payees);

            SetSummaryText(
                $"Payees database page{Environment.NewLine}{Environment.NewLine}" +
                $"Database:{Environment.NewLine}{currentDatabasePath}{Environment.NewLine}{Environment.NewLine}" +
                $"Payee table: {payeeTableName}{Environment.NewLine}" +
                $"Rows shown: {payees.Rows.Count}{Environment.NewLine}");

            SetRowStatus($"Loaded {payees.Rows.Count} payee row(s).");
        }
        catch (Exception exception)
        {
            SetSummaryText(exception.ToString());
            SetRowStatus("Failed to load Payees.");
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

    private static string? FindPayeeTableName(SqliteConnection connection)
    {
        using SqliteCommand command =
            connection.CreateCommand();

        command.CommandText =
            """
            SELECT [name]
            FROM [sqlite_master]
            WHERE [type] = 'table'
              AND [name] IN ('Payee', 'Payees')
            ORDER BY
                CASE [name]
                    WHEN 'Payee' THEN 1
                    WHEN 'Payees' THEN 2
                    ELSE 3
                END
            LIMIT 1;
            """;

        object? value =
            command.ExecuteScalar();

        return Convert.ToString(value, CultureInfo.InvariantCulture);
    }

    private static DataTable LoadPayees(SqliteConnection connection, string tableName)
    {
        HashSet<string> columns =
            GetColumns(connection, tableName);

        string idColumn =
            PickColumn(columns, "PayeeId", "Id", tableName + "Id");

        string nameColumn =
            PickColumn(columns, "PayeeName", "Name", "DisplayName");

        string activeExpression =
            columns.Contains("IsActive")
                ? "[IsActive]"
                : "1";

        using SqliteCommand command =
            connection.CreateCommand();

        command.CommandText =
            $"""
            SELECT
                [{idColumn}] AS [PayeeId],
                [{nameColumn}] AS [PayeeName],
                {activeExpression} AS [IsActive]
            FROM [{tableName}]
            ORDER BY [{nameColumn}]
            LIMIT 500;
            """;

        using SqliteDataReader reader =
            command.ExecuteReader();

        DataTable table =
            new();

        table.Load(reader);

        return table;
    }

    private static HashSet<string> GetColumns(SqliteConnection connection, string tableName)
    {
        using SqliteCommand command =
            connection.CreateCommand();

        command.CommandText =
            "SELECT [name] FROM pragma_table_info(@TableName);";

        command.Parameters.AddWithValue("@TableName", tableName);

        HashSet<string> columns =
            new(StringComparer.OrdinalIgnoreCase);

        using SqliteDataReader reader =
            command.ExecuteReader();

        while (reader.Read())
        {
            columns.Add(reader.GetString(0));
        }

        return columns;
    }

    private static string PickColumn(HashSet<string> columns, params string[] candidates)
    {
        foreach (string candidate in candidates)
        {
            if (columns.Contains(candidate))
            {
                return candidate;
            }
        }

        throw new InvalidOperationException(
            $"Could not find a matching column. Candidates: {string.Join(", ", candidates)}");
    }
}
