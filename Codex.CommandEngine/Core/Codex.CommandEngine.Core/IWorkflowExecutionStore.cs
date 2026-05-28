namespace Codex.CommandEngine.Core;

public interface IWorkflowExecutionStore
{
    void StartWorkflow(
        string workflowExecutionId,
        WorkflowExecutionRequest request,
        DateTimeOffset startedUtc);

    void CompleteWorkflow(
        string workflowExecutionId,
        WorkflowExecutionResult result,
        DateTimeOffset completedUtc);

    void StartStep(
        string workflowStepExecutionId,
        string workflowExecutionId,
        WorkflowStepExecutionRequest step,
        DateTimeOffset startedUtc);

    void CompleteStep(
        string workflowStepExecutionId,
        WorkflowStepExecutionResult stepResult,
        string? commandExecutionId,
        DateTimeOffset completedUtc);
}
