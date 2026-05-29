using System.Windows.Forms;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class ExtensionTreeLoadOrchestratorTests
{
    [Fact]
    public async Task LoadAsync_RunsLoadersInSortOrder()
    {
        using TreeView treeView = new();

        ExtensionTreeLoadOrchestrator orchestrator =
            new(
                [
                    new TestLoader("second", "Second", 200),
                    new TestLoader("first", "First", 100)
                ]);

        ExtensionTreeLoadSummary summary =
            await orchestrator.LoadAsync(treeView);

        Assert.Equal(2, treeView.Nodes.Count);
        Assert.Equal("first", treeView.Nodes[0].Name);
        Assert.Equal("second", treeView.Nodes[1].Name);
        Assert.Equal(2, summary.SucceededCount);
    }

    private sealed class TestLoader : IExtensionTreeNodeLoader
    {
        public TestLoader(string addinId, string displayName, int sortOrder)
        {
            AddinId = addinId;
            DisplayName = displayName;
            SortOrder = sortOrder;
        }

        public string AddinId { get; }

        public string DisplayName { get; }

        public int SortOrder { get; }

        public Task LoadNodeAsync(TreeView treeView, CancellationToken cancellationToken = default)
        {
            treeView.Nodes.Add(new TreeNode
            {
                Name = AddinId,
                Text = DisplayName
            });

            return Task.CompletedTask;
        }
    }
}
