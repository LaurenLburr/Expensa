using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public static class DefaultTreeContributionCommandAdapterRegistryFactory
{
    public static TreeContributionCommandAdapterRegistry Create()
    {
        return new TreeContributionCommandAdapterRegistry(
            [
                new WebsitesTreeContributionCommandAdapter()
            ]);
    }
}
