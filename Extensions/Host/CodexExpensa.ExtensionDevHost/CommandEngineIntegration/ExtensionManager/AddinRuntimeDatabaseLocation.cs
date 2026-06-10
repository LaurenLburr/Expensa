namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed class AddinRuntimeDatabaseLocation
{
    public required string AddinId { get; init; }

    public required string DatabaseName { get; init; }

    public required string DatabasePath { get; init; }

    public string DatabaseFolder =>
        Path.GetDirectoryName(DatabasePath) ?? string.Empty;
}
