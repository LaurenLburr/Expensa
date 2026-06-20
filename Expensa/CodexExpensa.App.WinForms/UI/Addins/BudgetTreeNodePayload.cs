namespace CodexExpensa.App.WinForms.UI.Addins;

public sealed record BudgetTreeNodePayload(
    string NodeId,
    string NodeType,
    string DisplayText,
    int? Year,
    int? Month);
