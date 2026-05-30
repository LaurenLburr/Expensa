namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed class ModuleCommandTreeNodeLoader : IExtensionTreeNodeLoader
{
    private readonly ExtensionTreeContributionDescriptor _descriptor;
    private readonly TreeContributionCommandAdapterRegistry _adapterRegistry;

    public ModuleCommandTreeNodeLoader(
        ExtensionTreeContributionDescriptor descriptor)
        : this(
            descriptor,
            DefaultTreeContributionCommandAdapterRegistryFactory.Create())
    {
    }

    public ModuleCommandTreeNodeLoader(
        ExtensionTreeContributionDescriptor descriptor,
        TreeContributionCommandAdapterRegistry adapterRegistry)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        ArgumentNullException.ThrowIfNull(adapterRegistry);

        _descriptor = descriptor;
        _adapterRegistry = adapterRegistry;
    }

    public string AddinId =>
        _descriptor.AddinId;

    public string DisplayName =>
        _descriptor.DisplayName;

    public int SortOrder =>
        _descriptor.SortOrder;

    public async Task LoadNodeAsync(
        TreeView treeView,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(treeView);

        ITreeContributionCommandAdapter adapter =
            _adapterRegistry.GetRequiredAdapter(_descriptor.CommandName);

        await adapter.LoadNodeAsync(
            treeView,
            _descriptor,
            cancellationToken).ConfigureAwait(true);
    }
}
