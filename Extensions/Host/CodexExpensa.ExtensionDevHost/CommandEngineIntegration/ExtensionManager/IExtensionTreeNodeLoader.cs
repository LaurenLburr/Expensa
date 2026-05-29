namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public interface IExtensionTreeNodeLoader
{
    string AddinId { get; }

    string DisplayName { get; }

    int SortOrder { get; }

    Task LoadNodeAsync(
        TreeView treeView,
        CancellationToken cancellationToken = default);
}
