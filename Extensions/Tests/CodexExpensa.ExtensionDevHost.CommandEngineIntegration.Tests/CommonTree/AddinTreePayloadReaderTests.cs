using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.CommonTree;

public sealed class AddinTreePayloadReaderTests
{
    [Fact]
    public void ReadPayload_ReturnsPayloadWhenTagImplementsInterface()
    {
        TestPayload payload =
            new(AddinTreeNodeType.Item, "node-1", "Node 1");

        TreeNode node =
            new("Node 1")
            {
                Tag = payload
            };

        IAddinTreePayload? result =
            AddinTreePayloadReader.ReadPayload(node);

        Assert.Same(payload, result);
    }

    [Fact]
    public void ReadPayload_ReturnsNullForNonPayloadTag()
    {
        TreeNode node =
            new("Node")
            {
                Tag = "not a payload"
            };

        Assert.Null(
            AddinTreePayloadReader.ReadPayload(node));
    }

    [Fact]
    public void IsAddinNode_MatchesAddinNameIgnoringCase()
    {
        TreeNode node =
            new("Node")
            {
                Tag = new TestPayload(AddinTreeNodeType.Item, "node-1", "Node 1")
            };

        Assert.True(
            AddinTreePayloadReader.IsAddinNode(node, "testaddin"));

        Assert.False(
            AddinTreePayloadReader.IsAddinNode(node, "OtherAddin"));
    }

    [Fact]
    public void HasNodeType_MatchesPayloadNodeType()
    {
        TreeNode node =
            new("Node")
            {
                Tag = new TestPayload(AddinTreeNodeType.BudgetMonth, "node-1", "Node 1")
            };

        Assert.True(
            AddinTreePayloadReader.HasNodeType(node, AddinTreeNodeType.BudgetMonth));

        Assert.False(
            AddinTreePayloadReader.HasNodeType(node, AddinTreeNodeType.Website));
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
