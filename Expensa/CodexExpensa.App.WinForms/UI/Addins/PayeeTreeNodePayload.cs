namespace CodexExpensa.App.WinForms.UI.Addins;

public sealed record PayeeTreeNodePayload(
    string NodeType,
    string PayeeId,
    string DisplayText);
