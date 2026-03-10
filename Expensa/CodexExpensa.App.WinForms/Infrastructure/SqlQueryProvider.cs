using System;
using System.Collections.Generic;
using CodexExpensa.Core.Abstractions;
using Microsoft.Data.Sqlite;

namespace CodexExpensa.Infrastructure.SqlQueries;

public sealed class SqlQueryProvider : ISqlQueryProvider
{
    private readonly SqliteConnection _connection;

    private readonly Dictionary<string, string> _cache =
        new(StringComparer.Ordinal);

    public SqlQueryProvider(SqliteConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public string GetSql(string queryName)
    {
        if (string.IsNullOrWhiteSpace(queryName))
        {
            throw new ArgumentException("Query name is required.", nameof(queryName));
        }

        if (_cache.TryGetValue(queryName, out string? sql))
        {
            return sql;
        }

        const string lookupSql = """
SELECT
    [SqlText]
FROM [SqlQuery]
WHERE [QueryName] = @QueryName
AND [IsActive] = 1;
""";

        using SqliteCommand command = _connection.CreateCommand();

        command.CommandText = lookupSql;
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
}