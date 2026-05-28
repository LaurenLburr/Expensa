using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class CommandEngineRuntimeTests
{
    [Fact]
    public void ListRegisteredCommandNames_AfterRegister_ReturnsRegisteredCommandsInNameOrder()
    {
        CommandEngineRuntime runtime = new();

        runtime.RegisterCommand(new TestCommandHandler("zeta.command"));
        runtime.RegisterCommand(new TestCommandHandler("alpha.command"));

        IReadOnlyList<string> commandNames =
            runtime.ListRegisteredCommandNames();

        Assert.Equal(["alpha.command", "zeta.command"], commandNames);
    }

    [Fact]
    public async Task ExecuteCommandAsync_WhenCommandRegistered_ReturnsCommandResult()
    {
        CommandEngineRuntime runtime = new();
        runtime.RegisterCommand(new TestCommandHandler("test.command"));

        CommandExecutionResult result =
            await runtime.ExecuteCommandAsync(new CommandExecutionRequest
            {
                CommandName = "test.command",
                CorrelationId = "correlation-001"
            });

        Assert.Equal(CommandExecutionStatus.Succeeded, result.Status);
        Assert.Equal("test.command", result.CommandName);
        Assert.Equal("correlation-001", result.CorrelationId);
    }

    [Fact]
    public async Task ExecuteWorkflowAsync_WhenCommandsRegistered_RunsWorkflow()
    {
        CommandEngineRuntime runtime = new();
        runtime.RegisterCommand(new TestCommandHandler("first.command"));
        runtime.RegisterCommand(new TestCommandHandler("second.command"));

        WorkflowExecutionResult result =
            await runtime.ExecuteWorkflowAsync(new WorkflowExecutionRequest
            {
                WorkflowName = "test.workflow",
                CorrelationId = "correlation-001",
                Steps =
                [
                    new WorkflowStepExecutionRequest
                    {
                        StepName = "First",
                        StepOrder = 1,
                        CommandName = "first.command"
                    },
                    new WorkflowStepExecutionRequest
                    {
                        StepName = "Second",
                        StepOrder = 2,
                        CommandName = "second.command"
                    }
                ]
            });

        Assert.Equal(WorkflowExecutionStatus.Succeeded, result.Status);
        Assert.Equal("test.workflow", result.WorkflowName);
        Assert.Equal("correlation-001", result.CorrelationId);
    }

    [Fact]
    public async Task ExecuteWorkflowAsync_WhenCommandMissing_ReturnsFailedWorkflow()
    {
        CommandEngineRuntime runtime = new();

        WorkflowExecutionResult result =
            await runtime.ExecuteWorkflowAsync(new WorkflowExecutionRequest
            {
                WorkflowName = "missing.workflow",
                CorrelationId = "correlation-001",
                Steps =
                [
                    new WorkflowStepExecutionRequest
                    {
                        StepName = "Missing",
                        StepOrder = 1,
                        CommandName = "missing.command"
                    }
                ]
            });

        Assert.Equal(WorkflowExecutionStatus.Failed, result.Status);
        Assert.Contains("Missing", result.Message);
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
