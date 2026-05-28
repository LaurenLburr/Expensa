using Codex.CommandEngine.Core;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class ExtensionRuntimeDashboardControllerTests
{
    [Fact]
    public void StartSmokeRuntime_ReturnsStartedSnapshot()
    {
        ExtensionRuntimeManager manager = new();
        ExtensionRuntimeDashboardController controller = new(manager);

        ExtensionRuntimeManagerSnapshot snapshot =
            controller.StartSmokeRuntime();

        Assert.Equal(ExtensionRuntimeManagerStatus.Started, snapshot.Status);
        Assert.Single(snapshot.Host.Commands);
        Assert.Equal(ExtensionSmokeTestCommandHandler.RegisteredCommandName, snapshot.Host.Commands[0].CommandName);
    }

    [Fact]
    public async Task ExecuteCommandAsync_AfterStartSmokeRuntime_ReturnsSuccess()
    {
        ExtensionRuntimeManager manager = new();
        ExtensionRuntimeDashboardController controller = new(manager);

        controller.StartSmokeRuntime();

        CommandExecutionResult result =
            await controller.ExecuteCommandAsync(
                ExtensionSmokeTestCommandHandler.RegisteredCommandName);

        Assert.Equal(CommandExecutionStatus.Succeeded, result.Status);
    }

    [Fact]
    public void ReloadSmokeRuntime_AfterStart_ReturnsStartedSnapshot()
    {
        ExtensionRuntimeManager manager = new();
        ExtensionRuntimeDashboardController controller = new(manager);

        controller.StartSmokeRuntime();

        ExtensionRuntimeManagerSnapshot snapshot =
            controller.ReloadSmokeRuntime();

        Assert.Equal(ExtensionRuntimeManagerStatus.Started, snapshot.Status);
        Assert.Single(snapshot.Host.Commands);
    }

    [Fact]
    public void StopRuntime_AfterStart_ReturnsStoppedSnapshot()
    {
        ExtensionRuntimeManager manager = new();
        ExtensionRuntimeDashboardController controller = new(manager);

        controller.StartSmokeRuntime();

        ExtensionRuntimeManagerSnapshot snapshot =
            controller.StopRuntime();

        Assert.Equal(ExtensionRuntimeManagerStatus.Stopped, snapshot.Status);
        Assert.Equal("Runtime is stopped.", snapshot.Host.Summary);
    }
}
