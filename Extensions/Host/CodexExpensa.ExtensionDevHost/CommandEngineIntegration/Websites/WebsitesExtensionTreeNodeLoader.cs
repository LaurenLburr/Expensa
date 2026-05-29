using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class WebsitesExtensionTreeNodeLoader : IExtensionTreeNodeLoader
{
    private readonly HostWebsiteTreeContributionLoader _contributionLoader;

    public WebsitesExtensionTreeNodeLoader()
        : this(new HostWebsiteTreeContributionLoader())
    {
    }

    public WebsitesExtensionTreeNodeLoader(
        HostWebsiteTreeContributionLoader contributionLoader)
    {
        ArgumentNullException.ThrowIfNull(contributionLoader);

        _contributionLoader = contributionLoader;
    }

    public string AddinId => "websites";

    public string DisplayName => "Websites";

    public int SortOrder => 100;

    public async Task LoadNodeAsync(
        TreeView treeView,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(treeView);

        await _contributionLoader.LoadContributionAsync(
            treeView,
            new HostWebsiteTreeLoadOptions
            {
                SearchText = string.Empty,
                IncludeDisabled = false,
                MaximumRows = 500,
                ExpandAll = false
            },
            cancellationToken).ConfigureAwait(true);
    }
}
