namespace CodexExpensa.App.WinForms.UI.Websites;

public sealed class WebsiteLoadResult
{
    public List<WebsiteTreeNode> Nodes { get; init; } = [];

    public int TotalCount { get; init; }

    public string Message { get; init; } = string.Empty;
}
