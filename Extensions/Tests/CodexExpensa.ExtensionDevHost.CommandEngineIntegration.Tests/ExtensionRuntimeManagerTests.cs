using Codex.CommandEngine.Core;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class ExtensionRuntimeManagerTests
{
    [Fact]
    public void GetSnapshot_BeforeStart_ReturnsNotStarted()
    {
        ExtensionRuntimeManager manager = new();

        ExtensionRuntimeManagerSnapshot snapshot =
            manager.GetSnapshot();

        Assert.Equal(ExtensionRuntimeManagerStatus.NotStarted, snapshot.Status);
        Assert.Contains("Runtime has not been started.", snapshot.Host.Summary);
    }

    [Fact]
    public void Start_WithSmokeProvider_ReturnsStartedSnapshot()
    {
        ExtensionRuntimeManager manager = new();

        manager.Start(SmokeRuntimeProviderFactory.Create());

        ExtensionRuntimeManagerSnapshot snapshot =
            manager.GetSnapshot();

        Assert.Equal(ExtensionRuntimeManagerStatus.Started, snapshot.Status);
        Assert.Single(snapshot.Host.Commands);
        Assert.Equal(ExtensionSmokeTestCommandHandler.RegisteredCommandName, snapshot.Host.Commands[0].CommandName);
    }

    [Fact]
    public async Task ExecuteCommandAsync_WithSmokeProvider_ReturnsSuccess()
    {
        ExtensionRuntimeManager manager = new();

        manager.Start(SmokeRuntimeProviderFactory.Create());

        CommandExecutionResult result =
            await manager.ExecuteCommandAsync(
                ExtensionSmokeTestCommandHandler.RegisteredCommandName);

        Assert.Equal(CommandExecutionStatus.Succeeded, result.Status);
        Assert.Contains("smoke test completed", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Reload_WithSmokeProvider_RestartsRuntime()
    {
        ExtensionRuntimeManager manager = new();

        manager.Start(SmokeRuntimeProviderFactory.Create());
        manager.Reload(SmokeRuntimeProviderFactory.Create());

        ExtensionRuntimeManagerSnapshot snapshot =
            manager.GetSnapshot();

        Assert.Equal(ExtensionRuntimeManagerStatus.Started, snapshot.Status);
        Assert.Single(snapshot.Host.Commands);
    }

    [Fact]
    public void Stop_AfterStart_ReturnsStoppedSnapshot()
    {
        ExtensionRuntimeManager manager = new();

        manager.Start(SmokeRuntimeProviderFactory.Create());
        manager.Stop();

        ExtensionRuntimeManagerSnapshot snapshot =
            manager.GetSnapshot();

        Assert.Equal(ExtensionRuntimeManagerStatus.Stopped, snapshot.Status);
        Assert.Equal("Runtime is stopped.", snapshot.Host.Summary);
    }
}
