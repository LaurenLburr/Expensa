namespace Codex.CommandEngine.Core;

public sealed class WorkflowResumeLoadException : Exception
{
    public WorkflowResumeLoadException(string message)
        : base(message)
    {
    }
}
