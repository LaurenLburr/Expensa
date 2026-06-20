namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed class AddinDevDatabaseCopyResult
{
    public required string AddinId { get; init; }
    public required string RuntimeDatabasePath { get; init; }
    public required string DevDatabasePath { get; init; }
    public string? ReplacedDevDatabaseArchivePath { get; init; }
}
