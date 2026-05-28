using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class ExtensionRuntimeHostTests
{
    [Fact]
    public void GetSnapshot_BeforeStart_ReturnsNotStartedSnapshot()
    {
        ExtensionRuntimeHost host = new();

        ExtensionRuntimeHostSnapshot snapshot =
            host.GetSnapshot();

        Assert.Equal(ExtensionRuntimeHostState.NotStarted, snapshot.State);
        Assert.Equal(0, snapshot.RegisteredCommandCount);
        Assert.Empty(snapshot.Commands);
        Assert.False(snapshot.Diagnostics.HasErrors);
    }

    [Fact]
    public void Start_WithProvider_StartsRuntimeAndReturnsSnapshot()
    {
        ExtensionRuntimeHost host = new();

        host.Start(new ExtensionCommandRuntimeBootstrapRequest
        {
            ThrowIfNoCommands = true,
            Providers =
            [
                new StaticRuntimeCommandRegistrationProvider(
                [
                    new RuntimeCommandRegistration
                    {
                        Handler = new TestCommandHandler("extension.open"),
                        DisplayName = "Open Extension",
                        Category = "Extension"
                    }
                ])
            ]
        });

        ExtensionRuntimeHostSnapshot snapshot =
            host.GetSnapshot();

        Assert.Equal(ExtensionRuntimeHostState.Started, snapshot.State);
        Assert.Equal(1, snapshot.RegisteredCommandCount);
        Assert.Single(snapshot.Commands);
        Assert.Equal("extension.open", snapshot.Commands[0].CommandName);
    }

    [Fact]
    public async Task ExecuteCommandAsync_AfterStart_ExecutesCommand()
    {
        ExtensionRuntimeHost host = new();

        host.Start(new ExtensionCommandRuntimeBootstrapRequest
        {
            ThrowIfNoCommands = true,
            Providers =
            [
                new StaticRuntimeCommandRegistrationProvider(
                [
                    new RuntimeCommandRegistration
                    {
                        Handler = new TestCommandHandler("extension.run"),
                        DisplayName = "Run Extension",
                        Category = "Extension"
                    }
                ])
            ]
        });

        CommandExecutionResult result =
            await host.ExecuteCommandAsync(new ExtensionRuntimeCommandRequest
            {
                CommandName = "extension.run",
                CorrelationId = "correlation-001"
            });

        Assert.Equal(CommandExecutionStatus.Succeeded, result.Status);
        Assert.Equal("correlation-001", result.CorrelationId);
    }

    [Fact]
    public async Task ExecuteCommandAsync_BeforeStart_Throws()
    {
        ExtensionRuntimeHost host = new();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => host.ExecuteCommandAsync(new ExtensionRuntimeCommandRequest
            {
                CommandName = "extension.run"
            }));
    }

    [Fact]
    public void Start_WithDiagnosticErrors_SetsFailedState()
    {
        ExtensionRuntimeHost host = new();

        ExtensionCommandRuntimeBootstrapResult result =
            host.Start(new ExtensionCommandRuntimeBootstrapRequest
            {
                ContinueOnRegistrationError = true,
                Providers =
                [
                    new StaticRuntimeCommandRegistrationProvider(
                    [
                        new RuntimeCommandRegistration
                        {
                            Handler = new TestCommandHandler("duplicate.command")
                        },
                        new RuntimeCommandRegistration
                        {
                            Handler = new TestCommandHandler("duplicate.command")
                        }
                    ])
                ]
            });

        Assert.True(result.HasErrors);
        Assert.Equal(ExtensionRuntimeHostState.Failed, host.State);
        Assert.Single(host.GetSnapshot().Commands);
    }

    [Fact]
    public void FormatSnapshot_BeforeStart_ReturnsReadableText()
    {
        ExtensionRuntimeHost host = new();

        string text =
            ExtensionRuntimeHostSnapshotTextFormatter.Format(
                host.GetSnapshot());

        Assert.Contains("Extension Runtime Host", text);
        Assert.Contains("State: NotStarted", text);
        Assert.Contains("No commands or diagnostics are available.", text);
    }

    [Fact]
    public void FormatSnapshot_AfterStart_ReturnsCommandDetails()
    {
        ExtensionRuntimeHost host = new();

        host.Start(new ExtensionCommandRuntimeBootstrapRequest
        {
            ThrowIfNoCommands = true,
            Providers =
            [
                new StaticRuntimeCommandRegistrationProvider(
                [
                    new RuntimeCommandRegistration
                    {
                        Handler = new TestCommandHandler("extension.open"),
                        DisplayName = "Open Extension",
                        Category = "Extension"
                    }
                ])
            ]
        });

        string text =
            ExtensionRuntimeHostSnapshotTextFormatter.Format(
                host.GetSnapshot());

        Assert.Contains("State: Started", text);
        Assert.Contains("Registered commands: 1", text);
        Assert.Contains("Command: extension.open", text);
        Assert.Contains("Display Name: Open Extension", text);
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
