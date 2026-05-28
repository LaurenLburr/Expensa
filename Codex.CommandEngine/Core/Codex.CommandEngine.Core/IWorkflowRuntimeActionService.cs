namespace Codex.CommandEngine.Core;

public interface IWorkflowRuntimeActionService
{
    WorkflowRuntimeActionResult Abandon(
        string workflowExecutionId,
        string message);

    WorkflowRuntimeActionResult Heartbeat(
        string workflowExecutionId,
        DateTimeOffset heartbeatUtc);
}
