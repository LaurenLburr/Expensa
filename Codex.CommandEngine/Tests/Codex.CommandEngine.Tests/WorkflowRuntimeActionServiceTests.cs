using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class WorkflowRuntimeActionServiceTests
{
    [Fact]
    public void Abandon_WhenWorkflowIsResumable_MarksWorkflowAbandoned()
    {
        FakeWorkflowRuntimeOperations operations =
            new(CreateDetail(WorkflowRuntimeOperationStatus.Resumable));

        WorkflowRuntimeActionService service =
            new(operations);

        WorkflowRuntimeActionResult result =
            service.Abandon("workflow-001", "Stop this one.");

        Assert.True(result.Succeeded);
        Assert.True(operations.MarkAbandonedCalled);
        Assert.Equal("workflow-001", operations.LastAbandonedWorkflowExecutionId);
        Assert.Equal("Stop this one.", operations.LastAbandonedMessage);
    }

    [Fact]
    public void Abandon_WhenWorkflowIsCompleted_ReturnsFailure()
    {
        FakeWorkflowRuntimeOperations operations =
            new(CreateDetail(WorkflowRuntimeOperationStatus.Completed));

        WorkflowRuntimeActionService service =
            new(operations);

        WorkflowRuntimeActionResult result =
            service.Abandon("workflow-001", "Stop this one.");

        Assert.False(result.Succeeded);
        Assert.False(operations.MarkAbandonedCalled);
        Assert.Contains("Completed workflows cannot be abandoned", result.Message);
    }

    [Fact]
    public void Heartbeat_WhenWorkflowIsRunning_UpdatesHeartbeat()
    {
        FakeWorkflowRuntimeOperations operations =
            new(CreateDetail(WorkflowRuntimeOperationStatus.Running));

        WorkflowRuntimeActionService service =
            new(operations);

        DateTimeOffset heartbeat =
            new(2035, 1, 2, 3, 4, 5, TimeSpan.Zero);

        WorkflowRuntimeActionResult result =
            service.Heartbeat("workflow-001", heartbeat);

        Assert.True(result.Succeeded);
        Assert.True(operations.HeartbeatCalled);
        Assert.Equal("workflow-001", operations.LastHeartbeatWorkflowExecutionId);
        Assert.Equal(heartbeat, operations.LastHeartbeatUtc);
    }

    [Fact]
    public void Heartbeat_WhenWorkflowIsAbandoned_ReturnsFailure()
    {
        FakeWorkflowRuntimeOperations operations =
            new(CreateDetail(WorkflowRuntimeOperationStatus.Abandoned));

        WorkflowRuntimeActionService service =
            new(operations);

        WorkflowRuntimeActionResult result =
            service.Heartbeat("workflow-001", DateTimeOffset.UtcNow);

        Assert.False(result.Succeeded);
        Assert.False(operations.HeartbeatCalled);
        Assert.Contains("Heartbeat cannot be updated", result.Message);
    }

    private static WorkflowRuntimeDetail CreateDetail(
        WorkflowRuntimeOperationStatus operationStatus)
    {
        return new WorkflowRuntimeDetail
        {
            Summary = new WorkflowRuntimeSummary
            {
                WorkflowExecutionId = "workflow-001",
                WorkflowName = "test.workflow",
                CorrelationId = "correlation-001",
                Status = operationStatus.ToString(),
                OperationStatus = operationStatus,
                CurrentStepOrder = 1,
                LastCompletedStepOrder = 0,
                IsResumable = operationStatus == WorkflowRuntimeOperationStatus.Resumable,
                LastHeartbeatUtc = DateTimeOffset.UtcNow.ToString("O"),
                RuntimeStateJson = "{}"
            },
            Steps = []
        };
    }

    private sealed class FakeWorkflowRuntimeOperations : IWorkflowRuntimeOperations
    {
        private readonly WorkflowRuntimeDetail _detail;

        public FakeWorkflowRuntimeOperations(WorkflowRuntimeDetail detail)
        {
            _detail = detail;
        }

        public bool MarkAbandonedCalled { get; private set; }

        public string? LastAbandonedWorkflowExecutionId { get; private set; }

        public string? LastAbandonedMessage { get; private set; }

        public bool HeartbeatCalled { get; private set; }

        public string? LastHeartbeatWorkflowExecutionId { get; private set; }

        public DateTimeOffset? LastHeartbeatUtc { get; private set; }

        public IReadOnlyList<WorkflowRuntimeSummary> ListResumable()
        {
            return [_detail.Summary];
        }

        public IReadOnlyList<WorkflowRuntimeSummary> ListIncomplete()
        {
            return [_detail.Summary];
        }

        public WorkflowRuntimeDetail GetDetail(string workflowExecutionId)
        {
            return _detail;
        }

        public void MarkAbandoned(string workflowExecutionId, string message)
        {
            MarkAbandonedCalled = true;
            LastAbandonedWorkflowExecutionId = workflowExecutionId;
            LastAbandonedMessage = message;
        }

        public void Heartbeat(string workflowExecutionId, DateTimeOffset heartbeatUtc)
        {
            HeartbeatCalled = true;
            LastHeartbeatWorkflowExecutionId = workflowExecutionId;
            LastHeartbeatUtc = heartbeatUtc;
        }
    }
}
