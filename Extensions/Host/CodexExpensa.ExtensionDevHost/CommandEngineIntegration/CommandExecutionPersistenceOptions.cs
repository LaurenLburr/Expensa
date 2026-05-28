namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class CommandExecutionPersistenceOptions
{
    public string DatabasePath { get; init; } = GetDefaultDatabasePath();

    public static string GetDefaultDatabasePath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions",
            "CommandExecution.db");
    }
}
