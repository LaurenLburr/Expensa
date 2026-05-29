using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed class ExtensionTreeNodeLoaderCatalog
{
    public IReadOnlyList<IExtensionTreeNodeLoader> GetLoaders()
    {
        return
        [
            new WebsitesExtensionTreeNodeLoader()
        ];
    }
}
