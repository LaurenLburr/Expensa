namespace WebsitesAddin;

public sealed class WebsiteScreenDocument
{
    public int FormatVersion { get; init; } = 1;

    public string Title { get; init; } = string.Empty;

    public string Subtitle { get; init; } = string.Empty;

    public IReadOnlyList<string> Columns { get; init; } =
        Array.Empty<string>();

    public IReadOnlyList<IReadOnlyDictionary<string, object?>> Rows { get; init; } =
        Array.Empty<IReadOnlyDictionary<string, object?>>();
}
