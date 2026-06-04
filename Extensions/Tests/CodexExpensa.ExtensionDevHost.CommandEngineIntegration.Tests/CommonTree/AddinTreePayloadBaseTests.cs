using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.CommonTree;

public sealed class AddinTreePayloadBaseTests
{
    [Fact]
    public void PayloadJson_DoesNotSerializeItselfRecursively()
    {
        TestPayload payload =
            new(
                AddinTreeNodeType.Item,
                "test-node",
                "Test Node")
            {
                ExtraValue = "Extra"
            };

        string json =
            payload.PayloadJson;

        Assert.Contains("\"addinName\"", json);
        Assert.Contains("\"nodeType\"", json);
        Assert.Contains("\"nodeId\"", json);
        Assert.Contains("\"displayText\"", json);
        Assert.Contains("\"extraValue\"", json);
        Assert.DoesNotContain("\"payloadJson\"", json, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_RejectsBlankIdentityValues()
    {
        Assert.Throws<ArgumentException>(() => new TestPayload(AddinTreeNodeType.Item, "", "Display"));
        Assert.Throws<ArgumentException>(() => new TestPayload(AddinTreeNodeType.Item, "id", ""));
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

        public string ExtraValue { get; init; } = string.Empty;

        protected override object ToPayloadSnapshot()
        {
            return new
            {
                AddinName,
                NodeType,
                NodeId,
                DisplayText,
                ExtraValue
            };
        }
    }
}
