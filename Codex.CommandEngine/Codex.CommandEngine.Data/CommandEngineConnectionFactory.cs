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

    public bool ShouldRunInitializerAfterOpen => _options.RunInitializerWhenCopiedFromDev;

    public SqliteConnection OpenConnection()
    {
        EnsureRuntimeDatabase();

        SqliteConnection connection = new(BuildConnectionString(_options.DatabasePath));
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "PRAGMA foreign_keys = ON;";
        command.ExecuteNonQuery();

        return connection;
    }

    private void EnsureRuntimeDatabase()
    {
        string? runtimeFolder = Path.GetDirectoryName(_options.DatabasePath);
        if (!string.IsNullOrWhiteSpace(runtimeFolder))
        {
            Directory.CreateDirectory(runtimeFolder);
        }

        if (!_options.CopyDevDatabaseToRuntime)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_options.DevDatabaseSourcePath))
        {
            return;
        }

        if (!File.Exists(_options.DevDatabaseSourcePath))
        {
            return;
        }

        if (File.Exists(_options.DatabasePath) && !_options.OverwriteRuntimeDatabaseFromDev)
        {
            return;
        }

        File.Copy(
            _options.DevDatabaseSourcePath,
            _options.DatabasePath,
            overwrite: _options.OverwriteRuntimeDatabaseFromDev);
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
