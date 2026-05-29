namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed partial class ExtensionRuntimeDashboardForm
{
    private async void OpenPersistentExecutionExplorer(
        object? sender,
        EventArgs e)
    {
        using PersistentExecutionExplorerDialog dialog = new(PersistenceStore);

        if (dialog.ShowDialog(this) != DialogResult.OK || dialog.SelectedRecord is null)
        {
            return;
        }

        diagnosticsTextBox.Text =
            CommandExecutionPersistentRecordTextFormatter.Format(
                "Selected Persisted Execution Record",
                [dialog.SelectedRecord]);

        if (!dialog.ReplayRequested)
        {
            return;
        }

        DialogResult response =
            MessageBox.Show(
                this,
                $"Replay persisted command '{dialog.SelectedRecord.CommandName}' with its saved parameters?",
                "Replay Persisted Command",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

        if (response != DialogResult.Yes)
        {
            return;
        }

        await ExecuteCommandAndRecordAsync(
            CommandExecutionPersistentRecordReplayMapper.GetCommandName(dialog.SelectedRecord),
            CommandExecutionPersistentRecordReplayMapper.GetParameterJson(dialog.SelectedRecord)).ConfigureAwait(true);
    }
}
