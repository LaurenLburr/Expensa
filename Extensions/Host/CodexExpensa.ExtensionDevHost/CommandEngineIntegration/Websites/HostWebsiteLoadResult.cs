namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class HostWebsiteLoadResult
{
    public IReadOnlyList<HostWebsiteTreeNode> Nodes { get; init; } = [];

    public int TotalCount { get; init; }

    public string Message { get; init; } = string.Empty;
}
