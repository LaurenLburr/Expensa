namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class HostWebsiteTreeNode
{
    public required string NodeId { get; init; }

    public required string DisplayText { get; init; }

    public string Url { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public bool IsEnabled { get; init; } = true;

    public IReadOnlyList<HostWebsiteTreeNode> Children { get; init; } = [];
}
