namespace CodexExpensa.App.WinForms.UI.Websites;

public sealed class HostWebsiteTreeNodePayload : IHostWebsiteTreeNodePayload
{
    public required string NodeId { get; init; }

    public required string WebsiteId { get; init; }

    public required string DisplayText { get; init; }

    public required string Url { get; init; }

    public required string TagName { get; init; }

    public HostWebsiteTreeNodeType NodeType { get; init; } =
        HostWebsiteTreeNodeType.Website;

    public required bool IsActive { get; init; }
}
