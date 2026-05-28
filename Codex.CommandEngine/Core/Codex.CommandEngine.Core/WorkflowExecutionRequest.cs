namespace Codex.CommandEngine.Core;

public sealed class WorkflowExecutionRequest
{
    public required string WorkflowName { get; init; }

    public string CorrelationId { get; init; } = Guid.NewGuid().ToString("N");

    public string ContextJson { get; init; } = "{}";

    public IReadOnlyList<WorkflowStepExecutionRequest> Steps { get; init; } =
        [];
}
