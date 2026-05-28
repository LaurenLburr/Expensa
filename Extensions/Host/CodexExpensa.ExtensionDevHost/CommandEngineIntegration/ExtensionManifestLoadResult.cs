namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class ExtensionManifestLoadResult
{
    public ExtensionManifest? Manifest { get; init; }

    public IReadOnlyList<string> Errors { get; init; } =
        [];

    public bool Success =>
        Manifest is not null && Errors.Count == 0;
}
