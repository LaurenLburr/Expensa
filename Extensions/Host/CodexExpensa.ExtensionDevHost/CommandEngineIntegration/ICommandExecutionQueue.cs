namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public interface ICommandExecutionQueue
{
    event EventHandler? Changed;

    CommandExecutionQueueItem Enqueue(
        string commandName,
        string parameterJson);

    void AddRecoveredItem(
        CommandExecutionQueueItem item);

    CommandExecutionQueueSnapshot GetSnapshot();

    bool TryCancel(
        Guid queueItemId);

    void ClearCompleted();
}
