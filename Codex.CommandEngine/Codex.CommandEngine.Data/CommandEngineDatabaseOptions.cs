namespace Codex.CommandEngine.Data;

public sealed class CommandEngineDatabaseOptions
{
    public required string DatabasePath { get; init; }

    public string? DevDatabaseSourcePath { get; init; }

    public bool CopyDevDatabaseToRuntime { get; init; } = true;

    public bool OverwriteRuntimeDatabaseFromDev { get; init; }

    public bool RunInitializerWhenCopiedFromDev { get; init; }

    public static CommandEngineDatabaseOptions ForDefaultUserDatabase()
    {
        return new CommandEngineDatabaseOptions
        {
            DatabasePath = CommandEngineDatabasePaths.DefaultDatabasePath,
            DevDatabaseSourcePath = CommandEngineDatabasePaths.ResolveDevDatabasePath(),
            CopyDevDatabaseToRuntime = true,
            OverwriteRuntimeDatabaseFromDev = false,
            RunInitializerWhenCopiedFromDev = false
        };
    }

    public static CommandEngineDatabaseOptions ForFile(string databasePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);

        return new CommandEngineDatabaseOptions
        {
            DatabasePath = databasePath,
            DevDatabaseSourcePath = CommandEngineDatabasePaths.ResolveDevDatabasePath(),
            CopyDevDatabaseToRuntime = true,
            OverwriteRuntimeDatabaseFromDev = false,
            RunInitializerWhenCopiedFromDev = false
        };
    }

    public static CommandEngineDatabaseOptions ForFile(
        string databasePath,
        string devDatabaseSourcePath,
        bool overwriteRuntimeDatabaseFromDev = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(devDatabaseSourcePath);

        return new CommandEngineDatabaseOptions
        {
            DatabasePath = databasePath,
            DevDatabaseSourcePath = devDatabaseSourcePath,
            CopyDevDatabaseToRuntime = true,
            OverwriteRuntimeDatabaseFromDev = overwriteRuntimeDatabaseFromDev,
            RunInitializerWhenCopiedFromDev = false
        };
    }
}
