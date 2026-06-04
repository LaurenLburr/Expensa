namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed class HostBudgetDatabaseCopyRecord
{
    public required string SourceLabel { get; init; }

    public required string DatabasePath { get; init; }

    public DateTime CreatedLocal { get; init; } = DateTime.Now;

    public string DatabaseName =>
        Path.GetFileName(DatabasePath);
}
