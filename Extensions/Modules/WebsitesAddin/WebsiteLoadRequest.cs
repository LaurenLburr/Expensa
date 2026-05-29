namespace WebsitesAddin;

public sealed class WebsiteLoadRequest
{
    public string SearchText { get; init; } = string.Empty;

    public bool IncludeDisabled { get; init; }

    public int MaximumRows { get; init; } = 500;
}
