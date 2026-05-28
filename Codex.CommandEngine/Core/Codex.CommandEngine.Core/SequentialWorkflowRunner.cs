namespace Codex.CommandEngine.Core;

public sealed class SequentialWorkflowRunner : IWorkflowRunner
{
    private readonly ICommandDispatcher _commandDispatcher;

    public SequentialWorkflowRunner(ICommandDispatcher commandDispatcher)
    {
        ArgumentNullException.ThrowIfNull(commandDispatcher);

        _commandDispatcher = commandDispatcher;
    }

    public async Task<WorkflowExecutionResult> ExecuteAsync(
        WorkflowExecutionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkflowName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.CorrelationId);

        if (cancellationToken.IsCancellationRequested)
        {
            return new WorkflowExecutionResult
            {
                WorkflowName = request.WorkflowName,
                CorrelationId = request.CorrelationId,
                Status = WorkflowExecutionStatus.Cancelled,
                Message = "Workflow execution was cancelled before it started."
            };
        }

        List<WorkflowStepExecutionResult> stepResults = [];

        foreach (WorkflowStepExecutionRequest step in request.Steps.OrderBy(static step => step.StepOrder))
        {
            cancellationToken.ThrowIfCancellationRequested();

            CommandExecutionResult commandResult =
                await _commandDispatcher.ExecuteAsync(
                    new CommandExecutionRequest
                    {
                        CommandName = step.CommandName,
                        CorrelationId = request.CorrelationId,
                        ContextJson = request.ContextJson,
                        Parameters = step.Parameters
                    },
                    cancellationToken).ConfigureAwait(false);

            WorkflowStepExecutionStatus stepStatus =
                MapCommandStatus(commandResult.Status);

            WorkflowStepExecutionResult stepResult =
                new()
                {
                    StepName = step.StepName,
                    CommandName = step.CommandName,
                    StepOrder = step.StepOrder,
                    Status = stepStatus,
                    Message = commandResult.Message,
                    CommandResult = commandResult
                };

            stepResults.Add(stepResult);

            if (stepStatus is WorkflowStepExecutionStatus.Failed or WorkflowStepExecutionStatus.Cancelled)
            {
                return new WorkflowExecutionResult
                {
                    WorkflowName = request.WorkflowName,
                    CorrelationId = request.CorrelationId,
                    Status = stepStatus == WorkflowStepExecutionStatus.Cancelled
                        ? WorkflowExecutionStatus.Cancelled
                        : WorkflowExecutionStatus.Failed,
                    Message = $"Workflow stopped at step '{step.StepName}'.",
                    StepResults = stepResults
                };
            }
        }

        return new WorkflowExecutionResult
        {
            WorkflowName = request.WorkflowName,
            CorrelationId = request.CorrelationId,
            Status = WorkflowExecutionStatus.Succeeded,
            Message = "Workflow completed successfully.",
            StepResults = stepResults
        };
    }

    private static WorkflowStepExecutionStatus MapCommandStatus(CommandExecutionStatus status)
    {
        return status switch
        {
            CommandExecutionStatus.Succeeded => WorkflowStepExecutionStatus.Succeeded,
            CommandExecutionStatus.Cancelled => WorkflowStepExecutionStatus.Cancelled,
            CommandExecutionStatus.Failed => WorkflowStepExecutionStatus.Failed,
            _ => WorkflowStepExecutionStatus.Failed
        };
    }
}
