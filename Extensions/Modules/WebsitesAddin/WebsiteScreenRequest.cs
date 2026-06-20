namespace WebsitesAddin;

public sealed class WebsiteScreenRequest
{
    public string DatabasePath { get; init; } = string.Empty;

    public string NodeId { get; init; } = string.Empty;

    public string NodeType { get; init; } = string.Empty;

    public string EntityId { get; init; } = string.Empty;

    public string DisplayText { get; init; } = string.Empty;

    public string Url { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public bool? IsActive { get; init; }
}
