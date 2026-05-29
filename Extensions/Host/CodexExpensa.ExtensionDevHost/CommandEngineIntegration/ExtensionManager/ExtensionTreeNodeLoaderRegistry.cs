namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed class ExtensionTreeNodeLoaderRegistry
{
    private readonly IReadOnlyList<IExtensionTreeNodeLoaderProvider> _providers;

    public ExtensionTreeNodeLoaderRegistry(
        IReadOnlyList<IExtensionTreeNodeLoaderProvider> providers)
    {
        ArgumentNullException.ThrowIfNull(providers);

        _providers = providers;
    }

    public IReadOnlyList<IExtensionTreeNodeLoader> GetLoaders()
    {
        List<IExtensionTreeNodeLoader> loaders = [];

        foreach (IExtensionTreeNodeLoaderProvider provider in _providers)
        {
            IReadOnlyList<IExtensionTreeNodeLoader> providerLoaders =
                provider.GetLoaders();

            loaders.AddRange(providerLoaders);
        }

        return loaders
            .OrderBy(static loader => loader.SortOrder)
            .ThenBy(static loader => loader.AddinId, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
