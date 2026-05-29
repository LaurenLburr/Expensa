namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public interface IExtensionManagerAddinTestCatalog
{
    IReadOnlyList<ExtensionManagerAddinTestNode> GetAddins();
}
