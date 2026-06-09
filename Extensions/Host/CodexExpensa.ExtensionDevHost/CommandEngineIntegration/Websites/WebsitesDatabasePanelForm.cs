using Microsoft.Data.Sqlite;
using System.Data;
using System.Diagnostics;
using System.Globalization;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed partial class WebsitesDatabasePanelForm : DatabasePanelTemplate
{
    private string currentDatabasePath = string.Empty;

    public WebsitesDatabasePanelForm()
    {
        currentDatabasePath = GetDefaultExpensaDatabasePath();

        ConfigureDatabasePanel(
            "WebsitesAddin",
            "Websites Database",
            Path.GetFileName(currentDatabasePath),
            currentDatabasePath);

        LoadDataSummary();
    }

    protected override void OnDatabasePathLinkClicked()
    {
        string folder = Path.GetDirectoryName(currentDatabasePath) ?? string.Empty;

        if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
        {
            MessageBox.Show(this, $"Database folder was not found:{Environment.NewLine}{folder}", "Websites Database", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        SetRowStatus("Loading Websites...");

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
                ShowMessage("No Websites table was found." + Environment.NewLine + Environment.NewLine + "Expected one of:" + Environment.NewLine + "Website" + Environment.NewLine + "Websites");
                return;
            }

            DataTable data = LoadRows(connection, tableName);

            SetGridDataSource(data);

            SetSummaryText(
                "Websites database page" + Environment.NewLine + Environment.NewLine +
                $"Database:{Environment.NewLine}{currentDatabasePath}{Environment.NewLine}{Environment.NewLine}" +
                $"Table: {tableName}{Environment.NewLine}" +
                $"Rows shown: {data.Rows.Count}{Environment.NewLine}");

            SetRowStatus($"Loaded {data.Rows.Count} row(s).");
        }
        catch (Exception exception)
        {
            SetSummaryText(exception.ToString());
            SetRowStatus("Failed to load Websites.");
        }
    }

    private void ShowMessage(string message)
    {
        SetSummaryText(message);
        SetRowStatus(message.Split(Environment.NewLine, StringSplitOptions.None).FirstOrDefault() ?? message);
    }

    private static string GetDefaultExpensaDatabasePath()
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CodexExpensa", "db", "codexexpensa.db");
    }

    private static string? FindTableName(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            SELECT [name]
            FROM [sqlite_master]
            WHERE [type] = 'table'
              AND [name] IN ('Website', 'Websites')
            ORDER BY
                CASE [name]
                    WHEN 'Website' THEN 1
                    WHEN 'Websites' THEN 2
                    ELSE 100
                END
            LIMIT 1;
            """;

        return Convert.ToString(command.ExecuteScalar(), CultureInfo.InvariantCulture);
    }

    private static DataTable LoadRows(SqliteConnection connection, string tableName)
    {
        HashSet<string> columns = GetColumns(connection, tableName);

        string idColumn = PickColumn(columns, "WebsiteId", "Id");
        string nameColumn = PickColumn(columns, "WebsiteName", "Name", "DisplayName", "Title", "Url");
        string activeExpression = columns.Contains("IsActive") ? "[IsActive]" : "1";

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            $"""
            SELECT
                [{idColumn}] AS [Id],
                [{nameColumn}] AS [Name],
                {activeExpression} AS [IsActive]
            FROM [{tableName}]
            ORDER BY [{nameColumn}]
            LIMIT 500;
            """;

        using SqliteDataReader reader = command.ExecuteReader();

        DataTable table = new();
        table.Load(reader);

        return table;
    }

    private static HashSet<string> GetColumns(SqliteConnection connection, string tableName)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT [name] FROM pragma_table_info(@TableName);";
        command.Parameters.AddWithValue("@TableName", tableName);

        HashSet<string> columns = new(StringComparer.OrdinalIgnoreCase);

        using SqliteDataReader reader = command.ExecuteReader();

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

        throw new InvalidOperationException($"Could not find a matching column. Candidates: {string.Join(", ", candidates)}");
    }
}
