namespace CodexExpensa.App.WinForms.UI.Websites;

public sealed class WebsiteTreeLoadOptions
{
    public string SearchText { get; init; } = string.Empty;

    public bool IncludeDisabled { get; init; }

    public int MaximumRows { get; init; } = 500;

    public bool ExpandAll { get; init; }
}
