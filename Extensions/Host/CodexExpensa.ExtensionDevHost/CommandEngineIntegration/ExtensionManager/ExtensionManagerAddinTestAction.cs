namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed class ExtensionManagerAddinTestAction
{
    public required ExtensionManagerAddinTestNode Addin { get; init; }

    public required ExtensionManagerAddinTestActionKind ActionKind { get; init; }

    public string ActionKey => ActionKind.ToString();
}
