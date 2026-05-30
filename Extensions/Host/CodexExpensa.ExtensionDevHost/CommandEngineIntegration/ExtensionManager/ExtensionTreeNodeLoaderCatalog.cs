namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed class ExtensionTreeNodeLoaderCatalog
{
    private readonly string _modulesFolder;

    public ExtensionTreeNodeLoaderCatalog()
        : this(ExtensionModuleFolderResolver.ResolveDefaultModulesFolder())
    {
    }

    public ExtensionTreeNodeLoaderCatalog(
        string modulesFolder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(modulesFolder);

        _modulesFolder = modulesFolder;
    }

    public IReadOnlyList<IExtensionTreeNodeLoader> GetLoaders()
    {
        ExtensionModuleAssemblyLoader assemblyLoader = new();

        IReadOnlyList<ExtensionModuleAssemblyLoadResult> assemblyResults =
            assemblyLoader.LoadAssemblies(_modulesFolder);

        IReadOnlyList<System.Reflection.Assembly> loadedAssemblies =
            assemblyResults
                .Where(static result => result.Assembly is not null)
                .Select(static result => result.Assembly!)
                .ToList();

        ExtensionTreeContributionProviderDiscovery discovery = new();

        IReadOnlyList<ExtensionTreeContributionDescriptor> descriptors =
            discovery.DiscoverDescriptors(loadedAssemblies);

        return descriptors
            .Select(static descriptor => (IExtensionTreeNodeLoader)new ModuleCommandTreeNodeLoader(descriptor))
            .OrderBy(static loader => loader.SortOrder)
            .ThenBy(static loader => loader.AddinId, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
