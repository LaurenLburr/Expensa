using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.TestHarness;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        using ExtensionRuntimeDashboardForm form = new();
        Application.Run(form);
    }
}
