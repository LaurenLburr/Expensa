using System.Windows.Forms;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class TreeViewExpansionStateServiceTests
{
    [Fact]
    public void CaptureExpandedNodeNames_ReturnsExpandedNamedNodes()
    {
        using TreeView treeView = new();

        TreeNode root = new()
        {
            Name = "root",
            Text = "Root"
        };

        root.Nodes.Add(new TreeNode
        {
            Name = "child",
            Text = "Child"
        });

        treeView.Nodes.Add(root);
        root.Expand();

        IReadOnlySet<string> expanded =
            TreeViewExpansionStateService.CaptureExpandedNodeNames(treeView);

        Assert.Contains("root", expanded);
        Assert.DoesNotContain("child", expanded);
    }

    [Fact]
    public void RestoreExpandedNodeNames_RestoresMatchingNodesOnly()
    {
        using TreeView treeView = new();

        TreeNode root = new()
        {
            Name = "root",
            Text = "Root"
        };

        TreeNode child = new()
        {
            Name = "child",
            Text = "Child"
        };

        root.Nodes.Add(child);
        treeView.Nodes.Add(root);

        TreeViewExpansionStateService.RestoreExpandedNodeNames(
            treeView,
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "root"
            });

        Assert.True(root.IsExpanded);
        Assert.False(child.IsExpanded);
    }
}
