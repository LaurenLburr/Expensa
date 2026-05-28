namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class ExtensionManifestRegistrySnapshot
{
    public IReadOnlyList<ExtensionManifestRecord> Records { get; init; } = [];

    public IReadOnlyList<string> Errors { get; init; } = [];

    public int EnabledCount => Records.Count(static record => record.Enabled);

    public int DisabledCount => Records.Count(static record => !record.Enabled);

    public int DuplicateCount => Records.Count(static record => record.IsDuplicate);

    public bool HasErrors => Errors.Count > 0;
}
