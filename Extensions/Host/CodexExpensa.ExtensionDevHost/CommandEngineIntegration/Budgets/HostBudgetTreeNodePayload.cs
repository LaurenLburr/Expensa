namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed class HostBudgetTreeNodePayload
{
    public required HostBudgetTreeNodeType NodeType { get; init; }
    public required string NodeId { get; init; }
    public required string DisplayText { get; init; }
    public int? BudgetYear { get; init; }
    public int? BudgetMonth { get; init; }
    public string MonthKey { get; init; } = string.Empty;

    public override string ToString()
    {
        return
            $"NodeType: {NodeType}{Environment.NewLine}" +
            $"NodeId: {NodeId}{Environment.NewLine}" +
            $"DisplayText: {DisplayText}{Environment.NewLine}" +
            $"BudgetYear: {BudgetYear}{Environment.NewLine}" +
            $"BudgetMonth: {BudgetMonth}{Environment.NewLine}" +
            $"MonthKey: {MonthKey}";
    }
}
