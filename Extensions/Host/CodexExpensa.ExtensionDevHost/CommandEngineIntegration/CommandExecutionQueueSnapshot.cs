namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class CommandExecutionQueueSnapshot
{
    public IReadOnlyList<CommandExecutionQueueItem> Items { get; init; } = [];

    public int PendingCount =>
        Items.Count(static item => item.Status == CommandExecutionQueueStatus.Pending);

    public int RunningCount =>
        Items.Count(static item => item.Status == CommandExecutionQueueStatus.Running);

    public int CompletedCount =>
        Items.Count(static item => item.Status == CommandExecutionQueueStatus.Completed);

    public int FailedCount =>
        Items.Count(static item => item.Status == CommandExecutionQueueStatus.Failed);

    public int CancelledCount =>
        Items.Count(static item => item.Status == CommandExecutionQueueStatus.Cancelled);
}
