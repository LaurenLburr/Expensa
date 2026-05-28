namespace Codex.CommandEngine.Core;

public sealed class WorkflowRuntimeActionResult
{
    public required string WorkflowExecutionId { get; init; }

    public bool Succeeded { get; init; }

    public string Message { get; init; } = string.Empty;

    public static WorkflowRuntimeActionResult Success(
        string workflowExecutionId,
        string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowExecutionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        return new WorkflowRuntimeActionResult
        {
            WorkflowExecutionId = workflowExecutionId,
            Succeeded = true,
            Message = message
        };
    }

    public static WorkflowRuntimeActionResult Failure(
        string workflowExecutionId,
        string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowExecutionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        return new WorkflowRuntimeActionResult
        {
            WorkflowExecutionId = workflowExecutionId,
            Succeeded = false,
            Message = message
        };
    }
}
