using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed partial class ExtensionRuntimeDashboardForm
{
    private void OpenExtensionManagerAddinTestSurface(
        object? sender,
        EventArgs e)
    {
        using ExtensionManagerAddinTestSurfaceForm form = new();

        form.ShowDialog(this);
    }
}
