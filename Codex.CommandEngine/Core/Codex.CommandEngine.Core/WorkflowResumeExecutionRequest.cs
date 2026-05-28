namespace Codex.CommandEngine.Core;

public sealed class WorkflowResumeExecutionRequest
{
    public required WorkflowResumeRequest ResumeRequest { get; init; }

    public string ContextJson { get; init; } = "{}";
}
