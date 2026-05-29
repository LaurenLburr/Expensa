using System.Windows.Forms;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class ExtensionManagerAddinTestTreeBuilderTests
{
    [Fact]
    public void Populate_SortsAddinsAndAddsLoadTestFormChild()
    {
        using TreeView treeView = new();

        ExtensionManagerAddinTestTreeBuilder.Populate(
            treeView,
            [
                new ExtensionManagerAddinTestNode
                {
                    AddinId = "second",
                    DisplayName = "Second",
                    SortOrder = 200,
                    TestFormName = "Second Test"
                },
                new ExtensionManagerAddinTestNode
                {
                    AddinId = "first",
                    DisplayName = "First",
                    SortOrder = 100,
                    TestFormName = "First Test"
                }
            ]);

        Assert.Single(treeView.Nodes);
        TreeNode root = treeView.Nodes[0];
        Assert.Equal("Add-ins", root.Text);
        Assert.Equal("first", root.Nodes[0].Name);
        Assert.Single(root.Nodes[0].Nodes);
        Assert.IsType<ExtensionManagerAddinTestAction>(root.Nodes[0].Nodes[0].Tag);
    }
}
