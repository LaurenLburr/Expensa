namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed class HostBudgetTreeLoadOptions
{
    public string SearchText { get; init; } = string.Empty;
    public bool IncludeClosed { get; init; }
    public int MaximumRows { get; init; } = 500;
    public bool ExpandAll { get; init; }
}
