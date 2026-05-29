namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public interface ICommandExecutionPersistenceStore
{
    void EnsureCreated();

    void UpsertQueueItem(CommandExecutionQueueItem item);

    void UpsertHistoryRecord(CommandExecutionHistoryRecord record);

    IReadOnlyList<CommandExecutionPersistentRecord> ListQueueItems();

    IReadOnlyList<CommandExecutionPersistentRecord> ListHistoryRecords();

    IReadOnlyList<CommandExecutionPersistentRecord> QueryRecords(
        CommandExecutionPersistentRecordQuery query);

    int DeleteRecords(CommandExecutionRetentionOptions options);

    void DeleteCompletedQueueItems();

    void Vacuum();
}
