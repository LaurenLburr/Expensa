using Codex.Data.SQLiteEngine.Abstractions;
using Codex.Data.SQLiteEngine.Configuration;
using Microsoft.Data.Sqlite;

namespace Codex.Data.SQLiteEngine.Implementations;

public sealed class TableSqlCatalog : ISqlCatalog
{
    private readonly SqliteConnection _connection;
    private readonly SqlCatalogOptions _options;

    public TableSqlCatalog(SqliteConnection connection, SqlCatalogOptions options)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public string GetSql(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("name is required.", nameof(name));

        using SqliteCommand cmd = _connection.CreateCommand();
        cmd.CommandText = $"SELECT {_options.SqlColumn} FROM {_options.TableName} WHERE {_options.NameColumn} = @name LIMIT 1;";
        cmd.Parameters.AddWithValue("@name", name);

        object? result = cmd.ExecuteScalar();

        if (result is null || result is DBNull)
            throw new InvalidOperationException($"SQL not found for '{name}'.");

        return Convert.ToString(result)!;
    }

    public string GetSql(string name, ISqliteTransactionScope tx)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("name is required.", nameof(name));
        if (tx is null)
            throw new ArgumentNullException(nameof(tx));

        const string alias = "SqlText";
        string sql =
            $"SELECT {_options.SqlColumn} AS {alias} FROM {_options.TableName} WHERE {_options.NameColumn} = @name LIMIT 1;";

        var rows = tx.Query(
            sql,
            r => r.GetString(r.GetOrdinal(alias)),
            new[] { new SqliteParameter("@name", name) },
            queryName: null);

        if (rows.Count == 0)
            throw new InvalidOperationException($"SQL not found for '{name}'.");

        return rows[0];
    }
}
