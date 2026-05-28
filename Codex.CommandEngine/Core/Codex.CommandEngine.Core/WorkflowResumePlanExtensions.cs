namespace Codex.CommandEngine.Core;

public static class WorkflowResumePlanExtensions
{
    public static WorkflowExecutionRequest ToWorkflowExecutionRequest(
        this WorkflowResumePlan plan,
        IReadOnlyList<WorkflowStepExecutionRequest> allSteps,
        string contextJson = "{}")
    {
        ArgumentNullException.ThrowIfNull(plan);
        ArgumentNullException.ThrowIfNull(allSteps);

        if (!plan.CanResume)
        {
            throw new InvalidOperationException("Cannot create a workflow execution request from a non-resumable plan.");
        }

        return new WorkflowExecutionRequest
        {
            WorkflowName = plan.WorkflowName,
            CorrelationId = plan.CorrelationId,
            ContextJson = contextJson,
            Steps = allSteps
                .Where(step => step.StepOrder > plan.LastCompletedStepOrder)
                .OrderBy(static step => step.StepOrder)
                .ToList()
        };
    }
}
