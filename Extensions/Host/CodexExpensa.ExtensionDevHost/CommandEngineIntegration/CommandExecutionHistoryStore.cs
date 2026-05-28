namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class CommandExecutionHistoryStore : ICommandExecutionHistoryStore
{
    private readonly List<CommandExecutionHistoryRecord> _records = [];

    public void Add(
        CommandExecutionHistoryRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);

        _records.Add(record);
    }

    public IReadOnlyList<CommandExecutionHistoryRecord> ListAll()
    {
        return _records
            .OrderByDescending(static record => record.StartedUtc)
            .ToList();
    }

    public void Clear()
    {
        _records.Clear();
    }
}
