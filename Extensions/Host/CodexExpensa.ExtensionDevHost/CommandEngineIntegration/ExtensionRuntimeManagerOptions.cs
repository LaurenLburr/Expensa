namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class ExtensionRuntimeManagerOptions
{
    public bool ThrowIfNoCommands { get; init; } = true;

    public bool ContinueOnRegistrationError { get; init; } = true;
}
