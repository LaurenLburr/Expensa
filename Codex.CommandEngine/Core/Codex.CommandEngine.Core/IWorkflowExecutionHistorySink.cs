namespace Codex.CommandEngine.Core;

public interface IWorkflowExecutionHistorySink
{
    void Started(
        WorkflowExecutionRequest request,
        string workflowExecutionId,
        DateTimeOffset startedUtc);

    void Completed(
        WorkflowExecutionRequest request,
        WorkflowExecutionResult result,
        string workflowExecutionId,
        DateTimeOffset completedUtc,
        long durationMilliseconds);
}
