namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed partial class ExtensionRuntimeDashboardForm
{
    private void PurgeFailedPersistedExecutions(object? sender, EventArgs e)
    {
        DialogResult response =
            MessageBox.Show(
                this,
                "Delete all persisted failed execution records?",
                "Purge Failed Executions",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

        if (response != DialogResult.Yes)
        {
            return;
        }

        int deleted =
            PersistenceStore.DeleteRecords(new CommandExecutionRetentionOptions
            {
                Status = "Failed"
            });

        diagnosticsTextBox.Text =
            $"Deleted {deleted} failed persisted execution record(s).";
    }

    private void PurgePersistedHistoryOlderThan30Days(object? sender, EventArgs e)
    {
        DialogResult response =
            MessageBox.Show(
                this,
                "Delete persisted history records older than 30 days?",
                "Purge Old History",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

        if (response != DialogResult.Yes)
        {
            return;
        }

        int deleted =
            PersistenceStore.DeleteRecords(new CommandExecutionRetentionOptions
            {
                SourceFilter = CommandExecutionPersistentRecordSourceFilter.History,
                CreatedBeforeUtc = DateTimeOffset.UtcNow.AddDays(-30)
            });

        diagnosticsTextBox.Text =
            $"Deleted {deleted} persisted history record(s) older than 30 days.";
    }

    private void VacuumExecutionDatabase(object? sender, EventArgs e)
    {
        PersistenceStore.Vacuum();

        diagnosticsTextBox.Text =
            "Execution database vacuum completed.";
    }
}
