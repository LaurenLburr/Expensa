using Codex.CommandEngine.Core;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class ExtensionRuntimeManagerTests
{
    [Fact]
    public void Status_BeforeStart_IsNotStarted()
    {
        ExtensionRuntimeManager manager = new();

        Assert.Equal(ExtensionRuntimeManagerStatus.NotStarted, manager.Status);
    }

    [Fact]
    public void GetSnapshot_BeforeStart_ReturnsNotStarted()
    {
        ExtensionRuntimeManager manager = new();

        ExtensionRuntimeManagerSnapshot snapshot =
            manager.GetSnapshot();

        Assert.Equal(ExtensionRuntimeManagerStatus.NotStarted, snapshot.Status);
        Assert.Contains("not been started", snapshot.Host.Summary, StringComparison.OrdinalIgnoreCase);
        Assert.False(string.IsNullOrWhiteSpace(snapshot.DiagnosticText));
    }

    [Fact]
    public void Start_WithSmokeProvider_ReturnsStartedSnapshot()
    {
        ExtensionRuntimeManager manager = new();

        manager.Start(SmokeRuntimeProviderFactory.Create());

        ExtensionRuntimeManagerSnapshot snapshot =
            manager.GetSnapshot();

        Assert.Equal(ExtensionRuntimeManagerStatus.Started, snapshot.Status);
        Assert.NotEmpty(snapshot.Host.Commands);
        Assert.Contains(snapshot.Host.Commands, command =>
            string.Equals(
                command.CommandName,
                ExtensionSmokeTestCommandHandler.RegisteredCommandName,
                StringComparison.Ordinal));
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
        Assert.Equal(ExtensionSmokeTestCommandHandler.RegisteredCommandName, result.CommandName);
        Assert.Contains("smoke", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Reload_WithSmokeProvider_KeepsRuntimeStarted()
    {
        ExtensionRuntimeManager manager = new();

        manager.Start(SmokeRuntimeProviderFactory.Create());
        manager.Reload(SmokeRuntimeProviderFactory.Create());

        ExtensionRuntimeManagerSnapshot snapshot =
            manager.GetSnapshot();

        Assert.Equal(ExtensionRuntimeManagerStatus.Started, snapshot.Status);
        Assert.NotEmpty(snapshot.Host.Commands);
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
        Assert.Contains("stopped", snapshot.Host.Summary, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ExecuteCommandAsync_BeforeStart_ReturnsFailureInsteadOfThrowing()
    {
        ExtensionRuntimeManager manager = new();

        CommandExecutionResult result =
            await manager.ExecuteCommandAsync(
                ExtensionSmokeTestCommandHandler.RegisteredCommandName);

        Assert.Equal(CommandExecutionStatus.Failed, result.Status);
    }
}
