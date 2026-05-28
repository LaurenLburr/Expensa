namespace Codex.CommandEngine.Core;

public sealed class WorkflowRuntimeDetail
{
    public required WorkflowRuntimeSummary Summary { get; init; }

    public IReadOnlyList<WorkflowRuntimeStepSummary> Steps { get; init; } =
        [];
}
