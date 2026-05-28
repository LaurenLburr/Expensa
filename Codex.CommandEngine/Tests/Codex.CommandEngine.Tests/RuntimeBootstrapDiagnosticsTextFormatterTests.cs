using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class RuntimeBootstrapDiagnosticsTextFormatterTests
{
    [Fact]
    public void CreateViewModel_FromRuntimeBootstrapResult_MapsCountsAndIssues()
    {
        CommandEngineRuntimeBootstrapResult result =
            new CommandEngineRuntimeBootstrapper()
                .Bootstrap(new CommandEngineRuntimeBootstrapRequest
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

        RuntimeBootstrapDiagnosticViewModel viewModel =
            RuntimeBootstrapDiagnosticViewModelFactory.Create(result);

        Assert.Equal(1, viewModel.RegisteredCommandCount);
        Assert.Equal(1, viewModel.IssueCount);
        Assert.True(viewModel.HasErrors);
        Assert.Equal("duplicate.command", viewModel.Lines[0].CommandName);
    }

    [Fact]
    public void Format_WhenNoIssues_ReturnsCleanSummary()
    {
        CommandEngineRuntimeBootstrapResult result =
            new CommandEngineRuntimeBootstrapper()
                .Bootstrap(new CommandEngineRuntimeBootstrapRequest
                {
                    Commands =
                    [
                        new RuntimeCommandRegistration
                        {
                            Handler = new TestCommandHandler("clean.command")
                        }
                    ]
                });

        string text =
            RuntimeBootstrapDiagnosticsTextFormatter.Format(result);

        Assert.Contains("Registered commands: 1", text);
        Assert.Contains("Issues: 0", text);
        Assert.Contains("No bootstrap issues found.", text);
    }

    [Fact]
    public void Format_WhenIssuesExist_ReturnsIssueDetails()
    {
        CommandEngineRuntimeBootstrapResult result =
            new CommandEngineRuntimeBootstrapper()
                .Bootstrap(new CommandEngineRuntimeBootstrapRequest
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

        string text =
            RuntimeBootstrapDiagnosticsTextFormatter.Format(result);

        Assert.Contains("Registered commands: 1", text);
        Assert.Contains("Issues: 1", text);
        Assert.Contains("Severity: Error", text);
        Assert.Contains("Command: duplicate.command", text);
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
