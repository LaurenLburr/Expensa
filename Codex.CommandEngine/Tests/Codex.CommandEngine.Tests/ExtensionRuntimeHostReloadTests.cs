using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class ExtensionRuntimeHostReloadTests
{
    [Fact]
    public void Reload_ReplacesRegisteredCommands()
    {
        ExtensionRuntimeHost host = new();

        host.Start(new ExtensionCommandRuntimeBootstrapRequest
        {
            ThrowIfNoCommands = true,
            Providers =
            [
                CreateProvider("command.one")
            ]
        });

        Assert.Single(host.ListCommands());
        Assert.Equal("command.one", host.ListCommands()[0].CommandName);

        host.Reload(new ExtensionCommandRuntimeBootstrapRequest
        {
            ThrowIfNoCommands = true,
            Providers =
            [
                CreateProvider("command.two")
            ]
        });

        IReadOnlyList<RuntimeCommandDescriptor> commands =
            host.ListCommands();

        Assert.Single(commands);
        Assert.Equal("command.two", commands[0].CommandName);
    }

    [Fact]
    public async Task Reload_AfterSuccessfulStart_UsesNewRuntime()
    {
        ExtensionRuntimeHost host = new();

        host.Start(new ExtensionCommandRuntimeBootstrapRequest
        {
            ThrowIfNoCommands = true,
            Providers =
            [
                CreateProvider("command.one")
            ]
        });

        host.Reload(new ExtensionCommandRuntimeBootstrapRequest
        {
            ThrowIfNoCommands = true,
            Providers =
            [
                CreateProvider("command.two")
            ]
        });

        CommandExecutionResult result =
            await host.ExecuteCommandAsync(new ExtensionRuntimeCommandRequest
            {
                CommandName = "command.two",
                CorrelationId = "reload-001"
            });

        Assert.Equal(CommandExecutionStatus.Succeeded, result.Status);
        Assert.Equal("command.two", result.CommandName);
    }

    [Fact]
    public void Reload_WhenBootstrapFails_SetsFailedState()
    {
        ExtensionRuntimeHost host = new();

        host.Start(new ExtensionCommandRuntimeBootstrapRequest
        {
            ThrowIfNoCommands = true,
            Providers =
            [
                CreateProvider("command.one")
            ]
        });

        Assert.Throws<InvalidOperationException>(
            () => host.Reload(new ExtensionCommandRuntimeBootstrapRequest
            {
                ThrowIfNoCommands = true
            }));

        Assert.Equal(ExtensionRuntimeHostState.Failed, host.State);
    }

    private static IRuntimeCommandRegistrationProvider CreateProvider(
        string commandName)
    {
        return new StaticRuntimeCommandRegistrationProvider(
        [
            new RuntimeCommandRegistration
            {
                Handler = new TestCommandHandler(commandName),
                DisplayName = commandName,
                Category = "Tests"
            }
        ]);
    }

    private sealed class TestCommandHandler : ICommandHandler
    {
        public TestCommandHandler(string commandName)
        {
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
                    "Reload executed.",
                    "{\"reload\":true}"));
        }
    }
}
