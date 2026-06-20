using System.Data;
using Microsoft.Data.Sqlite;

namespace WebsitesAddin;

public sealed class WebsiteSqlQueryCatalog
{
    private readonly string _databasePath;

    public WebsiteSqlQueryCatalog(
        WebsiteDatabaseOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.DatabasePath);

        _databasePath = options.DatabasePath;
    }

    public string GetSqlText(
        string queryName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(queryName);

        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            "SELECT [SqlText] FROM [SqlQuery] WHERE [QueryName] = @QueryName;";

        command.Parameters.AddWithValue("@QueryName", queryName);

        DataTable table = new();

        using SqliteDataReader reader = command.ExecuteReader();

        table.Load(reader);

        if (table.Rows.Count == 0)
        {
            throw new InvalidOperationException(
                $"SQL query was not found in catalog: {queryName}");
        }

        return Convert.ToString(table.Rows[0]["SqlText"]) ?? string.Empty;
    }

    private SqliteConnection OpenConnection()
    {
        SqliteConnection connection = new($"Data Source={_databasePath};Mode=ReadOnly;Pooling=False");
        connection.Open();
        return connection;
    }
}
