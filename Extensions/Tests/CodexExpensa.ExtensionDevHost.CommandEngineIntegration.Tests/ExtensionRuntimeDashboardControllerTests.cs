using Codex.CommandEngine.Core;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class ExtensionRuntimeDashboardControllerTests
{
    [Fact]
    public void GetSnapshot_BeforeStart_ReturnsManagerSnapshot()
    {
        ExtensionRuntimeManager manager = new();
        ExtensionRuntimeDashboardController controller = new(manager);

        ExtensionRuntimeManagerSnapshot snapshot =
            controller.GetSnapshot();

        Assert.Equal(ExtensionRuntimeManagerStatus.NotStarted, snapshot.Status);
        Assert.Contains("not been started", snapshot.Host.Summary, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void StartSmokeRuntime_ReturnsStartedSnapshotWithCommands()
    {
        ExtensionRuntimeManager manager = new();
        ExtensionRuntimeDashboardController controller = new(manager);

        ExtensionRuntimeManagerSnapshot snapshot =
            controller.StartSmokeRuntime();

        Assert.Equal(ExtensionRuntimeManagerStatus.Started, snapshot.Status);
        Assert.NotEmpty(snapshot.Host.Commands);
        Assert.Contains(snapshot.Host.Commands, command =>
            string.Equals(
                command.CommandName,
                ExtensionSmokeTestCommandHandler.RegisteredCommandName,
                StringComparison.Ordinal));
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
        Assert.Contains("smoke", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ReloadSmokeRuntime_AfterStart_ReturnsStartedSnapshotWithCommands()
    {
        ExtensionRuntimeManager manager = new();
        ExtensionRuntimeDashboardController controller = new(manager);

        controller.StartSmokeRuntime();

        ExtensionRuntimeManagerSnapshot snapshot =
            controller.ReloadSmokeRuntime();

        Assert.Equal(ExtensionRuntimeManagerStatus.Started, snapshot.Status);
        Assert.NotEmpty(snapshot.Host.Commands);
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
        Assert.Contains("stopped", snapshot.Host.Summary, StringComparison.OrdinalIgnoreCase);
    }
}
