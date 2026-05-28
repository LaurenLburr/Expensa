namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed partial class ExtensionRuntimeDashboardForm
{
    private bool _persistentQueueRecovered;

    private void RecoverPersistentQueueIfNeeded()
    {
        if (_persistentQueueRecovered)
        {
            return;
        }

        _persistentQueueRecovered = true;

        try
        {
            IReadOnlyList<CommandExecutionPersistentRecord> persistedQueueItems =
                PersistenceStore.ListQueueItems();

            IReadOnlyList<CommandExecutionQueueItem> recoveredItems =
                CommandExecutionQueueRecoveryMapper.ToRecoveredQueueItems(persistedQueueItems);

            foreach (CommandExecutionQueueItem item in recoveredItems)
            {
                ExecutionQueue.AddRecoveredItem(item);
            }

            if (recoveredItems.Count > 0)
            {
                PersistQueueSnapshot(ExecutionQueue.GetSnapshot());
            }
        }
        catch (Exception exception)
        {
            diagnosticsTextBox.Text =
                $"Failed to recover persisted queue:{Environment.NewLine}{exception}";
        }
    }
}
