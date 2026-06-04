namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed class BudgetFlatRow
{
    public required string NodeId { get; init; }

    public required string DisplayText { get; init; }

    public int? BudgetYear { get; init; }

    public int? BudgetMonth { get; init; }

    public string MonthKey { get; init; } = string.Empty;
}
