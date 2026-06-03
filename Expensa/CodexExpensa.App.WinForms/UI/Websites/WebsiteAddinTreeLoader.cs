namespace CodexExpensa.App.WinForms.UI.Websites;

public sealed class WebsiteAddinTreeLoader
{
    private readonly WebsiteAddinRuntimeInvoker _invoker;

    public WebsiteAddinTreeLoader()
        : this(new WebsiteAddinRuntimeInvoker())
    {
    }

    public WebsiteAddinTreeLoader(
        WebsiteAddinRuntimeInvoker invoker)
    {
        ArgumentNullException.ThrowIfNull(invoker);

        _invoker = invoker;
    }

    public async Task<WebsiteTreeLoadResult> LoadIntoTreeViewAsync(
        TreeView treeView,
        WebsiteTreeLoadOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(treeView);
        ArgumentNullException.ThrowIfNull(options);

        WebsiteRuntimeExecutionResult executionResult =
            await _invoker.ExecuteAsync(options, cancellationToken).ConfigureAwait(true);

        if (!executionResult.Succeeded)
        {
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(executionResult.Message)
                    ? $"Website add-in failed with status: {executionResult.Status}"
                    : executionResult.Message);
        }

        WebsiteLoadResult websiteResult =
            WebsiteLoadResultParser.Parse(executionResult.OutputJson);

        WebsiteTreeViewRenderer.Render(
            treeView,
            websiteResult);

        if (options.ExpandAll)
        {
            treeView.ExpandAll();
        }

        return new WebsiteTreeLoadResult
        {
            Nodes = websiteResult.Nodes,
            TotalCount = websiteResult.TotalCount,
            Message = websiteResult.Message,
            ExecutionStatus = executionResult.Status,
            OutputJson = executionResult.OutputJson
        };
    }
}
