namespace Codex.CommandEngine.Core;

public enum WorkflowHeartbeatStatus
{
    Unknown = 0,
    Healthy = 1,
    Stale = 2,
    Missing = 3
}
