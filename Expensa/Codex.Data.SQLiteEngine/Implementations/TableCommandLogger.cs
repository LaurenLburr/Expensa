using Codex.Data.SQLiteEngine.Abstractions;
using Codex.Data.SQLiteEngine.Configuration;
using Microsoft.Data.Sqlite;

namespace Codex.Data.SQLiteEngine.Implementations;

public sealed class TableCommandLogger : ICommandLogger
{
    private readonly SqliteConnection _connection;
    private readonly LoggingOptions _options;

    public TableCommandLogger(SqliteConnection connection, LoggingOptions options)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public void Log(string action, string? details)
    {
        using SqliteCommand cmd = _connection.CreateCommand();
        cmd.CommandText = $"INSERT INTO {_options.TableName} ({_options.TimestampColumn}, {_options.ActionColumn}, {_options.DetailsColumn}) VALUES (CURRENT_TIMESTAMP, @action, @details);";
        cmd.Parameters.AddWithValue("@action", action);
        cmd.Parameters.AddWithValue("@details", (object?)details ?? DBNull.Value);
        cmd.ExecuteNonQuery();
    }

    public void Log(string action, string? details, ISqliteTransactionScope tx)
    {
        if (tx is null)
            throw new ArgumentNullException(nameof(tx));

        string sql =
            $"INSERT INTO {_options.TableName} ({_options.TimestampColumn}, {_options.ActionColumn}, {_options.DetailsColumn}) VALUES (CURRENT_TIMESTAMP, @action, @details);";

        tx.ExecuteNonQuery(
            sql,
            new[]
            {
                new SqliteParameter("@action", action),
                new SqliteParameter("@details", (object?)details ?? DBNull.Value),
            },
            queryName: null);
    }
}
