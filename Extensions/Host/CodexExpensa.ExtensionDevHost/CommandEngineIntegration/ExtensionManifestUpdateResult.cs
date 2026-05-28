namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class ExtensionManifestUpdateResult
{
    public bool Success { get; init; }

    public string Message { get; init; } = string.Empty;

    public ExtensionManifest? Manifest { get; init; }
}
