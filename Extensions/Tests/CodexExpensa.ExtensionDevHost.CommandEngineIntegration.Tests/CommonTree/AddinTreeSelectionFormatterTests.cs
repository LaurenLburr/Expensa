using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.CommonTree;

public sealed class AddinTreeSelectionFormatterTests
{
    [Fact]
    public void FormatSelectedNode_ReturnsReadablePayloadDetails()
    {
        using TreeView treeView =
            new();

        TestPayload payload =
            new(AddinTreeNodeType.Item, "node-1", "Node 1");

        TreeNode node =
            new("Node 1")
            {
                Name = "node-1",
                Tag = payload
            };

        treeView.Nodes.Add(node);
        treeView.SelectedNode = node;

        string text =
            AddinTreeSelectionFormatter.FormatSelectedNode(treeView);

        Assert.Contains("Add-in: TestAddin", text);
        Assert.Contains("NodeType: Item", text);
        Assert.Contains("NodeId: node-1", text);
        Assert.Contains("DisplayText: Node 1", text);
        Assert.Contains("PayloadJson:", text);
    }

    [Fact]
    public void FormatSelectedNode_ReturnsEmptyForMissingPayload()
    {
        using TreeView treeView =
            new();

        TreeNode node =
            new("Node 1")
            {
                Name = "node-1",
                Tag = "not payload"
            };

        treeView.Nodes.Add(node);
        treeView.SelectedNode = node;

        string text =
            AddinTreeSelectionFormatter.FormatSelectedNode(treeView);

        Assert.Equal(string.Empty, text);
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
