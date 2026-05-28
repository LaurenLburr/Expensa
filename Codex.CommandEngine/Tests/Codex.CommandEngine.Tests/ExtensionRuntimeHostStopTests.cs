using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class ExtensionRuntimeHostStopTests
{
    [Fact]
    public void Stop_AfterStart_ClearsRuntimeAndSetsStoppedState()
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

        Assert.Equal(ExtensionRuntimeHostState.Started, host.State);
        Assert.Single(host.ListCommands());

        host.Stop();

        ExtensionRuntimeHostSnapshot snapshot =
            host.GetSnapshot();

        Assert.Equal(ExtensionRuntimeHostState.Stopped, host.State);
        Assert.Equal(ExtensionRuntimeHostState.Stopped, snapshot.State);
        Assert.Equal(0, snapshot.RegisteredCommandCount);
        Assert.Empty(snapshot.Commands);
    }

    [Fact]
    public async Task ExecuteCommandAsync_AfterStop_Throws()
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

        host.Stop();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => host.ExecuteCommandAsync(new ExtensionRuntimeCommandRequest
            {
                CommandName = "command.one"
            }));
    }

    [Fact]
    public void Stop_BeforeStart_IsAllowed()
    {
        ExtensionRuntimeHost host = new();

        host.Stop();

        ExtensionRuntimeHostSnapshot snapshot =
            host.GetSnapshot();

        Assert.Equal(ExtensionRuntimeHostState.Stopped, host.State);
        Assert.Equal(0, snapshot.RegisteredCommandCount);
    }

    [Fact]
    public void Reload_AfterStop_StartsNewRuntime()
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

        host.Stop();

        host.Reload(new ExtensionCommandRuntimeBootstrapRequest
        {
            ThrowIfNoCommands = true,
            Providers =
            [
                CreateProvider("command.two")
            ]
        });

        Assert.Equal(ExtensionRuntimeHostState.Started, host.State);
        Assert.Single(host.ListCommands());
        Assert.Equal("command.two", host.ListCommands()[0].CommandName);
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
                    "Executed.",
                    "{\"ok\":true}"));
        }
    }
}
