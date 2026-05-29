using Codex.CommandEngine.Core;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class HostWebsiteTreeLoadResult
{
    public required CommandExecutionResult ExecutionResult { get; init; }

    public required HostWebsiteLoadResult WebsiteResult { get; init; }

    public int RootNodeCount =>
        WebsiteResult.Nodes.Count;

    public bool Succeeded =>
        ExecutionResult.Status == CommandExecutionStatus.Succeeded;
}
