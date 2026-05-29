namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed partial class PersistentExecutionExplorerDialog
{
    public bool ReplayRequested { get; private set; }

    private void recordsListView_DoubleClick(
        object? sender,
        EventArgs e)
    {
        CommandExecutionPersistentRecord? record =
            GetSelectedRecord();

        if (record is null)
        {
            return;
        }

        using PersistentExecutionDetailDialog dialog = new(record);

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        if (!dialog.ReplayRequested)
        {
            return;
        }

        SelectedRecord = dialog.Record;
        ReplayRequested = true;
        DialogResult = DialogResult.OK;
        Close();
    }
}
