namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;

public sealed class AddinTreeLoadRequest
{
    public string DatabasePath { get; init; } = string.Empty;

    public string SearchText { get; init; } = string.Empty;

    public int MaximumRows { get; init; } = 500;

    public bool IncludeInactive { get; init; }

    public bool ExpandAll { get; init; }
}
