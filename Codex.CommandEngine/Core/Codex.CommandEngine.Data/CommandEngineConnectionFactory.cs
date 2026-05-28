using Microsoft.Data.Sqlite;

namespace Codex.CommandEngine.Data;

public sealed class CommandEngineConnectionFactory
{
    private readonly CommandEngineDatabaseOptions _options;

    public CommandEngineConnectionFactory(CommandEngineDatabaseOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.DatabasePath);

        _options = options;
    }

    public SqliteConnection OpenConnection()
    {
        string? folder = Path.GetDirectoryName(_options.DatabasePath);
        if (!string.IsNullOrWhiteSpace(folder))
        {
            Directory.CreateDirectory(folder);
        }

        SqliteConnection connection = new(BuildConnectionString(_options.DatabasePath));
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "PRAGMA foreign_keys = ON;";
        command.ExecuteNonQuery();

        return connection;
    }

    private static string BuildConnectionString(string databasePath)
    {
        SqliteConnectionStringBuilder builder = new()
        {
            DataSource = databasePath
        };

        return builder.ToString();
    }
}
