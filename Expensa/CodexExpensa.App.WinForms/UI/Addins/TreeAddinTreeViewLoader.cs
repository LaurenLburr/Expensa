namespace CodexExpensa.App.WinForms.UI.Addins;

public sealed class TreeAddinTreeViewLoader
{
    private readonly TreeAddinRuntimeInvoker invoker;

    public TreeAddinTreeViewLoader()
        : this(new TreeAddinRuntimeInvoker())
    {
    }

    public TreeAddinTreeViewLoader(TreeAddinRuntimeInvoker invoker)
    {
        ArgumentNullException.ThrowIfNull(invoker);

        this.invoker = invoker;
    }

    public async Task<TreeAddinAggregateLoadResult> LoadAllIntoTreeViewAsync(
        TreeView treeView,
        TreeAddinLoadOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(treeView);
        ArgumentNullException.ThrowIfNull(options);

        List<TreeAddinRuntimeResult> results = [];
        List<TreeNode> rootNodes = [];

        foreach (TreeAddinDefinition definition in TreeAddinDefinition.DefaultTreeAddins())
        {
            try
            {
                TreeAddinRuntimeResult result =
                    await invoker.ExecuteAsync(
                        definition,
                        options,
                        cancellationToken).ConfigureAwait(true);

                results.Add(result);

                if (result.Succeeded)
                {
                    rootNodes.Add(
                        TreeAddinTreeNodeFactory.CreateRootNode(result));
                }
            }
            catch (Exception exception)
            {
                results.Add(
                    new TreeAddinRuntimeResult
                    {
                        Definition = definition,
                        Status = "Failed",
                        Message = exception.Message,
                        OutputJson = string.Empty
                    });

                rootNodes.Add(
                    TreeAddinTreeNodeFactory.CreateFailureNode(
                        definition,
                        exception));
            }
        }

        treeView.BeginUpdate();

        try
        {
            treeView.Nodes.Clear();

            foreach (TreeNode node in rootNodes)
            {
                treeView.Nodes.Add(node);
            }

            if (options.ExpandAll)
            {
                treeView.ExpandAll();
            }
        }
        finally
        {
            treeView.EndUpdate();
        }

        return new TreeAddinAggregateLoadResult
        {
            AddinResults = results,
            RootNodeCount = treeView.Nodes.Count
        };
    }
}
