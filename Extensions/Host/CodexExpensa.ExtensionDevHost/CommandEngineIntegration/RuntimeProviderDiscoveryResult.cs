using Codex.CommandEngine.Core;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class RuntimeProviderDiscoveryResult
{
    public IReadOnlyList<IRuntimeCommandRegistrationProvider> Providers { get; init; } =
        [];

    public IReadOnlyList<RuntimeProviderDiscoveryIssue> Issues { get; init; } =
        [];

    public bool HasIssues => Issues.Count > 0;
}
