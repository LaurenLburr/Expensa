namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed class HostBudgetDatabaseLocation
{
    public required string DatabaseName { get; init; }

    public required string DatabasePath { get; init; }

    public string DatabaseFolder =>
        Path.GetDirectoryName(DatabasePath) ?? string.Empty;
}
