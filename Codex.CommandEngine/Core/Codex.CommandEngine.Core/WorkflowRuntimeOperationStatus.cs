namespace Codex.CommandEngine.Core;

public enum WorkflowRuntimeOperationStatus
{
    Unknown = 0,
    Resumable = 1,
    Abandoned = 2,
    Completed = 3,
    Running = 4
}
