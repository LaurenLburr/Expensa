namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class ExtensionManifestRegistryViewModel
{
    public int TotalCount { get; init; }

    public int EnabledCount { get; init; }

    public int DisabledCount { get; init; }

    public int DuplicateCount { get; init; }

    public int ErrorCount { get; init; }

    public string Summary { get; init; } = string.Empty;

    public IReadOnlyList<ExtensionManifestRecord> Records { get; init; } = [];

    public IReadOnlyList<string> Errors { get; init; } = [];
}
