namespace WebsitesAddin;

public sealed class WebsiteLoadResult
{
    public IReadOnlyList<WebsiteTreeNode> Nodes { get; init; } = [];

    public int TotalCount =>
        Nodes.Count;

    public string Message { get; init; } = string.Empty;
}
