using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed class CommonBudgetTreeContributionLoader
{
    private readonly BudgetTreeProvider provider;

    public CommonBudgetTreeContributionLoader()
        : this(new BudgetTreeProvider())
    {
    }

    public CommonBudgetTreeContributionLoader(
        BudgetTreeProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);

        this.provider = provider;
    }

    public async Task<int> LoadContributionAsync(
        TreeView treeView,
        HostBudgetTreeLoadOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(treeView);
        ArgumentNullException.ThrowIfNull(options);

        IReadOnlyList<AddinTreeNode<BudgetTreePayload>> nodes =
            await provider.LoadAsync(
                new AddinTreeLoadRequest
                {
                    SearchText = options.SearchText,
                    IncludeInactive = options.IncludeClosed,
                    MaximumRows = options.MaximumRows,
                    ExpandAll = options.ExpandAll
                },
                cancellationToken).ConfigureAwait(true);

        AddinTreeNode<BudgetTreePayload> rootNode =
            new()
            {
                Payload = new BudgetTreePayload(
                    AddinTreeNodeType.Root,
                    "budgets",
                    "Budgets"),
                Children = nodes
            };

        return AddinTreeRenderer.Render(
            treeView,
            [rootNode],
            options.ExpandAll);
    }
}
