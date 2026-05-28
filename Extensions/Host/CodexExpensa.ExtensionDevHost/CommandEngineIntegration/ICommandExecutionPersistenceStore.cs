namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public interface ICommandExecutionPersistenceStore
{
    void EnsureCreated();

    void UpsertQueueItem(CommandExecutionQueueItem item);

    void UpsertHistoryRecord(CommandExecutionHistoryRecord record);

    IReadOnlyList<CommandExecutionPersistentRecord> ListQueueItems();

    IReadOnlyList<CommandExecutionPersistentRecord> ListHistoryRecords();

    void DeleteCompletedQueueItems();
}
