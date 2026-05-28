using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class ExtensionRuntimeSmokeTestTests
{
    [Fact]
    public void CreateRuntime_RegistersSmokeTestCommand()
    {
        IExtensionCommandRuntime runtime =
            ExtensionRuntimeSmokeTest.CreateRuntime();

        IReadOnlyList<RuntimeCommandDescriptor> commands =
            runtime.ListCommands();

        Assert.Single(commands);
        Assert.Equal(ExtensionSmokeTestCommandHandler.RegisteredCommandName, commands[0].CommandName);
        Assert.Equal("Extension Smoke Test", commands[0].DisplayName);
        Assert.Equal("Extension Diagnostics", commands[0].Category);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsSuccessfulSmokeTestResult()
    {
        CommandExecutionResult result =
            await ExtensionRuntimeSmokeTest.ExecuteAsync();

        Assert.Equal(CommandExecutionStatus.Succeeded, result.Status);
        Assert.Equal(ExtensionSmokeTestCommandHandler.RegisteredCommandName, result.CommandName);
        Assert.Contains("smoke test completed", result.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("\"ok\":true", result.OutputJson);
    }

    [Fact]
    public async Task SmokeRuntime_ExecutesCommandThroughExtensionBoundary()
    {
        IExtensionCommandRuntime runtime =
            ExtensionRuntimeSmokeTest.CreateRuntime();

        CommandExecutionResult result =
            await runtime.ExecuteCommandAsync(new ExtensionRuntimeCommandRequest
            {
                CommandName = ExtensionSmokeTestCommandHandler.RegisteredCommandName,
                CorrelationId = "correlation-001"
            });

        Assert.Equal(CommandExecutionStatus.Succeeded, result.Status);
        Assert.Equal("correlation-001", result.CorrelationId);
    }
}
