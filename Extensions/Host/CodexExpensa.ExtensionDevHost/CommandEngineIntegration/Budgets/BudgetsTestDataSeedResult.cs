namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed class BudgetsTestDataSeedResult
{
    public required string DevDatabasePath { get; init; }

    public required string ScriptPath { get; init; }

    public int BudgetMonthCount { get; init; }

    public int BudgetMonthRowCount { get; init; }

    public int TransactionCount { get; init; }

    public IReadOnlyList<string> Statuses { get; init; } =
        Array.Empty<string>();
}
