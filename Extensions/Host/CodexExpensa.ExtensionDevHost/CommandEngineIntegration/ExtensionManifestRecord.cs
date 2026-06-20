namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class ExtensionManifestRecord
{
    public required string ExtensionId { get; init; }

    public string DisplayName { get; init; } = string.Empty;

    public int DisplaySort { get; init; }

    public string Version { get; init; } = string.Empty;

    public string AssemblyFile { get; init; } = string.Empty;

    public string ProviderType { get; init; } = string.Empty;

    public string MinimumHostVersion { get; init; } = string.Empty;

    public bool Enabled { get; init; }

    public string Description { get; init; } = string.Empty;

    public string ManifestFolder { get; init; } = string.Empty;

    public string ManifestPath { get; init; } = string.Empty;

    public bool IsDuplicate { get; init; }

    public int DuplicateCount { get; init; } = 1;
}
