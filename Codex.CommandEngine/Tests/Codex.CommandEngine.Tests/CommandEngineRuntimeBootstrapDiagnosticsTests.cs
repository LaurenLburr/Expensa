using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class CommandEngineRuntimeBootstrapDiagnosticsTests
{
    [Fact]
    public void Bootstrap_WhenDuplicateCommandAndContinueOnError_ReturnsDiagnostics()
    {
        CommandEngineRuntimeBootstrapper bootstrapper = new();

        CommandEngineRuntimeBootstrapResult result =
            bootstrapper.Bootstrap(new CommandEngineRuntimeBootstrapRequest
            {
                ContinueOnRegistrationError = true,
                Commands =
                [
                    new RuntimeCommandRegistration
                    {
                        Handler = new TestCommandHandler("duplicate.command")
                    },
                    new RuntimeCommandRegistration
                    {
                        Handler = new TestCommandHandler("duplicate.command")
                    }
                ]
            });

        Assert.True(result.HasErrors);
        Assert.Single(result.RegisteredCommands);
        Assert.Single(result.Diagnostics.Issues);
        Assert.Equal(RuntimeBootstrapIssueSeverity.Error, result.Diagnostics.Issues[0].Severity);
        Assert.Equal("duplicate.command", result.Diagnostics.Issues[0].CommandName);
    }

    [Fact]
    public void Bootstrap_WhenDuplicateCommandAndNotContinueOnError_Throws()
    {
        CommandEngineRuntimeBootstrapper bootstrapper = new();

        Assert.Throws<InvalidOperationException>(
            () => bootstrapper.Bootstrap(new CommandEngineRuntimeBootstrapRequest
            {
                Commands =
                [
                    new RuntimeCommandRegistration
                    {
                        Handler = new TestCommandHandler("duplicate.command")
                    },
                    new RuntimeCommandRegistration
                    {
                        Handler = new TestCommandHandler("duplicate.command")
                    }
                ]
            }));
    }

    [Fact]
    public void Builder_ContinueOnRegistrationError_ReturnsDiagnostics()
    {
        CommandEngineRuntimeBootstrapResult result =
            new CommandEngineRuntimeBuilder()
                .ContinueOnRegistrationError()
                .AddCommand(new TestCommandHandler("duplicate.command"))
                .AddCommand(new TestCommandHandler("duplicate.command"))
                .Build();

        Assert.True(result.HasErrors);
        Assert.Single(result.RegisteredCommands);
        Assert.Single(result.Diagnostics.Issues);
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
