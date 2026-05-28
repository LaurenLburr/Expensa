using Codex.CommandEngine.Core;
using Codex.CommandEngine.Data;

namespace Codex.CommandEngine.IntegrationTests;

internal sealed class RepositoryWorkflowResumeRequestLoader : IWorkflowResumeRequestLoader
{
    private readonly WorkflowExecutionRepository _repository;

    public RepositoryWorkflowResumeRequestLoader(WorkflowExecutionRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);

        _repository = repository;
    }

    public WorkflowResumeExecutionRequest Load(
        string workflowExecutionId,
        IReadOnlyList<WorkflowStepExecutionRequest> workflowDefinitionSteps)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowExecutionId);
        ArgumentNullException.ThrowIfNull(workflowDefinitionSteps);

        WorkflowResumeState state =
            _repository.FindResumeState(workflowExecutionId)
            ?? throw new WorkflowResumeLoadException($"Workflow execution '{workflowExecutionId}' was not found.");

        if (!state.IsResumable)
        {
            throw new WorkflowResumeLoadException($"Workflow execution '{workflowExecutionId}' is not resumable.");
        }

        if (state.Status is "Succeeded" or "Failed" or "Cancelled" or "Abandoned")
        {
            throw new WorkflowResumeLoadException(
                $"Workflow execution '{workflowExecutionId}' cannot be resumed because its status is '{state.Status}'.");
        }

        return new WorkflowResumeExecutionRequest
        {
            ContextJson = state.RuntimeStateJson,
            ResumeRequest = new WorkflowResumeRequest
            {
                WorkflowExecutionId = state.WorkflowExecutionId,
                WorkflowName = state.WorkflowName,
                CorrelationId = state.CorrelationId,
                LastCompletedStepOrder = state.LastCompletedStepOrder,
                Steps = workflowDefinitionSteps
            }
        };
    }
}
