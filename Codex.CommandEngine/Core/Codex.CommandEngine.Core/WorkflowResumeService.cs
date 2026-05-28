namespace Codex.CommandEngine.Core;

public sealed class WorkflowResumeService : IWorkflowResumeService
{
    private readonly IWorkflowResumeRequestLoader _loader;
    private readonly IWorkflowResumeRunner _runner;

    public WorkflowResumeService(
        IWorkflowResumeRequestLoader loader,
        IWorkflowResumeRunner runner)
    {
        ArgumentNullException.ThrowIfNull(loader);
        ArgumentNullException.ThrowIfNull(runner);

        _loader = loader;
        _runner = runner;
    }

    public Task<WorkflowExecutionResult> ResumeAsync(
        WorkflowResumeServiceRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkflowExecutionId);
        ArgumentNullException.ThrowIfNull(request.WorkflowDefinitionSteps);

        WorkflowResumeExecutionRequest resumeRequest =
            _loader.Load(
                request.WorkflowExecutionId,
                request.WorkflowDefinitionSteps);

        return _runner.ResumeAsync(resumeRequest, cancellationToken);
    }
}
