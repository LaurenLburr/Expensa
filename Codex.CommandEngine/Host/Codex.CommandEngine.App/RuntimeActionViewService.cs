using Codex.CommandEngine.Core;

namespace Codex.CommandEngine.App;

public sealed class RuntimeActionViewService
{
    private readonly WorkflowRuntimeActionCommandService _commandService;

    public RuntimeActionViewService(
        WorkflowRuntimeActionCommandService commandService)
    {
        ArgumentNullException.ThrowIfNull(commandService);

        _commandService = commandService;
    }

    public string AbandonWorkflow(
        string workflowExecutionId,
        string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowExecutionId);

        return _commandService.AbandonAndPresent(
            workflowExecutionId,
            message,
            DateTimeOffset.UtcNow);
    }

    public string UpdateWorkflowHeartbeat(
        string workflowExecutionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowExecutionId);

        DateTimeOffset nowUtc =
            DateTimeOffset.UtcNow;

        return _commandService.HeartbeatAndPresent(
            workflowExecutionId,
            nowUtc,
            nowUtc);
    }
}
