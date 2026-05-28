using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class CommandEngineRuntimeBootstrapperTests
{
    [Fact]
    public void Bootstrap_WithCommands_ReturnsRuntimeAndRegisteredDescriptors()
    {
        CommandEngineRuntimeBootstrapper bootstrapper = new();

        CommandEngineRuntimeBootstrapResult result =
            bootstrapper.Bootstrap(new CommandEngineRuntimeBootstrapRequest
            {
                Commands =
                [
                    new RuntimeCommandRegistration
                    {
                        Handler = new TestCommandHandler("extension.open"),
                        DisplayName = "Open Extension",
                        Category = "Extension"
                    }
                ]
            });

        Assert.Single(result.RegisteredCommands);
        Assert.Equal("extension.open", result.RegisteredCommands[0].CommandName);
    }

    [Fact]
    public void Bootstrap_WhenRequiredButNoCommands_Throws()
    {
        CommandEngineRuntimeBootstrapper bootstrapper = new();

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => bootstrapper.Bootstrap(new CommandEngineRuntimeBootstrapRequest
                {
                    ThrowIfNoCommands = true
                }));

        Assert.Contains("At least one command registration is required", exception.Message);
    }

    [Fact]
    public async Task Builder_AddCommand_BuildsExecutableRuntime()
    {
        CommandEngineRuntimeBootstrapResult result =
            new CommandEngineRuntimeBuilder()
                .AddCommand(new RuntimeCommandRegistration
                {
                    Handler = new TestCommandHandler("extension.run"),
                    DisplayName = "Run Extension",
                    Category = "Extension"
                })
                .BuildRequired();

        CommandExecutionResult executionResult =
            await result.Runtime.ExecuteCommandAsync(new CommandExecutionRequest
            {
                CommandName = "extension.run",
                CorrelationId = "correlation-001"
            });

        Assert.Equal(CommandExecutionStatus.Succeeded, executionResult.Status);
    }

    [Fact]
    public void Builder_BuildRequired_WhenNoCommands_Throws()
    {
        CommandEngineRuntimeBuilder builder = new();

        Assert.Throws<InvalidOperationException>(
            () => builder.BuildRequired());
    }

    private sealed class TestCommandHandler : ICommandHandler
    {
        public TestCommandHandler(string commandName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(commandName);

            CommandName = commandName;
        }

        public string CommandName { get; }

        public Task<CommandExecutionResult> ExecuteAsync(
            CommandExecutionRequest request,
            ICommandExecutionContext context)
        {
            return Task.FromResult(
                CommandExecutionResult.Succeeded(
                    request.CommandName,
                    request.CorrelationId,
                    "Executed.",
                    "{\"ok\":true}"));
        }
    }
}
