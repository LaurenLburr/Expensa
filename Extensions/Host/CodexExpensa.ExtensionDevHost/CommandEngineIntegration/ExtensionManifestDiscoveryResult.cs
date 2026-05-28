namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class ExtensionManifestDiscoveryResult
{
    public IReadOnlyList<ExtensionManifest> Manifests { get; init; } = [];
    public IReadOnlyList<string> Errors { get; init; } = [];
    public bool Success => Errors.Count == 0;
}
