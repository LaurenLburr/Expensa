namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed class ExtensionTreeNodeLoaderCatalog
{
    public IReadOnlyList<IExtensionTreeNodeLoader> GetLoaders()
    {
        ExtensionTreeNodeLoaderProviderCatalog providerCatalog = new();

        ExtensionTreeNodeLoaderRegistry registry =
            new(providerCatalog.GetProviders());

        return registry.GetLoaders();
    }
}
