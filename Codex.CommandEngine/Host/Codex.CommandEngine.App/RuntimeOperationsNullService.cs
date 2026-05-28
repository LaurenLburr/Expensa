using Codex.CommandEngine.Core;

namespace Codex.CommandEngine.App;

public sealed class RuntimeOperationsNullService : IWorkflowRuntimeOperations
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
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowExecutionId);

        throw new InvalidOperationException(
            "Runtime workflow detail is unavailable because no workflow runtime operations provider is configured.");
    }

    public void MarkAbandoned(string workflowExecutionId, string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowExecutionId);

        throw new InvalidOperationException(
            "Runtime workflow abandon is unavailable because no workflow runtime operations provider is configured.");
    }

    public void Heartbeat(string workflowExecutionId, DateTimeOffset heartbeatUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowExecutionId);

        throw new InvalidOperationException(
            "Runtime workflow heartbeat is unavailable because no workflow runtime operations provider is configured.");
    }
}
