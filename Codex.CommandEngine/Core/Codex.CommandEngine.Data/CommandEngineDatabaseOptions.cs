namespace Codex.CommandEngine.Data;

public sealed class CommandEngineDatabaseOptions
{
    public required string DatabasePath { get; init; }

    public static CommandEngineDatabaseOptions ForDefaultUserDatabase()
    {
        return new CommandEngineDatabaseOptions
        {
            DatabasePath = CommandEngineDatabasePaths.DefaultDatabasePath
        };
    }

    public static CommandEngineDatabaseOptions ForFile(string databasePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);

        return new CommandEngineDatabaseOptions
        {
            DatabasePath = databasePath
        };
    }
}
