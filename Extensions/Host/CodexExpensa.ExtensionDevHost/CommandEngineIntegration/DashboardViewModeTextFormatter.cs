namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public static class DashboardViewModeTextFormatter
{
    public static string Format(
        DashboardViewMode mode)
    {
        return mode switch
        {
            DashboardViewMode.RuntimeCommands => "Runtime Commands",
            DashboardViewMode.ManifestRegistry => "Manifest Registry",
            DashboardViewMode.ExecutionHistory => "Execution History",
            DashboardViewMode.ExecutionQueue => "Execution Queue",
            _ => mode.ToString()
        };
    }
}
