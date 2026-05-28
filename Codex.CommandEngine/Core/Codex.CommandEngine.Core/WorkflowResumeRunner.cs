namespace Codex.CommandEngine.Core;

public sealed class WorkflowResumeRunner : IWorkflowResumeRunner
{
    private readonly IWorkflowResumePlanner _planner;
    private readonly IWorkflowRunner _workflowRunner;

    public WorkflowResumeRunner(
        IWorkflowResumePlanner planner,
        IWorkflowRunner workflowRunner)
    {
        ArgumentNullException.ThrowIfNull(planner);
        ArgumentNullException.ThrowIfNull(workflowRunner);

        _planner = planner;
        _workflowRunner = workflowRunner;
    }

    public async Task<WorkflowExecutionResult> ResumeAsync(
        WorkflowResumeExecutionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.ResumeRequest);

        WorkflowResumePlan plan =
            _planner.CreateResumePlan(request.ResumeRequest);

        if (!plan.CanResume)
        {
            return new WorkflowExecutionResult
            {
                WorkflowName = plan.WorkflowName,
                CorrelationId = plan.CorrelationId,
                Status = WorkflowExecutionStatus.Succeeded,
                Message = "Workflow has no pending steps to resume."
            };
        }

        WorkflowExecutionRequest workflowRequest =
            plan.ToWorkflowExecutionRequest(
                request.ResumeRequest.Steps,
                request.ContextJson);

        return await _workflowRunner
            .ExecuteAsync(workflowRequest, cancellationToken)
            .ConfigureAwait(false);
    }
}
