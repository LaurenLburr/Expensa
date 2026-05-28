using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class WorkflowResumeServiceTests
{
    [Fact]
    public async Task ResumeAsync_LoadsRequestAndRunsResumeRunner()
    {
        RecordingLoader loader = new();
        RecordingResumeRunner runner = new();

        WorkflowResumeService service =
            new(loader, runner);

        WorkflowExecutionResult result =
            await service.ResumeAsync(new WorkflowResumeServiceRequest
            {
                WorkflowExecutionId = "workflow-execution-001",
                WorkflowDefinitionSteps =
                [
                    new WorkflowStepExecutionRequest
                    {
                        StepName = "Second",
                        CommandName = "second.command",
                        StepOrder = 2
                    }
                ]
            });

        Assert.Equal(WorkflowExecutionStatus.Succeeded, result.Status);
        Assert.Equal("workflow-execution-001", loader.LastWorkflowExecutionId);
        Assert.NotNull(runner.LastRequest);
        Assert.Equal("resume.workflow", runner.LastRequest!.ResumeRequest.WorkflowName);
    }

    [Fact]
    public async Task ResumeAsync_WhenCancellationRequested_PassesCancellationToken()
    {
        RecordingLoader loader = new();
        RecordingResumeRunner runner = new();

        WorkflowResumeService service =
            new(loader, runner);

        using CancellationTokenSource source = new();
        source.Cancel();

        WorkflowExecutionResult result =
            await service.ResumeAsync(
                new WorkflowResumeServiceRequest
                {
                    WorkflowExecutionId = "workflow-execution-002"
                },
                source.Token);

        Assert.Equal(WorkflowExecutionStatus.Cancelled, result.Status);
        Assert.True(runner.CancellationWasRequested);
    }

    private sealed class RecordingLoader : IWorkflowResumeRequestLoader
    {
        public string? LastWorkflowExecutionId { get; private set; }

        public IReadOnlyList<WorkflowStepExecutionRequest>? LastSteps { get; private set; }

        public WorkflowResumeExecutionRequest Load(
            string workflowExecutionId,
            IReadOnlyList<WorkflowStepExecutionRequest> workflowDefinitionSteps)
        {
            LastWorkflowExecutionId = workflowExecutionId;
            LastSteps = workflowDefinitionSteps;

            return new WorkflowResumeExecutionRequest
            {
                ContextJson = "{\"loaded\":true}",
                ResumeRequest = new WorkflowResumeRequest
                {
                    WorkflowExecutionId = workflowExecutionId,
                    WorkflowName = "resume.workflow",
                    CorrelationId = "correlation-001",
                    LastCompletedStepOrder = 1,
                    Steps = workflowDefinitionSteps
                }
            };
        }
    }

    private sealed class RecordingResumeRunner : IWorkflowResumeRunner
    {
        public WorkflowResumeExecutionRequest? LastRequest { get; private set; }

        public bool CancellationWasRequested { get; private set; }

        public Task<WorkflowExecutionResult> ResumeAsync(
            WorkflowResumeExecutionRequest request,
            CancellationToken cancellationToken = default)
        {
            LastRequest = request;
            CancellationWasRequested = cancellationToken.IsCancellationRequested;

            return Task.FromResult(new WorkflowExecutionResult
            {
                WorkflowName = request.ResumeRequest.WorkflowName,
                CorrelationId = request.ResumeRequest.CorrelationId,
                Status = cancellationToken.IsCancellationRequested
                    ? WorkflowExecutionStatus.Cancelled
                    : WorkflowExecutionStatus.Succeeded,
                Message = cancellationToken.IsCancellationRequested
                    ? "Cancelled."
                    : "Resumed."
            });
        }
    }
}
