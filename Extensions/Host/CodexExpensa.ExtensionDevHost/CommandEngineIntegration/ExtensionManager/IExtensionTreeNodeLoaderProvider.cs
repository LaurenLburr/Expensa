namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public interface IExtensionTreeNodeLoaderProvider
{
    IReadOnlyList<IExtensionTreeNodeLoader> GetLoaders();
}
