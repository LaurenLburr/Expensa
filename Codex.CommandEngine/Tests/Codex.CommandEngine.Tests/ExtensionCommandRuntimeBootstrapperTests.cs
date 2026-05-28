using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class ExtensionCommandRuntimeBootstrapperTests
{
    [Fact]
    public void Bootstrap_WithProvider_ReturnsRuntimeAndRegisteredCommands()
    {
        ExtensionCommandRuntimeBootstrapper bootstrapper = new();

        ExtensionCommandRuntimeBootstrapResult result =
            bootstrapper.Bootstrap(new ExtensionCommandRuntimeBootstrapRequest
            {
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
                ],
                ThrowIfNoCommands = true
            });

        Assert.False(result.HasErrors);
        Assert.Single(result.RegisteredCommands);
        Assert.Equal("extension.open", result.RegisteredCommands[0].CommandName);
        Assert.Single(result.Runtime.ListCommands());
    }

    [Fact]
    public void Bootstrap_WithDuplicateCommandsAndContinueOnError_ReturnsDiagnostics()
    {
        ExtensionCommandRuntimeBootstrapper bootstrapper = new();

        ExtensionCommandRuntimeBootstrapResult result =
            bootstrapper.Bootstrap(new ExtensionCommandRuntimeBootstrapRequest
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
        Assert.Single(result.RegisteredCommands);
        Assert.Single(result.Diagnostics.Issues);
        Assert.Equal("duplicate.command", result.Diagnostics.Issues[0].CommandName);
    }

    [Fact]
    public void Factory_BootstrapFromProviders_ReturnsDiagnostics()
    {
        ExtensionCommandRuntimeBootstrapResult result =
            ExtensionCommandRuntimeFactory.BootstrapFromProviders(
                new ExtensionCommandRuntimeBootstrapRequest
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
        Assert.Single(result.RegisteredCommands);
    }

    [Fact]
    public void Bootstrap_WhenRequiredButNoCommands_Throws()
    {
        ExtensionCommandRuntimeBootstrapper bootstrapper = new();

        Assert.Throws<InvalidOperationException>(
            () => bootstrapper.Bootstrap(new ExtensionCommandRuntimeBootstrapRequest
            {
                ThrowIfNoCommands = true
            }));
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
