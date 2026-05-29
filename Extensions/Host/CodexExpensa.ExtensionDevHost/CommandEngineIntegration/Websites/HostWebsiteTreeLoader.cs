using Codex.CommandEngine.Core;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class HostWebsiteTreeLoader : IHostWebsiteTreeLoader
{
    private readonly HostWebsiteRuntimeModuleInvoker _invoker;

    public HostWebsiteTreeLoader()
        : this(new HostWebsiteRuntimeModuleInvoker())
    {
    }

    public HostWebsiteTreeLoader(
        HostWebsiteRuntimeModuleInvoker invoker)
    {
        ArgumentNullException.ThrowIfNull(invoker);

        _invoker = invoker;
    }

    public async Task<HostWebsiteTreeLoadResult> LoadIntoTreeViewAsync(
        TreeView treeView,
        HostWebsiteTreeLoadOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(treeView);
        ArgumentNullException.ThrowIfNull(options);

        CommandExecutionResult executionResult =
            await _invoker.ExecuteAsync(
                new HostWebsiteRuntimeLoadRequest
                {
                    SearchText = options.SearchText,
                    IncludeDisabled = options.IncludeDisabled,
                    MaximumRows = options.MaximumRows
                },
                cancellationToken).ConfigureAwait(true);

        HostWebsiteLoadResult websiteResult =
            HostWebsiteLoadExecutionResultAdapter.FromExecutionResult(executionResult);

        HostWebsiteTreeViewRenderer.Render(
            treeView,
            websiteResult);

        if (options.ExpandAll)
        {
            treeView.ExpandAll();
        }

        return new HostWebsiteTreeLoadResult
        {
            ExecutionResult = executionResult,
            WebsiteResult = websiteResult
        };
    }
}
