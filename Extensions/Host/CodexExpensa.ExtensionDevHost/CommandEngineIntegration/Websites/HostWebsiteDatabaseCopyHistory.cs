namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class HostWebsiteDatabaseCopyHistory
{
    private readonly List<HostWebsiteDatabaseCopyRecord> _records = [];

    public IReadOnlyList<HostWebsiteDatabaseCopyRecord> Records =>
        _records;

    public void Add(
        HostWebsiteDatabaseCopyRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);

        _records.Insert(0, record);
    }

    public void Clear()
    {
        _records.Clear();
    }
}
