using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class DashboardViewStateTests
{
    [Fact]
    public void NewState_DefaultsToRuntimeCommands()
    {
        DashboardViewState state = new();

        Assert.Equal(DashboardViewMode.RuntimeCommands, state.Mode);
        Assert.True(state.IsRuntimeCommands);
        Assert.False(state.IsManifestRegistry);
        Assert.False(state.IsExecutionHistory);
        Assert.False(state.IsExecutionQueue);
    }

    [Fact]
    public void SetMode_UpdatesHelpers()
    {
        DashboardViewState state = new();

        state.SetMode(DashboardViewMode.ExecutionQueue);

        Assert.True(state.IsExecutionQueue);
        Assert.False(state.IsRuntimeCommands);
    }

    [Fact]
    public void Formatter_ReturnsFriendlyText()
    {
        Assert.Equal(
            "Execution Queue",
            DashboardViewModeTextFormatter.Format(DashboardViewMode.ExecutionQueue));
    }
}
