namespace Codex.CommandEngine.Data;

public sealed class WorkflowResumeUpdate
{
    public required string WorkflowExecutionId { get; init; }

    public int LastCompletedStepOrder { get; init; }

    public string RuntimeStateJson { get; init; } = "{}";

    public bool IsResumable { get; init; } = true;

    public string ResumeToken { get; init; } = Guid.NewGuid().ToString("N");
}
