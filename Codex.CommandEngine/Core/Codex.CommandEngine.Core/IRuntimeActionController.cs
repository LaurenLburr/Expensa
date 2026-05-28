namespace Codex.CommandEngine.Core;

public interface IRuntimeActionController
{
    RuntimeActionControllerResult AbandonWorkflow(
        string workflowExecutionId,
        string message,
        DateTimeOffset completedUtc);

    RuntimeActionControllerResult UpdateHeartbeat(
        string workflowExecutionId,
        DateTimeOffset heartbeatUtc,
        DateTimeOffset completedUtc);
}
