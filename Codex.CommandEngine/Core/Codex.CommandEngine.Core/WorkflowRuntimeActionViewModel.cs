namespace Codex.CommandEngine.Core;

public sealed class WorkflowRuntimeActionViewModel
{
    public required string WorkflowExecutionId { get; init; }

    public required string ActionName { get; init; }

    public bool Succeeded { get; init; }

    public string Message { get; init; } = string.Empty;

    public DateTimeOffset CompletedUtc { get; init; } = DateTimeOffset.UtcNow;

    public string DisplayStatus => Succeeded ? "Succeeded" : "Failed";
}
