using Microsoft.Data.Sqlite;

namespace Codex.CommandEngine.Data;

public sealed class CommandEngineDatabaseBootstrapper
{
    private readonly CommandEngineConnectionFactory _connectionFactory;

    public CommandEngineDatabaseBootstrapper(CommandEngineConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);

        _connectionFactory = connectionFactory;
    }

    public void RebuildFromSqlScript(
        string schemaSql)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(schemaSql);

        using SqliteConnection connection =
            _connectionFactory.OpenConnection();

        RebuildFromSqlScript(connection, schemaSql);
    }

    public static void RebuildFromSqlScript(
        SqliteConnection connection,
        string schemaSql)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentException.ThrowIfNullOrWhiteSpace(schemaSql);

        if (connection.State != System.Data.ConnectionState.Open)
        {
            connection.Open();
        }

        using SqliteCommand command =
            connection.CreateCommand();

        command.CommandText = schemaSql;
        command.ExecuteNonQuery();
    }
}
