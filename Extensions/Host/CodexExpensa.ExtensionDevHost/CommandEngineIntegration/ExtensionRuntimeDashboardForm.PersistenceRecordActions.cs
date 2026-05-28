namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed partial class ExtensionRuntimeDashboardForm
{
    private CommandExecutionPersistentRecord? GetSelectedPersistentRecord()
    {
        if (commandListView.SelectedItems.Count == 0)
        {
            MessageBox.Show(
                this,
                "Select a persisted execution row first.",
                "Command Execution Persistence",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return null;
        }

        if (commandListView.SelectedItems[0].Tag is CommandExecutionPersistentRecord record)
        {
            return record;
        }

        MessageBox.Show(
            this,
            "Selected row does not contain persisted execution metadata.",
            "Command Execution Persistence",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);

        return null;
    }

    private void ShowSelectedPersistedExecutionDetails(
        object? sender,
        EventArgs e)
    {
        CommandExecutionPersistentRecord? record =
            GetSelectedPersistentRecord();

        if (record is null)
        {
            return;
        }

        diagnosticsTextBox.Text =
            CommandExecutionPersistentRecordTextFormatter.Format(
                "Selected Persisted Execution Record",
                [record]);
    }

    private void CopySelectedPersistedExecutionParameters(
        object? sender,
        EventArgs e)
    {
        CommandExecutionPersistentRecord? record =
            GetSelectedPersistentRecord();

        if (record is not null)
        {
            Clipboard.SetText(record.ParameterJson);
        }
    }

    private void CopySelectedPersistedExecutionOutput(
        object? sender,
        EventArgs e)
    {
        CommandExecutionPersistentRecord? record =
            GetSelectedPersistentRecord();

        if (record is not null)
        {
            Clipboard.SetText(record.OutputJson);
        }
    }

    private async void ReplaySelectedPersistedExecution(
        object? sender,
        EventArgs e)
    {
        CommandExecutionPersistentRecord? record =
            GetSelectedPersistentRecord();

        if (record is null)
        {
            return;
        }

        DialogResult response =
            MessageBox.Show(
                this,
                $"Replay persisted command '{record.CommandName}' with its saved parameters?",
                "Replay Persisted Command",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

        if (response != DialogResult.Yes)
        {
            return;
        }

        await ExecuteCommandAndRecordAsync(record.CommandName, record.ParameterJson).ConfigureAwait(true);
    }
}
