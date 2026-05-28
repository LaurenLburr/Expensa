using Codex.CommandEngine.Core;
using Codex.CommandEngine.Data;

namespace Codex.CommandEngine.IntegrationTests;

internal sealed class RepositoryWorkflowRuntimeOperations : IWorkflowRuntimeOperations
{
    private readonly WorkflowExecutionRepository _repository;

    public RepositoryWorkflowRuntimeOperations(WorkflowExecutionRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    public IReadOnlyList<WorkflowRuntimeSummary> ListResumable()
    {
        return _repository
            .ListResumable()
            .Select(MapResumeState)
            .ToList();
    }

    public IReadOnlyList<WorkflowRuntimeSummary> ListIncomplete()
    {
        return _repository
            .ListIncomplete()
            .Select(workflow =>
            {
                WorkflowResumeState? resumeState =
                    _repository.FindResumeState(workflow.WorkflowExecutionId);

                return resumeState is null
                    ? new WorkflowRuntimeSummary
                    {
                        WorkflowExecutionId = workflow.WorkflowExecutionId,
                        WorkflowName = workflow.WorkflowName,
                        CorrelationId = workflow.CorrelationId,
                        Status = workflow.Status,
                        OperationStatus = MapOperationStatus(workflow.Status, false),
                        CurrentStepOrder = workflow.CurrentStepOrder,
                        LastCompletedStepOrder = 0,
                        IsResumable = false,
                        RuntimeStateJson = "{}"
                    }
                    : MapResumeState(resumeState);
            })
            .ToList();
    }

    public WorkflowRuntimeDetail GetDetail(string workflowExecutionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowExecutionId);

        WorkflowResumeState state =
            _repository.FindResumeState(workflowExecutionId)
            ?? throw new InvalidOperationException($"Workflow execution '{workflowExecutionId}' was not found.");

        IReadOnlyList<WorkflowRuntimeStepSummary> steps =
            _repository
                .ListSteps(workflowExecutionId)
                .Select(static step => new WorkflowRuntimeStepSummary
                {
                    WorkflowStepExecutionId = step.WorkflowStepExecutionId,
                    StepName = step.StepName,
                    StepOrder = step.StepOrder,
                    CommandName = step.CommandName,
                    Status = step.Status,
                    CompletedUtc = step.CompletedUtc,
                    Message = step.Message,
                    CommandExecutionId = step.CommandExecutionId
                })
                .ToList();

        return new WorkflowRuntimeDetail
        {
            Summary = MapResumeState(state),
            Steps = steps
        };
    }

    public void MarkAbandoned(string workflowExecutionId, string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowExecutionId);
        _repository.MarkAbandoned(workflowExecutionId, message);
    }

    public void Heartbeat(string workflowExecutionId, DateTimeOffset heartbeatUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowExecutionId);
        _repository.Heartbeat(workflowExecutionId, heartbeatUtc.ToString("O"));
    }

    private static WorkflowRuntimeSummary MapResumeState(WorkflowResumeState state)
    {
        return new WorkflowRuntimeSummary
        {
            WorkflowExecutionId = state.WorkflowExecutionId,
            WorkflowName = state.WorkflowName,
            CorrelationId = state.CorrelationId,
            Status = state.Status,
            OperationStatus = MapOperationStatus(state.Status, state.IsResumable),
            CurrentStepOrder = state.CurrentStepOrder,
            LastCompletedStepOrder = state.LastCompletedStepOrder,
            IsResumable = state.IsResumable,
            LastHeartbeatUtc = state.LastHeartbeatUtc,
            RuntimeStateJson = state.RuntimeStateJson
        };
    }

    private static WorkflowRuntimeOperationStatus MapOperationStatus(string status, bool isResumable)
    {
        if (string.Equals(status, "Abandoned", StringComparison.OrdinalIgnoreCase))
        {
            return WorkflowRuntimeOperationStatus.Abandoned;
        }

        if (status is "Succeeded" or "Failed" or "Cancelled")
        {
            return WorkflowRuntimeOperationStatus.Completed;
        }

        if (isResumable)
        {
            return WorkflowRuntimeOperationStatus.Resumable;
        }

        if (string.Equals(status, "Started", StringComparison.OrdinalIgnoreCase))
        {
            return WorkflowRuntimeOperationStatus.Running;
        }

        return WorkflowRuntimeOperationStatus.Unknown;
    }
}
