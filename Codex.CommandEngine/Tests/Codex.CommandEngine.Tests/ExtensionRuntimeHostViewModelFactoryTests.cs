using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class ExtensionRuntimeHostViewModelFactoryTests
{
    [Fact]
    public void Create_WhenHostNotStarted_ReturnsNotStartedViewModel()
    {
        ExtensionRuntimeHost host = new();

        ExtensionRuntimeHostViewModel viewModel =
            ExtensionRuntimeHostViewModelFactory.Create(
                host.GetSnapshot());

        Assert.Equal(ExtensionRuntimeHostState.NotStarted, viewModel.State);
        Assert.Equal("NotStarted", viewModel.StateText);
        Assert.Equal(0, viewModel.RegisteredCommandCount);
        Assert.Equal("Runtime has not been started.", viewModel.Summary);
    }

    [Fact]
    public void Create_WhenHostStarted_ReturnsCommandsInCategoryThenNameOrder()
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
                        Handler = new TestCommandHandler("zeta.command"),
                        DisplayName = "Zeta",
                        Category = "B"
                    },
                    new RuntimeCommandRegistration
                    {
                        Handler = new TestCommandHandler("alpha.command"),
                        DisplayName = "Alpha",
                        Category = "A"
                    }
                ])
            ]
        });

        ExtensionRuntimeHostViewModel viewModel =
            ExtensionRuntimeHostViewModelFactory.Create(
                host.GetSnapshot());

        Assert.Equal(ExtensionRuntimeHostState.Started, viewModel.State);
        Assert.Equal(2, viewModel.RegisteredCommandCount);
        Assert.Equal("Runtime started with 2 command(s).", viewModel.Summary);
        Assert.Equal("alpha.command", viewModel.Commands[0].CommandName);
        Assert.Equal("zeta.command", viewModel.Commands[1].CommandName);
    }

    [Fact]
    public void Create_WhenHostHasDiagnostics_ReturnsDiagnosticRows()
    {
        ExtensionRuntimeHost host = new();

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

        ExtensionRuntimeHostViewModel viewModel =
            ExtensionRuntimeHostViewModelFactory.Create(
                host.GetSnapshot());

        Assert.Equal(ExtensionRuntimeHostState.Failed, viewModel.State);
        Assert.True(viewModel.HasErrors);
        Assert.Equal(1, viewModel.DiagnosticCount);
        Assert.Single(viewModel.Diagnostics);
        Assert.Equal("duplicate.command", viewModel.Diagnostics[0].CommandName);
        Assert.Contains("Runtime failed with 1 diagnostic issue", viewModel.Summary);
    }

    [Fact]
    public void Create_WhenHostStopped_ReturnsStoppedSummary()
    {
        ExtensionRuntimeHost host = new();

        host.Stop();

        ExtensionRuntimeHostViewModel viewModel =
            ExtensionRuntimeHostViewModelFactory.Create(
                host.GetSnapshot());

        Assert.Equal(ExtensionRuntimeHostState.Stopped, viewModel.State);
        Assert.Equal("Runtime is stopped.", viewModel.Summary);
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
