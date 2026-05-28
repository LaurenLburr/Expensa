namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class DashboardViewState
{
    public DashboardViewMode Mode { get; private set; } =
        DashboardViewMode.RuntimeCommands;

    public bool IsRuntimeCommands =>
        Mode == DashboardViewMode.RuntimeCommands;

    public bool IsManifestRegistry =>
        Mode == DashboardViewMode.ManifestRegistry;

    public bool IsExecutionHistory =>
        Mode == DashboardViewMode.ExecutionHistory;

    public bool IsExecutionQueue =>
        Mode == DashboardViewMode.ExecutionQueue;

    public void SetMode(
        DashboardViewMode mode)
    {
        Mode = mode;
    }
}
