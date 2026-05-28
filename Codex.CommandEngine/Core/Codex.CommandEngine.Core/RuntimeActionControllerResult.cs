namespace Codex.CommandEngine.Core;

public sealed class RuntimeActionControllerResult
{
    public required string WorkflowExecutionId { get; init; }

    public required string ActionName { get; init; }

    public bool Succeeded { get; init; }

    public string DisplayText { get; init; } = string.Empty;

    public string RefreshText { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;
}
