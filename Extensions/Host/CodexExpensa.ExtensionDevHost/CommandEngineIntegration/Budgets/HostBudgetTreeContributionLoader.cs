namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed class HostBudgetTreeContributionLoader
{
    private readonly HostBudgetRuntimeModuleInvoker invoker;

    public HostBudgetTreeContributionLoader()
        : this(new HostBudgetRuntimeModuleInvoker())
    {
    }

    public HostBudgetTreeContributionLoader(HostBudgetRuntimeModuleInvoker invoker)
    {
        ArgumentNullException.ThrowIfNull(invoker);
        this.invoker = invoker;
    }

    public async Task<HostBudgetTreeLoadResult> LoadContributionAsync(
        TreeView treeView,
        HostBudgetTreeLoadOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(treeView);
        ArgumentNullException.ThrowIfNull(options);

        Codex.CommandEngine.Core.CommandExecutionResult executionResult =
            await invoker.ExecuteAsync(
                new HostBudgetRuntimeLoadRequest
                {
                    SearchText = options.SearchText,
                    IncludeClosed = options.IncludeClosed,
                    MaximumRows = options.MaximumRows
                },
                cancellationToken).ConfigureAwait(true);

        int rootNodeCount =
            HostBudgetTreeViewRenderer.Render(treeView, executionResult.OutputJson, options.ExpandAll);

        return new HostBudgetTreeLoadResult
        {
            ExecutionResult = executionResult,
            RootNodeCount = rootNodeCount
        };
    }
}
