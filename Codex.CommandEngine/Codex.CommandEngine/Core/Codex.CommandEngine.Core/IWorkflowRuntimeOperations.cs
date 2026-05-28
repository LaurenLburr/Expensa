namespace Codex.CommandEngine.Core;

public interface IWorkflowRuntimeOperations
{
    IReadOnlyList<WorkflowRuntimeSummary> ListResumable();

    IReadOnlyList<WorkflowRuntimeSummary> ListIncomplete();

    WorkflowRuntimeDetail GetDetail(string workflowExecutionId);

    void MarkAbandoned(string workflowExecutionId, string message);

    void Heartbeat(string workflowExecutionId, DateTimeOffset heartbeatUtc);
}
