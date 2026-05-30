namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public interface ITreeContributionCommandAdapter
{
    string CommandName { get; }

    Task LoadNodeAsync(
        TreeView treeView,
        ExtensionTreeContributionDescriptor descriptor,
        CancellationToken cancellationToken = default);
}
