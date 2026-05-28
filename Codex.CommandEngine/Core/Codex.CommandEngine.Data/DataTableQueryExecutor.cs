using System.Data;
using Microsoft.Data.Sqlite;

namespace Codex.CommandEngine.Data;

public sealed class DataTableQueryExecutor
{
    private readonly CommandEngineConnectionFactory _connectionFactory;

    public DataTableQueryExecutor(CommandEngineConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);

        _connectionFactory = connectionFactory;
    }

    public DataTable ExecuteQuery(string sql)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sql);

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = sql;

        return SqliteDataTableLoader.Load(command);
    }

    public DataTable ExecuteQuery(
        string sql,
        IReadOnlyDictionary<string, object?> parameters)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sql);
        ArgumentNullException.ThrowIfNull(parameters);

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = sql;

        AddParameters(command, parameters);

        return SqliteDataTableLoader.Load(command);
    }

    public int ExecuteNonQuery(
        string sql,
        IReadOnlyDictionary<string, object?> parameters)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sql);
        ArgumentNullException.ThrowIfNull(parameters);

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = sql;

        AddParameters(command, parameters);

        return command.ExecuteNonQuery();
    }

    public object? ExecuteScalar(
        string sql,
        IReadOnlyDictionary<string, object?> parameters)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sql);
        ArgumentNullException.ThrowIfNull(parameters);

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = sql;

        AddParameters(command, parameters);

        return command.ExecuteScalar();
    }

    private static void AddParameters(
        SqliteCommand command,
        IReadOnlyDictionary<string, object?> parameters)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(parameters);

        foreach (KeyValuePair<string, object?> parameter in parameters)
        {
            string parameterName =
                parameter.Key.StartsWith("$", StringComparison.Ordinal)
                    ? parameter.Key
                    : "$" + parameter.Key;

            command.Parameters.AddWithValue(parameterName, parameter.Value ?? DBNull.Value);
        }
    }
}
