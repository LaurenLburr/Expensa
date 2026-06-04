using Codex.CommandEngine.Core;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed class HostBudgetTreeLoadResult
{
    public required CommandExecutionResult ExecutionResult { get; init; }
    public int RootNodeCount { get; init; }
}
