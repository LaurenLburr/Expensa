namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class HostWebsiteRuntimeLoadRequest
{
    public string SearchText { get; init; } = string.Empty;

    public bool IncludeDisabled { get; init; }

    public int MaximumRows { get; init; } = 500;
}
