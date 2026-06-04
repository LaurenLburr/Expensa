using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.CommonTree;

public sealed class AddinTreeRendererTests
{
    [Fact]
    public void CreateTreeNode_PutsPayloadOnTagAndRendersChildren()
    {
        TestPayload parentPayload =
            new(AddinTreeNodeType.Root, "root", "Root");

        TestPayload childPayload =
            new(AddinTreeNodeType.Item, "child", "Child");

        AddinTreeNode<TestPayload> sourceNode =
            new()
            {
                Payload = parentPayload,
                Children =
                [
                    new AddinTreeNode<TestPayload>
                    {
                        Payload = childPayload
                    }
                ]
            };

        TreeNode treeNode =
            AddinTreeRenderer.CreateTreeNode(sourceNode);

        Assert.Equal("root", treeNode.Name);
        Assert.Equal("Root", treeNode.Text);
        Assert.Same(parentPayload, treeNode.Tag);

        TreeNode childNode =
            Assert.Single(treeNode.Nodes.Cast<TreeNode>());

        Assert.Equal("child", childNode.Name);
        Assert.Equal("Child", childNode.Text);
        Assert.Same(childPayload, childNode.Tag);
    }

    [Fact]
    public void Render_ClearsExistingNodesAndAddsNewRootNodes()
    {
        using TreeView treeView =
            new();

        treeView.Nodes.Add("old");

        AddinTreeNode<TestPayload> sourceNode =
            new()
            {
                Payload = new TestPayload(AddinTreeNodeType.Root, "root", "Root")
            };

        int rootCount =
            AddinTreeRenderer.Render(
                treeView,
                [sourceNode],
                expandAll: false);

        Assert.Equal(1, rootCount);
        Assert.Single(treeView.Nodes);
        Assert.Equal("root", treeView.Nodes[0].Name);
    }

    private sealed class TestPayload : AddinTreePayloadBase
    {
        public TestPayload(
            AddinTreeNodeType nodeType,
            string nodeId,
            string displayText)
            : base("TestAddin", nodeType, nodeId, displayText)
        {
        }
    }
}
