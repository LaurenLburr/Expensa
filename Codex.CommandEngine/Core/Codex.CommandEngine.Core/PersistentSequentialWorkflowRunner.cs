namespace Codex.CommandEngine.Core;

public sealed class PersistentSequentialWorkflowRunner : IWorkflowRunner
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IWorkflowExecutionStore _workflowStore;

    public PersistentSequentialWorkflowRunner(
        ICommandDispatcher commandDispatcher,
        IWorkflowExecutionStore workflowStore)
    {
        ArgumentNullException.ThrowIfNull(commandDispatcher);
        ArgumentNullException.ThrowIfNull(workflowStore);

        _commandDispatcher = commandDispatcher;
        _workflowStore = workflowStore;
    }

    public async Task<WorkflowExecutionResult> ExecuteAsync(
        WorkflowExecutionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkflowName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.CorrelationId);

        string workflowExecutionId = Guid.NewGuid().ToString("N");

        _workflowStore.StartWorkflow(
            workflowExecutionId,
            request,
            DateTimeOffset.UtcNow);

        if (cancellationToken.IsCancellationRequested)
        {
            WorkflowExecutionResult cancelledBeforeStart =
                new()
                {
                    WorkflowName = request.WorkflowName,
                    CorrelationId = request.CorrelationId,
                    Status = WorkflowExecutionStatus.Cancelled,
                    Message = "Workflow execution was cancelled before the first step."
                };

            _workflowStore.CompleteWorkflow(
                workflowExecutionId,
                cancelledBeforeStart,
                DateTimeOffset.UtcNow);

            return cancelledBeforeStart;
        }

        List<WorkflowStepExecutionResult> stepResults = [];

        foreach (WorkflowStepExecutionRequest step in request.Steps.OrderBy(static item => item.StepOrder))
        {
            string workflowStepExecutionId = Guid.NewGuid().ToString("N");

            _workflowStore.StartStep(
                workflowStepExecutionId,
                workflowExecutionId,
                step,
                DateTimeOffset.UtcNow);

            CommandExecutionResult commandResult;

            try
            {
                commandResult =
                    await _commandDispatcher.ExecuteAsync(
                        new CommandExecutionRequest
                        {
                            CommandName = step.CommandName,
                            CorrelationId = request.CorrelationId,
                            ContextJson = request.ContextJson,
                            Parameters = step.Parameters
                        },
                        cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                commandResult = CommandExecutionResult.Cancelled(
                    step.CommandName,
                    request.CorrelationId);
            }
            catch (Exception exception)
            {
                commandResult = CommandExecutionResult.Failed(
                    step.CommandName,
                    request.CorrelationId,
                    exception.Message,
                    exception);
            }

            WorkflowStepExecutionResult stepResult =
                new()
                {
                    StepName = step.StepName,
                    CommandName = step.CommandName,
                    StepOrder = step.StepOrder,
                    Status = MapCommandStatus(commandResult.Status),
                    Message = commandResult.Message,
                    CommandResult = commandResult
                };

            stepResults.Add(stepResult);

            _workflowStore.CompleteStep(
                workflowStepExecutionId,
                stepResult,
                null,
                DateTimeOffset.UtcNow);

            if (stepResult.Status is WorkflowStepExecutionStatus.Failed or WorkflowStepExecutionStatus.Cancelled)
            {
                WorkflowExecutionResult stoppedResult =
                    new()
                    {
                        WorkflowName = request.WorkflowName,
                        CorrelationId = request.CorrelationId,
                        Status = stepResult.Status == WorkflowStepExecutionStatus.Cancelled
                            ? WorkflowExecutionStatus.Cancelled
                            : WorkflowExecutionStatus.Failed,
                        Message = $"Workflow stopped at step '{step.StepName}'.",
                        StepResults = stepResults
                    };

                _workflowStore.CompleteWorkflow(
                    workflowExecutionId,
                    stoppedResult,
                    DateTimeOffset.UtcNow);

                return stoppedResult;
            }
        }

        WorkflowExecutionResult completedResult =
            new()
            {
                WorkflowName = request.WorkflowName,
                CorrelationId = request.CorrelationId,
                Status = WorkflowExecutionStatus.Succeeded,
                Message = "Workflow completed successfully.",
                StepResults = stepResults
            };

        _workflowStore.CompleteWorkflow(
            workflowExecutionId,
            completedResult,
            DateTimeOffset.UtcNow);

        return completedResult;
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
