namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed class HostBudgetDatabaseCopyHistory
{
    private readonly List<HostBudgetDatabaseCopyRecord> records = [];

    public IReadOnlyList<HostBudgetDatabaseCopyRecord> Records =>
        records;

    public void Add(HostBudgetDatabaseCopyRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);

        records.Insert(0, record);
    }
}
