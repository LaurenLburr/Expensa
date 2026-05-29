using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class WebsitesExtensionTreeNodeLoaderProvider : IExtensionTreeNodeLoaderProvider
{
    public IReadOnlyList<IExtensionTreeNodeLoader> GetLoaders()
    {
        return
        [
            new WebsitesExtensionTreeNodeLoader()
        ];
    }
}
