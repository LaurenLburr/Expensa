using System.Windows.Forms;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class ExtensionManagerAddinTestTreeBuilderTests
{
    [Fact]
    public void Populate_SortsAddinsAndAddsDatabaseAndTestChildren()
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
                    DatabaseDisplayName = "Second Database",
                    TestFormName = "Second Test"
                },
                new ExtensionManagerAddinTestNode
                {
                    AddinId = "first",
                    DisplayName = "First",
                    SortOrder = 100,
                    DatabaseDisplayName = "First Database",
                    TestFormName = "First Test"
                }
            ]);

        TreeNode root = treeView.Nodes[0];

        Assert.Equal("first", root.Nodes[0].Name);
        Assert.Equal(2, root.Nodes[0].Nodes.Count);
        Assert.Equal("first.database", root.Nodes[0].Nodes[0].Name);
        Assert.Equal("first.test", root.Nodes[0].Nodes[1].Name);
    }

    [Fact]
    public void Populate_DatabaseAndTestChildrenHaveExpectedActionKinds()
    {
        using TreeView treeView = new();

        ExtensionManagerAddinTestTreeBuilder.Populate(
            treeView,
            [
                new ExtensionManagerAddinTestNode
                {
                    AddinId = "websites",
                    DisplayName = "Websites",
                    SortOrder = 100,
                    DatabaseDisplayName = "Websites Database",
                    TestFormName = "Websites Tree Test"
                }
            ]);

        ExtensionManagerAddinTestAction databaseAction =
            Assert.IsType<ExtensionManagerAddinTestAction>(
                treeView.Nodes[0].Nodes[0].Nodes[0].Tag);

        ExtensionManagerAddinTestAction testAction =
            Assert.IsType<ExtensionManagerAddinTestAction>(
                treeView.Nodes[0].Nodes[0].Nodes[1].Tag);

        Assert.Equal(ExtensionManagerAddinTestActionKind.Database, databaseAction.ActionKind);
        Assert.Equal(ExtensionManagerAddinTestActionKind.Test, testAction.ActionKind);
    }
}
