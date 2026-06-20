namespace CodexExpensa.App.WinForms.UI.Addins;

public sealed class AddinScreenSelection
{
    public string NodeId { get; init; } = string.Empty;

    public string NodeType { get; init; } = string.Empty;

    public string EntityId { get; init; } = string.Empty;

    public string DisplayText { get; init; } = string.Empty;

    public int? Year { get; init; }

    public int? Month { get; init; }

    public string Url { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public bool? IsActive { get; init; }
}
