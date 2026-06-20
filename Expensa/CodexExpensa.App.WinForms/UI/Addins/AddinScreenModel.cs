namespace CodexExpensa.App.WinForms.UI.Addins;

public sealed class AddinScreenModel
{
    public int FormatVersion { get; init; } = 1;

    public string Title { get; init; } = string.Empty;

    public string Subtitle { get; init; } = string.Empty;

    public IReadOnlyList<string> Columns { get; init; } =
        Array.Empty<string>();

    public IReadOnlyList<IReadOnlyDictionary<string, object?>> Rows { get; init; } =
        Array.Empty<IReadOnlyDictionary<string, object?>>();

    public bool IsHierarchical { get; init; }

    public string IdColumnName { get; init; } = string.Empty;

    public string ParentIdColumnName { get; init; } = string.Empty;

    public IReadOnlyList<string> HiddenColumns { get; init; } =
        Array.Empty<string>();

    public bool AllowAddTransaction { get; init; }
}
