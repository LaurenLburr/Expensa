namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed partial class ExtensionRuntimeDashboardForm
{
    private void UpdateStatusPanel(
        string mode,
        string runtimeStatus,
        int rowCount)
    {
        statusDetailsLabel.Text =
            DashboardStatusTextFormatter.Format(new DashboardStatusViewModel
            {
                Mode = mode,
                RuntimeStatus = runtimeStatus,
                RowCount = rowCount,
                Recursive = recursiveCheckBox.Checked,
                FolderPath = folderTextBox.Text,
                ShowingDuplicates = _showDuplicateManifests
            });
    }
}
