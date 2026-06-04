namespace BudgetsAddin;

public sealed class BudgetLoadRequest
{
    public string DatabasePath { get; init; } = string.Empty;

    public string SearchText { get; init; } = string.Empty;

    public bool IncludeClosed { get; init; }

    public int MaximumRows { get; init; } = 500;
}
