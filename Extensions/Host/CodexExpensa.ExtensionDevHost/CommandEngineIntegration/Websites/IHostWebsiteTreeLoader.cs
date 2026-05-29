namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public interface IHostWebsiteTreeLoader
{
    Task<HostWebsiteTreeLoadResult> LoadIntoTreeViewAsync(
        TreeView treeView,
        HostWebsiteTreeLoadOptions options,
        CancellationToken cancellationToken = default);
}
