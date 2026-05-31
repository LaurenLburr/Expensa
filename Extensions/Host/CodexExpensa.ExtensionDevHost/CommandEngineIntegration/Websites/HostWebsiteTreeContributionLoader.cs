using Codex.CommandEngine.Core;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class HostWebsiteTreeContributionLoader
{
    private readonly HostWebsiteRuntimeModuleInvoker _invoker;

    public HostWebsiteTreeContributionLoader()
        : this(new HostWebsiteRuntimeModuleInvoker())
    {
    }

    public HostWebsiteTreeContributionLoader(
        HostWebsiteRuntimeModuleInvoker invoker)
    {
        ArgumentNullException.ThrowIfNull(invoker);

        _invoker = invoker;
    }

    public async Task<HostWebsiteTreeLoadResult> LoadContributionAsync(
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

        HostWebsiteLoadResult loadResult =
            HostWebsiteLoadExecutionResultAdapter.FromExecutionResult(
                executionResult);

        HostWebsiteTreeViewRenderer.Render(
            treeView,
            loadResult);

        if (options.ExpandAll)
        {
            treeView.ExpandAll();
        }

        return new HostWebsiteTreeLoadResult
        {
            ExecutionResult = executionResult,
            WebsiteResult = loadResult
        };
    }
}
