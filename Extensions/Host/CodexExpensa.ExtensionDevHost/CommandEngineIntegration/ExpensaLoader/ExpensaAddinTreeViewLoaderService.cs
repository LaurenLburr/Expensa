using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExpensaLoader;

public sealed class ExpensaAddinTreeViewLoaderService
{
    private readonly ExpensaStyleAddinRuntimeInvoker invoker;

    public ExpensaAddinTreeViewLoaderService()
        : this(new ExpensaStyleAddinRuntimeInvoker())
    {
    }

    public ExpensaAddinTreeViewLoaderService(
        ExpensaStyleAddinRuntimeInvoker invoker)
    {
        ArgumentNullException.ThrowIfNull(invoker);

        this.invoker = invoker;
    }

    public async Task<IReadOnlyList<ExpensaAddinLoaderResult>> LoadIntoTreeViewAsync(
        TreeView treeView,
        ExpensaAddinLoaderRequest request,
        bool expandAll,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(treeView);
        ArgumentNullException.ThrowIfNull(request);

        ExpensaAddinLoaderResult result =
            await invoker.ExecuteAsync(
                request,
                cancellationToken).ConfigureAwait(true);

        AddinTreeNode<ExpensaAddinTreePayload> root =
            CreateAggregateRoot(
                [ExpensaAddinTreeOutputParser.ParseResultAsRoot(request.Kind, result)]);

        AddinTreeRenderer.Render(
            treeView,
            [root],
            expandAll);

        return [result];
    }

    public async Task<IReadOnlyList<ExpensaAddinLoaderResult>> LoadBothIntoTreeViewAsync(
        TreeView treeView,
        string databasePath,
        string searchText,
        bool includeInactive,
        int maximumRows,
        bool expandAll,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(treeView);

        List<ExpensaAddinLoaderResult> results = [];
        List<AddinTreeNode<ExpensaAddinTreePayload>> addinRoots = [];

        foreach (ExpensaAddinLoaderKind kind in GetDefaultLoadOrder())
        {
            ExpensaAddinLoaderRequest request =
                new()
                {
                    Kind = kind,
                    DatabasePath = databasePath,
                    SearchText = searchText,
                    IncludeInactive = includeInactive,
                    MaximumRows = maximumRows
                };

            ExpensaAddinLoaderResult result =
                await invoker.ExecuteAsync(
                    request,
                    cancellationToken).ConfigureAwait(true);

            results.Add(result);

            addinRoots.Add(
                ExpensaAddinTreeOutputParser.ParseResultAsRoot(
                    kind,
                    result));
        }

        AddinTreeNode<ExpensaAddinTreePayload> aggregateRoot =
            CreateAggregateRoot(
                addinRoots);

        AddinTreeRenderer.Render(
            treeView,
            [aggregateRoot],
            expandAll);

        return results;
    }

    public async Task<IReadOnlyList<ExpensaAddinLoaderResult>> LoadAllIntoTreeViewAsync(
        TreeView treeView,
        ExpensaAddinLoaderRequestTemplate requestTemplate,
        bool expandAll,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(treeView);
        ArgumentNullException.ThrowIfNull(requestTemplate);

        return await LoadBothIntoTreeViewAsync(
            treeView,
            requestTemplate.DatabasePath,
            requestTemplate.SearchText,
            requestTemplate.IncludeInactive,
            requestTemplate.MaximumRows,
            expandAll,
            cancellationToken).ConfigureAwait(true);
    }

    private static AddinTreeNode<ExpensaAddinTreePayload> CreateAggregateRoot(
        IReadOnlyList<AddinTreeNode<ExpensaAddinTreePayload>> addinRoots)
    {
        return new AddinTreeNode<ExpensaAddinTreePayload>
        {
            Payload = new ExpensaAddinTreePayload(
                "Expensa",
                AddinTreeNodeType.Root,
                "expensa-addins",
                "Expensa Add-ins"),
            Children = addinRoots
        };
    }

    private static IReadOnlyList<ExpensaAddinLoaderKind> GetDefaultLoadOrder()
    {
        return
        [
            ExpensaAddinLoaderKind.Websites,
            ExpensaAddinLoaderKind.Budgets,
            ExpensaAddinLoaderKind.Payees
        ];
    }
}
