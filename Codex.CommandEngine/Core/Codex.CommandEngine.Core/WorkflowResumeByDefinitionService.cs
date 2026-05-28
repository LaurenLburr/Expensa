namespace Codex.CommandEngine.Core;

public sealed class WorkflowResumeByDefinitionService : IWorkflowResumeByDefinitionService
{
    private readonly IWorkflowRuntimeOperations _runtimeOperations;
    private readonly IWorkflowDefinitionStore _definitionStore;
    private readonly IWorkflowResumeService _resumeService;

    public WorkflowResumeByDefinitionService(
        IWorkflowRuntimeOperations runtimeOperations,
        IWorkflowDefinitionStore definitionStore,
        IWorkflowResumeService resumeService)
    {
        ArgumentNullException.ThrowIfNull(runtimeOperations);
        ArgumentNullException.ThrowIfNull(definitionStore);
        ArgumentNullException.ThrowIfNull(resumeService);

        _runtimeOperations = runtimeOperations;
        _definitionStore = definitionStore;
        _resumeService = resumeService;
    }

    public async Task<WorkflowExecutionResult> ResumeAsync(
        WorkflowResumeByDefinitionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkflowExecutionId);

        WorkflowRuntimeDetail detail =
            _runtimeOperations.GetDetail(request.WorkflowExecutionId);

        if (!detail.Summary.IsResumable)
        {
            return new WorkflowExecutionResult
            {
                WorkflowName = detail.Summary.WorkflowName,
                CorrelationId = detail.Summary.CorrelationId,
                Status = WorkflowExecutionStatus.Failed,
                Message = "Workflow execution is not resumable."
            };
        }

        WorkflowDefinitionDocument definition =
            _definitionStore.FindByName(detail.Summary.WorkflowName)
            ?? throw new InvalidOperationException(
                $"Workflow definition '{detail.Summary.WorkflowName}' was not found.");

        IReadOnlyList<WorkflowStepExecutionRequest> steps =
            definition.Steps
                .OrderBy(static step => step.StepOrder)
                .Select(static step => new WorkflowStepExecutionRequest
                {
                    StepName = step.StepName,
                    StepOrder = step.StepOrder,
                    CommandName = step.CommandName,
                    Parameters = step.Parameters
                })
                .ToList();

        return await _resumeService.ResumeAsync(
            new WorkflowResumeServiceRequest
            {
                WorkflowExecutionId = request.WorkflowExecutionId,
                WorkflowDefinitionSteps = steps
            },
            cancellationToken).ConfigureAwait(false);
    }
}
