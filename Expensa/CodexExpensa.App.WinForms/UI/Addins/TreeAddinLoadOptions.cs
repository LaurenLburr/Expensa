namespace CodexExpensa.App.WinForms.UI.Addins;

public sealed class TreeAddinLoadOptions
{
    public string SearchText { get; init; } = string.Empty;

    public bool IncludeInactive { get; init; }

    public int MaximumRows { get; init; } = 500;

    public bool ExpandAll { get; init; }
}
