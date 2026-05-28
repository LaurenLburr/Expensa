namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed partial class ExtensionRuntimeDashboardForm
{
    private void ShowPersistedExecutionCounts(
        object? sender,
        EventArgs e)
    {
        try
        {
            IReadOnlyList<CommandExecutionPersistentRecord> queueItems =
                PersistenceStore.ListQueueItems();

            IReadOnlyList<CommandExecutionPersistentRecord> historyRecords =
                PersistenceStore.ListHistoryRecords();

            diagnosticsTextBox.Text =
                $"Persistent Execution Store{Environment.NewLine}" +
                $"=========================={Environment.NewLine}{Environment.NewLine}" +
                $"Queue records: {queueItems.Count}{Environment.NewLine}" +
                $"History records: {historyRecords.Count}{Environment.NewLine}";

            summaryLabel.Text =
                $"Persistent store: {queueItems.Count} queue record(s), {historyRecords.Count} history record(s).";

            UpdateStatusPanel(
                DashboardViewModeTextFormatter.Format(_viewState.Mode),
                "Persistence",
                queueItems.Count + historyRecords.Count);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                exception.Message,
                "Command Execution Persistence",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void ShowPersistedQueueRecords(
        object? sender,
        EventArgs e)
    {
        try
        {
            IReadOnlyList<CommandExecutionPersistentRecord> records =
                PersistenceStore.ListQueueItems();

            CommandExecutionPersistentRecordListViewBuilder.ConfigureColumns(commandListView);
            CommandExecutionPersistentRecordListViewBuilder.Populate(commandListView, records);

            diagnosticsTextBox.Text =
                CommandExecutionPersistentRecordTextFormatter.Format("Persisted Queue Records", records);

            summaryLabel.Text =
                $"Persisted queue records: {records.Count}.";

            UpdateStatusPanel(
                "Persisted Queue",
                "Persistence",
                records.Count);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                exception.Message,
                "Command Execution Persistence",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void ShowPersistedHistoryRecords(
        object? sender,
        EventArgs e)
    {
        try
        {
            IReadOnlyList<CommandExecutionPersistentRecord> records =
                PersistenceStore.ListHistoryRecords();

            CommandExecutionPersistentRecordListViewBuilder.ConfigureColumns(commandListView);
            CommandExecutionPersistentRecordListViewBuilder.Populate(commandListView, records);

            diagnosticsTextBox.Text =
                CommandExecutionPersistentRecordTextFormatter.Format("Persisted History Records", records);

            summaryLabel.Text =
                $"Persisted history records: {records.Count}.";

            UpdateStatusPanel(
                "Persisted History",
                "Persistence",
                records.Count);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                exception.Message,
                "Command Execution Persistence",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
