namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed class ExtensionTreeLoadOrchestrator
{
    private readonly IReadOnlyList<IExtensionTreeNodeLoader> _loaders;

    public ExtensionTreeLoadOrchestrator(
        IReadOnlyList<IExtensionTreeNodeLoader> loaders)
    {
        ArgumentNullException.ThrowIfNull(loaders);

        _loaders = loaders
            .OrderBy(static loader => loader.SortOrder)
            .ToList();
    }

    public async Task<ExtensionTreeLoadSummary> LoadAsync(
        TreeView treeView,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(treeView);

        List<ExtensionTreeLoadResult> results = [];

        treeView.BeginUpdate();

        try
        {
            treeView.Nodes.Clear();

            foreach (IExtensionTreeNodeLoader loader in _loaders)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    await loader.LoadNodeAsync(treeView, cancellationToken).ConfigureAwait(true);

                    results.Add(new ExtensionTreeLoadResult
                    {
                        AddinId = loader.AddinId,
                        DisplayName = loader.DisplayName,
                        SortOrder = loader.SortOrder,
                        Succeeded = true,
                        Message = "Loaded."
                    });
                }
                catch (Exception exception)
                {
                    results.Add(new ExtensionTreeLoadResult
                    {
                        AddinId = loader.AddinId,
                        DisplayName = loader.DisplayName,
                        SortOrder = loader.SortOrder,
                        Succeeded = false,
                        Message = exception.Message
                    });
                }
            }
        }
        finally
        {
            treeView.EndUpdate();
        }

        return new ExtensionTreeLoadSummary
        {
            Results = results
        };
    }
}
