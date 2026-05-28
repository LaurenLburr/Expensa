namespace Codex.CommandEngine.Core;

public sealed class WorkflowResumePlanner : IWorkflowResumePlanner
{
    public WorkflowResumePlan CreateResumePlan(WorkflowResumeRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkflowExecutionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkflowName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.CorrelationId);

        WorkflowStepExecutionRequest? nextStep =
            request.Steps
                .Where(step => step.StepOrder > request.LastCompletedStepOrder)
                .OrderBy(static step => step.StepOrder)
                .FirstOrDefault();

        if (nextStep is null)
        {
            return new WorkflowResumePlan
            {
                WorkflowExecutionId = request.WorkflowExecutionId,
                WorkflowName = request.WorkflowName,
                CorrelationId = request.CorrelationId,
                LastCompletedStepOrder = request.LastCompletedStepOrder,
                NextStep = null,
                Message = "No pending workflow steps were found."
            };
        }

        return new WorkflowResumePlan
        {
            WorkflowExecutionId = request.WorkflowExecutionId,
            WorkflowName = request.WorkflowName,
            CorrelationId = request.CorrelationId,
            LastCompletedStepOrder = request.LastCompletedStepOrder,
            NextStep = nextStep,
            Message = $"Next step is '{nextStep.StepName}'."
        };
    }
}
