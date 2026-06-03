namespace CodexExpensa.App.WinForms.UI.Websites;

public sealed class HostWebsiteCategoryGroupTreeNodePayload : IHostWebsiteTreeNodePayload
{
    public required string NodeId { get; init; }

    public string WebsiteId => string.Empty;

    public required string DisplayText { get; init; }

    public string Url => string.Empty;

    public string TagName => string.Empty;

    public HostWebsiteTreeNodeType NodeType =>
        HostWebsiteTreeNodeType.CategoryGroup;

    public bool IsActive => true;
}
