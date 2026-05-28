namespace Codex.CommandEngine.Core;

public sealed class ExtensionCommandRuntimeBootstrapRequest
{
    public IReadOnlyList<IRuntimeCommandRegistrationProvider> Providers { get; init; } =
        [];

    public bool ThrowIfNoCommands { get; init; }

    public bool ContinueOnRegistrationError { get; init; }
}
