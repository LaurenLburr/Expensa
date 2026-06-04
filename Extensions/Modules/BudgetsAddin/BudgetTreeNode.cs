namespace BudgetsAddin;

public sealed class BudgetTreeNode
{
    public required string NodeId { get; init; }

    public required string DisplayText { get; init; }

    public required string NodeType { get; init; }

    public int? BudgetYear { get; init; }

    public int? BudgetMonth { get; init; }

    public string MonthKey { get; init; } = string.Empty;

    public IReadOnlyList<BudgetTreeNode> Children { get; init; } = [];
}
