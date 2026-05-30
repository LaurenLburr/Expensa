using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class WebsitesTreeContributionCommandAdapter : ITreeContributionCommandAdapter
{
    private readonly HostWebsiteTreeContributionLoader _contributionLoader;

    public WebsitesTreeContributionCommandAdapter()
        : this(new HostWebsiteTreeContributionLoader())
    {
    }

    public WebsitesTreeContributionCommandAdapter(
        HostWebsiteTreeContributionLoader contributionLoader)
    {
        ArgumentNullException.ThrowIfNull(contributionLoader);

        _contributionLoader = contributionLoader;
    }

    public string CommandName =>
        "websites.load";

    public async Task LoadNodeAsync(
        TreeView treeView,
        ExtensionTreeContributionDescriptor descriptor,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(treeView);
        ArgumentNullException.ThrowIfNull(descriptor);

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
