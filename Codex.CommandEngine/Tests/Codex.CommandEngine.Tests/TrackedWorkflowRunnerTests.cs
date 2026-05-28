using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class TrackedWorkflowRunnerTests
{
    [Fact]
    public async Task ExecuteAsync_WhenWorkflowSucceeds_NotifiesHistorySink()
    {
        RecordingWorkflowHistorySink sink = new();

        TrackedWorkflowRunner runner =
            new(
                new FixedWorkflowRunner(WorkflowExecutionStatus.Succeeded),
                sink);

        WorkflowExecutionResult result =
            await runner.ExecuteAsync(new WorkflowExecutionRequest
            {
                WorkflowName = "tracked.workflow",
                CorrelationId = "correlation-001"
            });

        Assert.Equal(WorkflowExecutionStatus.Succeeded, result.Status);
        Assert.True(sink.StartedCalled);
        Assert.True(sink.CompletedCalled);
        Assert.Equal("tracked.workflow", sink.StartedRequest?.WorkflowName);
        Assert.Equal("tracked.workflow", sink.CompletedRequest?.WorkflowName);
        Assert.NotEqual(string.Empty, sink.WorkflowExecutionId);
        Assert.True(sink.DurationMilliseconds >= 0);
    }

    [Fact]
    public async Task ExecuteAsync_WhenInnerThrows_ReturnsFailedAndNotifiesCompleted()
    {
        RecordingWorkflowHistorySink sink = new();

        TrackedWorkflowRunner runner =
            new(
                new ThrowingWorkflowRunner(),
                sink);

        WorkflowExecutionResult result =
            await runner.ExecuteAsync(new WorkflowExecutionRequest
            {
                WorkflowName = "throw.workflow",
                CorrelationId = "correlation-002"
            });

        Assert.Equal(WorkflowExecutionStatus.Failed, result.Status);
        Assert.True(sink.StartedCalled);
        Assert.True(sink.CompletedCalled);
        Assert.Equal("Boom", result.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCancelled_ReturnsCancelledAndNotifiesCompleted()
    {
        RecordingWorkflowHistorySink sink = new();

        TrackedWorkflowRunner runner =
            new(
                new FixedWorkflowRunner(WorkflowExecutionStatus.Succeeded),
                sink);

        using CancellationTokenSource source = new();
        source.Cancel();

        WorkflowExecutionResult result =
            await runner.ExecuteAsync(
                new WorkflowExecutionRequest
                {
                    WorkflowName = "cancel.workflow",
                    CorrelationId = "correlation-003"
                },
                source.Token);

        Assert.Equal(WorkflowExecutionStatus.Succeeded, result.Status);
        Assert.True(sink.StartedCalled);
        Assert.True(sink.CompletedCalled);
    }

    private sealed class RecordingWorkflowHistorySink : IWorkflowExecutionHistorySink
    {
        public bool StartedCalled { get; private set; }

        public bool CompletedCalled { get; private set; }

        public WorkflowExecutionRequest? StartedRequest { get; private set; }

        public WorkflowExecutionRequest? CompletedRequest { get; private set; }

        public string WorkflowExecutionId { get; private set; } = string.Empty;

        public long DurationMilliseconds { get; private set; }

        public void Started(
            WorkflowExecutionRequest request,
            string workflowExecutionId,
            DateTimeOffset startedUtc)
        {
            StartedCalled = true;
            StartedRequest = request;
            WorkflowExecutionId = workflowExecutionId;
        }

        public void Completed(
            WorkflowExecutionRequest request,
            WorkflowExecutionResult result,
            string workflowExecutionId,
            DateTimeOffset completedUtc,
            long durationMilliseconds)
        {
            CompletedCalled = true;
            CompletedRequest = request;
            WorkflowExecutionId = workflowExecutionId;
            DurationMilliseconds = durationMilliseconds;
        }
    }

    private sealed class FixedWorkflowRunner : IWorkflowRunner
    {
        private readonly WorkflowExecutionStatus _status;

        public FixedWorkflowRunner(WorkflowExecutionStatus status)
        {
            _status = status;
        }

        public Task<WorkflowExecutionResult> ExecuteAsync(
            WorkflowExecutionRequest request,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new WorkflowExecutionResult
            {
                WorkflowName = request.WorkflowName,
                CorrelationId = request.CorrelationId,
                Status = _status,
                Message = "Completed."
            });
        }
    }

    private sealed class ThrowingWorkflowRunner : IWorkflowRunner
    {
        public Task<WorkflowExecutionResult> ExecuteAsync(
            WorkflowExecutionRequest request,
            CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Boom");
        }
    }
}
