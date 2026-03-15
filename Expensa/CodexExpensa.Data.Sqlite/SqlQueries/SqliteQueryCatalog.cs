using Microsoft.Data.Sqlite;

namespace CodexExpensa.Data.Sqlite.SqlQueries;

internal sealed class SqliteQueryCatalog
{
    private readonly SqliteConnection _connection;
    private readonly Dictionary<string, string> _cache = new(StringComparer.Ordinal);

    public SqliteQueryCatalog(SqliteConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public string GetSql(string queryName)
    {
        System.Diagnostics.Debug.WriteLine("Query " + queryName);

        if (string.IsNullOrWhiteSpace(queryName))
        {
            throw new ArgumentException("Query name is required.", nameof(queryName));
        }

        if (_cache.TryGetValue(queryName, out string? cached))
        {
            return cached;
        }

        const string sql = """
SELECT
    [SqlText]
FROM [SqlQuery]
WHERE [QueryName] = @QueryName
AND [IsActive] = 1
""";

        using SqliteCommand command = _connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue("@QueryName", queryName);

        object? result = command.ExecuteScalar();

        if (result is not string sqlText || string.IsNullOrWhiteSpace(sqlText))
        {
            throw new InvalidOperationException(
                $"Active SQL query was not found for QueryName '{queryName}'.");
        }

        _cache[queryName] = sqlText;
        return sqlText;
    }

    public void ClearCache()
    {
        _cache.Clear();
    }
}