namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class RuntimeProviderDiscoveryIssue
{
    public required string Source { get; init; }

    public required string Message { get; init; }

    public Exception? Exception { get; init; }
}
