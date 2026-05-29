namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed partial class ExtensionRuntimeDashboardForm
{
    private void OpenPersistentExecutionRetentionDialog(
        object? sender,
        EventArgs e)
    {
        using PersistentExecutionRetentionDialog dialog =
            new(PersistenceStore);

        dialog.ShowDialog(this);
    }
}
