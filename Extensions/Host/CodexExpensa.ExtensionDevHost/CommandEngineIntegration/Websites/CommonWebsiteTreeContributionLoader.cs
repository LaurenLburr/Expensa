using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class CommonWebsiteTreeContributionLoader
{
    private readonly WebsiteTreeProvider provider;

    public CommonWebsiteTreeContributionLoader()
        : this(new WebsiteTreeProvider())
    {
    }

    public CommonWebsiteTreeContributionLoader(WebsiteTreeProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);
        this.provider = provider;
    }

    public async Task<int> LoadContributionAsync(
        TreeView treeView,
        HostWebsiteTreeLoadOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(treeView);
        ArgumentNullException.ThrowIfNull(options);

        IReadOnlyList<AddinTreeNode<WebsiteTreePayload>> nodes =
            await provider.LoadAsync(
                new AddinTreeLoadRequest
                {
                    SearchText = options.SearchText,
                    IncludeInactive = options.IncludeDisabled,
                    MaximumRows = options.MaximumRows,
                    ExpandAll = options.ExpandAll
                },
                cancellationToken).ConfigureAwait(true);

        AddinTreeNode<WebsiteTreePayload> rootNode =
            new()
            {
                Payload = new WebsiteTreePayload(
                    AddinTreeNodeType.Root,
                    "websites",
                    "Websites"),
                Children = nodes
            };

        return AddinTreeRenderer.Render(treeView, [rootNode], options.ExpandAll);
    }
}
