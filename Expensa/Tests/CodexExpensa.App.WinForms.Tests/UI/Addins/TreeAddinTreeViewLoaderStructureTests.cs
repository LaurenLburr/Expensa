using Xunit;

namespace CodexExpensa.App.WinForms.Tests.UI.Addins;

public sealed class TreeAddinTreeViewLoaderStructureTests
{
    [Fact]
    public void TreeViewLoader_LoadsEveryDefaultTreeAddin()
    {
        string text = TestRepositoryPath.ReadExpensaFile("UI", "Addins", "TreeAddinTreeViewLoader.cs");

        Assert.Contains("TreeAddinDefinition.DefaultTreeAddins()", text);
        Assert.Contains("foreach (TreeAddinDefinition definition", text);
        Assert.Contains("invoker.ExecuteAsync", text);
        Assert.Contains("TreeAddinTreeNodeFactory.CreateRootNode", text);
    }

    [Fact]
    public void TreeViewLoader_ClearsTreeOnceAndAddsAllRoots()
    {
        string text = TestRepositoryPath.ReadExpensaFile("UI", "Addins", "TreeAddinTreeViewLoader.cs");

        Assert.Contains("treeView.BeginUpdate()", text);
        Assert.Contains("treeView.Nodes.Clear()", text);
        Assert.Contains("foreach (TreeNode node in rootNodes)", text);
        Assert.Contains("treeView.Nodes.Add(node)", text);
        Assert.Contains("treeView.EndUpdate()", text);
    }

    [Fact]
    public void TreeViewLoader_ReturnsAggregateLoadResult()
    {
        string text = TestRepositoryPath.ReadExpensaFile("UI", "Addins", "TreeAddinTreeViewLoader.cs");

        Assert.Contains("TreeAddinAggregateLoadResult", text);
        Assert.Contains("AddinResults = results", text);
        Assert.Contains("RootNodeCount = treeView.Nodes.Count", text);
    }
}
