namespace CodexExpensa.App.WinForms.UI.Websites;

public sealed class WebsiteTreeLoadResult
{
    public required IReadOnlyList<WebsiteTreeNode> Nodes { get; init; }

    public int TotalCount { get; init; }

    public string Message { get; init; } = string.Empty;

    public string ExecutionStatus { get; init; } = string.Empty;

    public string OutputJson { get; init; } = string.Empty;
}
