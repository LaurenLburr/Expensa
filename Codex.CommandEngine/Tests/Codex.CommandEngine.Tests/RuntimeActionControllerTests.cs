using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class RuntimeActionControllerTests
{
    [Fact]
    public void AbandonWorkflow_WhenActionSucceeds_ReturnsDisplayAndRefreshText()
    {
        FakeRuntimeOperations operations =
            new(CreateDetail(WorkflowRuntimeOperationStatus.Resumable, "Started"));

        RuntimeActionController controller =
            CreateController(
                new FakeActionService(true, "Workflow was abandoned."),
                operations);

        RuntimeActionControllerResult result =
            controller.AbandonWorkflow(
                "workflow-001",
                "Operator request.",
                new DateTimeOffset(2035, 1, 2, 3, 4, 5, TimeSpan.Zero));

        Assert.True(result.Succeeded);
        Assert.Equal("Abandon Workflow", result.ActionName);
        Assert.Contains("Workflow Runtime Action", result.DisplayText);
        Assert.Contains("Workflow Runtime Detail", result.RefreshText);
        Assert.Equal("workflow-001", operations.LastDetailWorkflowExecutionId);
    }

    [Fact]
    public void UpdateHeartbeat_WhenActionFails_ReturnsFailureDisplayText()
    {
        FakeRuntimeOperations operations =
            new(CreateDetail(WorkflowRuntimeOperationStatus.Running, "Started"));

        RuntimeActionController controller =
            CreateController(
                new FakeActionService(false, "Heartbeat failed."),
                operations);

        RuntimeActionControllerResult result =
            controller.UpdateHeartbeat(
                "workflow-001",
                new DateTimeOffset(2035, 1, 2, 3, 4, 5, TimeSpan.Zero),
                new DateTimeOffset(2035, 1, 2, 3, 4, 6, TimeSpan.Zero));

        Assert.False(result.Succeeded);
        Assert.Equal("Update Workflow Heartbeat", result.ActionName);
        Assert.Contains("Status: Failed", result.DisplayText);
        Assert.Contains("Heartbeat failed.", result.DisplayText);
    }

    [Fact]
    public void AbandonWorkflow_WhenRefreshFails_ReturnsRefreshErrorText()
    {
        RuntimeActionController controller =
            CreateController(
                new FakeActionService(true, "Workflow was abandoned."),
                new ThrowingRuntimeOperations());

        RuntimeActionControllerResult result =
            controller.AbandonWorkflow(
                "workflow-001",
                "Operator request.",
                DateTimeOffset.UtcNow);

        Assert.True(result.Succeeded);
        Assert.Contains("Runtime Refresh Failed", result.RefreshText);
    }

    private static RuntimeActionController CreateController(
        IWorkflowRuntimeActionService actionService,
        IWorkflowRuntimeOperations operations)
    {
        return new RuntimeActionController(
            actionService,
            new WorkflowRuntimeActionPresenter(
                new WorkflowRuntimeActionViewModelFactory()),
            operations,
            new WorkflowHeartbeatEvaluator(),
            TimeSpan.FromMinutes(10));
    }

    private static WorkflowRuntimeDetail CreateDetail(
        WorkflowRuntimeOperationStatus operationStatus,
        string status)
    {
        return new WorkflowRuntimeDetail
        {
            Summary = new WorkflowRuntimeSummary
            {
                WorkflowExecutionId = "workflow-001",
                WorkflowName = "runtime.workflow",
                CorrelationId = "correlation-001",
                Status = status,
                OperationStatus = operationStatus,
                CurrentStepOrder = 1,
                LastCompletedStepOrder = 0,
                IsResumable = operationStatus == WorkflowRuntimeOperationStatus.Resumable,
                LastHeartbeatUtc = DateTimeOffset.UtcNow.ToString("O"),
                RuntimeStateJson = "{}"
            },
            Steps =
            [
                new WorkflowRuntimeStepSummary
                {
                    WorkflowStepExecutionId = "step-001",
                    StepName = "First",
                    StepOrder = 1,
                    CommandName = "first.command",
                    Status = "Started",
                    Message = "Running"
                }
            ]
        };
    }

    private sealed class FakeActionService : IWorkflowRuntimeActionService
    {
        private readonly bool _succeeded;
        private readonly string _message;

        public FakeActionService(bool succeeded, string message)
        {
            _succeeded = succeeded;
            _message = message;
        }

        public WorkflowRuntimeActionResult Abandon(string workflowExecutionId, string message)
        {
            return _succeeded
                ? WorkflowRuntimeActionResult.Success(workflowExecutionId, _message)
                : WorkflowRuntimeActionResult.Failure(workflowExecutionId, _message);
        }

        public WorkflowRuntimeActionResult Heartbeat(string workflowExecutionId, DateTimeOffset heartbeatUtc)
        {
            return _succeeded
                ? WorkflowRuntimeActionResult.Success(workflowExecutionId, _message)
                : WorkflowRuntimeActionResult.Failure(workflowExecutionId, _message);
        }
    }

    private sealed class FakeRuntimeOperations : IWorkflowRuntimeOperations
    {
        private readonly WorkflowRuntimeDetail _detail;

        public FakeRuntimeOperations(WorkflowRuntimeDetail detail)
        {
            _detail = detail;
        }

        public string? LastDetailWorkflowExecutionId { get; private set; }

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
            LastDetailWorkflowExecutionId = workflowExecutionId;
            return _detail;
        }

        public void MarkAbandoned(string workflowExecutionId, string message)
        {
        }

        public void Heartbeat(string workflowExecutionId, DateTimeOffset heartbeatUtc)
        {
        }
    }

    private sealed class ThrowingRuntimeOperations : IWorkflowRuntimeOperations
    {
        public IReadOnlyList<WorkflowRuntimeSummary> ListResumable()
        {
            return [];
        }

        public IReadOnlyList<WorkflowRuntimeSummary> ListIncomplete()
        {
            return [];
        }

        public WorkflowRuntimeDetail GetDetail(string workflowExecutionId)
        {
            throw new InvalidOperationException("Refresh failed.");
        }

        public void MarkAbandoned(string workflowExecutionId, string message)
        {
        }

        public void Heartbeat(string workflowExecutionId, DateTimeOffset heartbeatUtc)
        {
        }
    }
}
