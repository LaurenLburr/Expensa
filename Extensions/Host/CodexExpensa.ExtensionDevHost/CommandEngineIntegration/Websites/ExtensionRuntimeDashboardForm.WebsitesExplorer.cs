using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed partial class ExtensionRuntimeDashboardForm
{
    private void OpenWebsitesRuntimeExplorer(
        object? sender,
        EventArgs e)
    {
        using WebsitesRuntimeExplorerForm form = new();

        form.ShowDialog(this);
    }
}
