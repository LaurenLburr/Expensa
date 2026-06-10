namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed class AddinRuntimeDatabaseCopyResult
{
    public required string AddinId { get; init; }

    public required string SourceLabel { get; init; }

    public required string SourceDatabasePath { get; init; }

    public required string RuntimeDatabasePath { get; init; }

    public required string ActiveRuntimePathFile { get; init; }

    public string? ReplacedDatabaseArchivePath { get; init; }
}
