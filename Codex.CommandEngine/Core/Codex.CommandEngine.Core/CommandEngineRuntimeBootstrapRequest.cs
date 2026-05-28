namespace Codex.CommandEngine.Core;

public sealed class CommandEngineRuntimeBootstrapRequest
{
    public IReadOnlyList<RuntimeCommandRegistration> Commands { get; init; } =
        [];

    public bool ThrowIfNoCommands { get; init; }

    public bool ContinueOnRegistrationError { get; init; }
}
