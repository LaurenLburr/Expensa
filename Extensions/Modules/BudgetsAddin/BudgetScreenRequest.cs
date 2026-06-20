namespace BudgetsAddin;

public sealed class BudgetScreenRequest
{
    public string DatabasePath { get; init; } = string.Empty;

    public string NodeId { get; init; } = string.Empty;

    public string NodeType { get; init; } = string.Empty;

    public string DisplayText { get; init; } = string.Empty;

    public int? Year { get; init; }

    public int? Month { get; init; }
}
