using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class WorkflowRuntimeActionCommandServiceTests
{
    [Fact]
    public void AbandonAndPresent_CallsActionServiceAndReturnsFormattedText()
    {
        FakeActionService actionService = new();
        WorkflowRuntimeActionCommandService service =
            new(
                actionService,
                new WorkflowRuntimeActionPresenter(
                    new WorkflowRuntimeActionViewModelFactory()));

        string text =
            service.AbandonAndPresent(
                "workflow-001",
                "Operator request.",
                new DateTimeOffset(2035, 1, 2, 3, 4, 5, TimeSpan.Zero));

        Assert.True(actionService.AbandonCalled);
        Assert.Equal("workflow-001", actionService.LastWorkflowExecutionId);
        Assert.Contains("Action: Abandon Workflow", text);
        Assert.Contains("Status: Succeeded", text);
    }

    [Fact]
    public void HeartbeatAndPresent_CallsActionServiceAndReturnsFormattedText()
    {
        FakeActionService actionService = new();
        WorkflowRuntimeActionCommandService service =
            new(
                actionService,
                new WorkflowRuntimeActionPresenter(
                    new WorkflowRuntimeActionViewModelFactory()));

        DateTimeOffset heartbeat =
            new(2035, 5, 6, 7, 8, 9, TimeSpan.Zero);

        string text =
            service.HeartbeatAndPresent(
                "workflow-002",
                heartbeat,
                heartbeat);

        Assert.True(actionService.HeartbeatCalled);
        Assert.Equal("workflow-002", actionService.LastWorkflowExecutionId);
        Assert.Equal(heartbeat, actionService.LastHeartbeatUtc);
        Assert.Contains("Action: Update Workflow Heartbeat", text);
    }

    private sealed class FakeActionService : IWorkflowRuntimeActionService
    {
        public bool AbandonCalled { get; private set; }

        public bool HeartbeatCalled { get; private set; }

        public string? LastWorkflowExecutionId { get; private set; }

        public DateTimeOffset? LastHeartbeatUtc { get; private set; }

        public WorkflowRuntimeActionResult Abandon(
            string workflowExecutionId,
            string message)
        {
            AbandonCalled = true;
            LastWorkflowExecutionId = workflowExecutionId;

            return WorkflowRuntimeActionResult.Success(
                workflowExecutionId,
                "Workflow was abandoned.");
        }

        public WorkflowRuntimeActionResult Heartbeat(
            string workflowExecutionId,
            DateTimeOffset heartbeatUtc)
        {
            HeartbeatCalled = true;
            LastWorkflowExecutionId = workflowExecutionId;
            LastHeartbeatUtc = heartbeatUtc;

            return WorkflowRuntimeActionResult.Success(
                workflowExecutionId,
                "Workflow heartbeat was updated.");
        }
    }
}
