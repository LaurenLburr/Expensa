namespace Codex.CommandEngine.Core;

public sealed class WorkflowRuntimeActionService : IWorkflowRuntimeActionService
{
    private readonly IWorkflowRuntimeOperations _operations;

    public WorkflowRuntimeActionService(IWorkflowRuntimeOperations operations)
    {
        ArgumentNullException.ThrowIfNull(operations);

        _operations = operations;
    }

    public WorkflowRuntimeActionResult Abandon(
        string workflowExecutionId,
        string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowExecutionId);

        if (string.IsNullOrWhiteSpace(message))
        {
            message = "Workflow was abandoned by operator request.";
        }

        try
        {
            WorkflowRuntimeDetail detail =
                _operations.GetDetail(workflowExecutionId);

            if (detail.Summary.OperationStatus == WorkflowRuntimeOperationStatus.Completed)
            {
                return WorkflowRuntimeActionResult.Failure(
                    workflowExecutionId,
                    "Completed workflows cannot be abandoned.");
            }

            if (detail.Summary.OperationStatus == WorkflowRuntimeOperationStatus.Abandoned)
            {
                return WorkflowRuntimeActionResult.Failure(
                    workflowExecutionId,
                    "Workflow is already abandoned.");
            }

            _operations.MarkAbandoned(workflowExecutionId, message);

            return WorkflowRuntimeActionResult.Success(
                workflowExecutionId,
                "Workflow was abandoned.");
        }
        catch (Exception exception)
        {
            return WorkflowRuntimeActionResult.Failure(
                workflowExecutionId,
                exception.Message);
        }
    }

    public WorkflowRuntimeActionResult Heartbeat(
        string workflowExecutionId,
        DateTimeOffset heartbeatUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowExecutionId);

        try
        {
            WorkflowRuntimeDetail detail =
                _operations.GetDetail(workflowExecutionId);

            if (detail.Summary.OperationStatus is WorkflowRuntimeOperationStatus.Completed
                or WorkflowRuntimeOperationStatus.Abandoned)
            {
                return WorkflowRuntimeActionResult.Failure(
                    workflowExecutionId,
                    "Heartbeat cannot be updated for completed or abandoned workflows.");
            }

            _operations.Heartbeat(workflowExecutionId, heartbeatUtc);

            return WorkflowRuntimeActionResult.Success(
                workflowExecutionId,
                "Workflow heartbeat was updated.");
        }
        catch (Exception exception)
        {
            return WorkflowRuntimeActionResult.Failure(
                workflowExecutionId,
                exception.Message);
        }
    }
}
