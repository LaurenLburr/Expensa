using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class SequentialWorkflowRunnerTests
{
    [Fact]
    public async Task ExecuteAsync_WhenAllStepsSucceed_ReturnsSucceeded()
    {
        CommandDispatcher dispatcher = new();
        dispatcher.Register(new SuccessHandler("first.command"));
        dispatcher.Register(new SuccessHandler("second.command"));

        SequentialWorkflowRunner runner = new(dispatcher);

        WorkflowExecutionResult result =
            await runner.ExecuteAsync(new WorkflowExecutionRequest
            {
                WorkflowName = "sample.workflow",
                CorrelationId = "correlation-001",
                Steps =
                [
                    new WorkflowStepExecutionRequest
                    {
                        StepName = "Second",
                        CommandName = "second.command",
                        StepOrder = 2
                    },
                    new WorkflowStepExecutionRequest
                    {
                        StepName = "First",
                        CommandName = "first.command",
                        StepOrder = 1
                    }
                ]
            });

        Assert.Equal(WorkflowExecutionStatus.Succeeded, result.Status);
        Assert.Equal(2, result.StepResults.Count);
        Assert.Equal("First", result.StepResults[0].StepName);
        Assert.Equal("Second", result.StepResults[1].StepName);
    }

    [Fact]
    public async Task ExecuteAsync_WhenStepFails_StopsWorkflow()
    {
        CommandDispatcher dispatcher = new();
        dispatcher.Register(new SuccessHandler("first.command"));
        dispatcher.Register(new FailureHandler("fail.command"));
        dispatcher.Register(new SuccessHandler("never.command"));

        SequentialWorkflowRunner runner = new(dispatcher);

        WorkflowExecutionResult result =
            await runner.ExecuteAsync(new WorkflowExecutionRequest
            {
                WorkflowName = "sample.workflow",
                CorrelationId = "correlation-002",
                Steps =
                [
                    new WorkflowStepExecutionRequest
                    {
                        StepName = "First",
                        CommandName = "first.command",
                        StepOrder = 1
                    },
                    new WorkflowStepExecutionRequest
                    {
                        StepName = "Fail",
                        CommandName = "fail.command",
                        StepOrder = 2
                    },
                    new WorkflowStepExecutionRequest
                    {
                        StepName = "Never",
                        CommandName = "never.command",
                        StepOrder = 3
                    }
                ]
            });

        Assert.Equal(WorkflowExecutionStatus.Failed, result.Status);
        Assert.Equal(2, result.StepResults.Count);
        Assert.Equal("Fail", result.StepResults[1].StepName);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCancelledBeforeStart_ReturnsCancelled()
    {
        CommandDispatcher dispatcher = new();
        SequentialWorkflowRunner runner = new(dispatcher);

        using CancellationTokenSource source = new();
        source.Cancel();

        WorkflowExecutionResult result =
            await runner.ExecuteAsync(
                new WorkflowExecutionRequest
                {
                    WorkflowName = "cancelled.workflow",
                    CorrelationId = "correlation-003"
                },
                source.Token);

        Assert.Equal(WorkflowExecutionStatus.Cancelled, result.Status);
    }

    private sealed class SuccessHandler : ICommandHandler
    {
        public SuccessHandler(string commandName)
        {
            CommandName = commandName;
        }

        public string CommandName { get; }

        public Task<CommandExecutionResult> ExecuteAsync(
            CommandExecutionRequest request,
            ICommandExecutionContext context)
        {
            return Task.FromResult(CommandExecutionResult.Succeeded(
                request.CommandName,
                request.CorrelationId,
                "OK"));
        }
    }

    private sealed class FailureHandler : ICommandHandler
    {
        public FailureHandler(string commandName)
        {
            CommandName = commandName;
        }

        public string CommandName { get; }

        public Task<CommandExecutionResult> ExecuteAsync(
            CommandExecutionRequest request,
            ICommandExecutionContext context)
        {
            return Task.FromResult(CommandExecutionResult.Failed(
                request.CommandName,
                request.CorrelationId,
                "Failed."));
        }
    }
}
