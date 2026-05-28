namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class ExtensionManifestValidationResult
{
    public IReadOnlyList<string> Errors { get; init; } =
        [];

    public bool IsValid => Errors.Count == 0;
}
