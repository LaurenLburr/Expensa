using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class WorkflowResumeRunnerTests
{
    [Fact]
    public async Task ResumeAsync_WhenPendingStepsExist_ExecutesRemainingSteps()
    {
        RecordingWorkflowRunner innerRunner = new();

        WorkflowResumeRunner runner =
            new(
                new WorkflowResumePlanner(),
                innerRunner);

        WorkflowExecutionResult result =
            await runner.ResumeAsync(new WorkflowResumeExecutionRequest
            {
                ContextJson = "{\"resume\":true}",
                ResumeRequest = new WorkflowResumeRequest
                {
                    WorkflowExecutionId = "workflow-execution-001",
                    WorkflowName = "resume.workflow",
                    CorrelationId = "correlation-001",
                    LastCompletedStepOrder = 1,
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
                            StepName = "Second",
                            CommandName = "second.command",
                            StepOrder = 2
                        },
                        new WorkflowStepExecutionRequest
                        {
                            StepName = "Third",
                            CommandName = "third.command",
                            StepOrder = 3
                        }
                    ]
                }
            });

        Assert.Equal(WorkflowExecutionStatus.Succeeded, result.Status);
        Assert.NotNull(innerRunner.LastRequest);
        Assert.Equal("resume.workflow", innerRunner.LastRequest!.WorkflowName);
        Assert.Equal("correlation-001", innerRunner.LastRequest.CorrelationId);
        Assert.Equal("{\"resume\":true}", innerRunner.LastRequest.ContextJson);
        Assert.Equal(2, innerRunner.LastRequest.Steps.Count);
        Assert.Equal("Second", innerRunner.LastRequest.Steps[0].StepName);
        Assert.Equal("Third", innerRunner.LastRequest.Steps[1].StepName);
    }

    [Fact]
    public async Task ResumeAsync_WhenNoPendingSteps_ReturnsSucceededWithoutExecutingInnerRunner()
    {
        RecordingWorkflowRunner innerRunner = new();

        WorkflowResumeRunner runner =
            new(
                new WorkflowResumePlanner(),
                innerRunner);

        WorkflowExecutionResult result =
            await runner.ResumeAsync(new WorkflowResumeExecutionRequest
            {
                ResumeRequest = new WorkflowResumeRequest
                {
                    WorkflowExecutionId = "workflow-execution-002",
                    WorkflowName = "complete.workflow",
                    CorrelationId = "correlation-002",
                    LastCompletedStepOrder = 2,
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
                            StepName = "Second",
                            CommandName = "second.command",
                            StepOrder = 2
                        }
                    ]
                }
            });

        Assert.Equal(WorkflowExecutionStatus.Succeeded, result.Status);
        Assert.Null(innerRunner.LastRequest);
        Assert.Equal("Workflow has no pending steps to resume.", result.Message);
    }

    private sealed class RecordingWorkflowRunner : IWorkflowRunner
    {
        public WorkflowExecutionRequest? LastRequest { get; private set; }

        public Task<WorkflowExecutionResult> ExecuteAsync(
            WorkflowExecutionRequest request,
            CancellationToken cancellationToken = default)
        {
            LastRequest = request;

            return Task.FromResult(new WorkflowExecutionResult
            {
                WorkflowName = request.WorkflowName,
                CorrelationId = request.CorrelationId,
                Status = WorkflowExecutionStatus.Succeeded,
                Message = "Resumed workflow completed."
            });
        }
    }
}
