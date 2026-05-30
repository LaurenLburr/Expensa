using System.Windows.Forms;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class ModuleCommandTreeNodeLoaderAdapterTests
{
    [Fact]
    public async Task LoadNodeAsync_UsesRegisteredAdapter()
    {
        TestAdapter adapter = new();

        TreeContributionCommandAdapterRegistry registry =
            new([adapter]);

        ModuleCommandTreeNodeLoader loader =
            new(
                new ExtensionTreeContributionDescriptor
                {
                    AddinId = "test",
                    DisplayName = "Test",
                    SortOrder = 10,
                    CommandName = "test.load",
                    RootNodeName = "test",
                    RootDisplayText = "Test"
                },
                registry);

        using TreeView treeView = new();

        await loader.LoadNodeAsync(treeView);

        Assert.True(adapter.WasCalled);
    }

    [Fact]
    public async Task LoadNodeAsync_WhenNoAdapter_Throws()
    {
        ModuleCommandTreeNodeLoader loader =
            new(
                new ExtensionTreeContributionDescriptor
                {
                    AddinId = "test",
                    DisplayName = "Test",
                    SortOrder = 10,
                    CommandName = "missing.load",
                    RootNodeName = "test",
                    RootDisplayText = "Test"
                },
                new TreeContributionCommandAdapterRegistry([]));

        using TreeView treeView = new();

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await loader.LoadNodeAsync(treeView));
    }

    private sealed class TestAdapter : ITreeContributionCommandAdapter
    {
        public string CommandName => "test.load";

        public bool WasCalled { get; private set; }

        public Task LoadNodeAsync(
            TreeView treeView,
            ExtensionTreeContributionDescriptor descriptor,
            CancellationToken cancellationToken = default)
        {
            WasCalled = true;

            treeView.Nodes.Add(new TreeNode
            {
                Name = descriptor.RootNodeName,
                Text = descriptor.RootDisplayText
            });

            return Task.CompletedTask;
        }
    }
}
