namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class HostWebsiteTreeNodePayload : IHostWebsiteTreeNodePayload
{
    public HostWebsiteTreeNodeType NodeType => HostWebsiteTreeNodeType.Website;

    public required string NodeId { get; init; }

    public required string DisplayText { get; init; }

    public required string WebsiteId { get; init; }

    public string TagName { get; init; } = string.Empty;

    public string Url { get; init; } = string.Empty;

    public bool IsActive { get; init; } = true;
}
