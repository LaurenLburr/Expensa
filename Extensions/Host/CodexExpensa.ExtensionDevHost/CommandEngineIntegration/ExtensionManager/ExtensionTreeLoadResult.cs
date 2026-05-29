namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed class ExtensionTreeLoadResult
{
    public required string AddinId { get; init; }

    public required string DisplayName { get; init; }

    public int SortOrder { get; init; }

    public bool Succeeded { get; init; }

    public string Message { get; init; } = string.Empty;
}
