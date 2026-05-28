using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class WorkflowRuntimeActionCommandServiceFailureTests
{
    [Fact]
    public void AbandonAndPresent_WhenActionFails_ReturnsFailedFormattedText()
    {
        WorkflowRuntimeActionCommandService service =
            new(
                new FailingActionService(),
                new WorkflowRuntimeActionPresenter(
                    new WorkflowRuntimeActionViewModelFactory()));

        string text =
            service.AbandonAndPresent(
                "workflow-001",
                "Stop.",
                new DateTimeOffset(2035, 1, 2, 3, 4, 5, TimeSpan.Zero));

        Assert.Contains("Status: Failed", text);
        Assert.Contains("Nope.", text);
    }

    [Fact]
    public void HeartbeatAndPresent_WhenActionFails_ReturnsFailedFormattedText()
    {
        WorkflowRuntimeActionCommandService service =
            new(
                new FailingActionService(),
                new WorkflowRuntimeActionPresenter(
                    new WorkflowRuntimeActionViewModelFactory()));

        string text =
            service.HeartbeatAndPresent(
                "workflow-002",
                DateTimeOffset.UtcNow,
                new DateTimeOffset(2035, 1, 2, 3, 4, 5, TimeSpan.Zero));

        Assert.Contains("Status: Failed", text);
        Assert.Contains("Nope.", text);
    }

    private sealed class FailingActionService : IWorkflowRuntimeActionService
    {
        public WorkflowRuntimeActionResult Abandon(
            string workflowExecutionId,
            string message)
        {
            return WorkflowRuntimeActionResult.Failure(
                workflowExecutionId,
                "Nope.");
        }

        public WorkflowRuntimeActionResult Heartbeat(
            string workflowExecutionId,
            DateTimeOffset heartbeatUtc)
        {
            return WorkflowRuntimeActionResult.Failure(
                workflowExecutionId,
                "Nope.");
        }
    }
}
