namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed class HostBudgetExpensaProdDatabaseCopyResult
{
    public required string ProdDatabasePath { get; init; }

    public required string RuntimeDatabasePath { get; init; }

    public required string DevDatabasePath { get; init; }

    public required string ActiveRuntimePathFile { get; init; }

    public IReadOnlyList<string> Warnings { get; init; } = [];
}
