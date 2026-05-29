using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed class ExtensionTreeNodeLoaderProviderCatalog
{
    private readonly string _modulesFolder;

    public ExtensionTreeNodeLoaderProviderCatalog()
        : this(ExtensionModuleFolderResolver.ResolveDefaultModulesFolder())
    {
    }

    public ExtensionTreeNodeLoaderProviderCatalog(
        string modulesFolder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(modulesFolder);

        _modulesFolder = modulesFolder;
    }

    public IReadOnlyList<IExtensionTreeNodeLoaderProvider> GetProviders()
    {
        List<IExtensionTreeNodeLoaderProvider> providers = [];

        ExtensionModuleAssemblyLoader assemblyLoader = new();

        IReadOnlyList<ExtensionModuleAssemblyLoadResult> assemblyResults =
            assemblyLoader.LoadAssemblies(_modulesFolder);

        IReadOnlyList<System.Reflection.Assembly> loadedAssemblies =
            assemblyResults
                .Where(static result => result.Assembly is not null)
                .Select(static result => result.Assembly!)
                .ToList();

        ExtensionTreeNodeLoaderProviderDiscovery discovery = new();

        providers.AddRange(
            discovery.DiscoverProviders(loadedAssemblies));

        if (providers.Count == 0)
        {
            providers.Add(new WebsitesExtensionTreeNodeLoaderProvider());
        }

        return providers;
    }
}
