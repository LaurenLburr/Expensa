using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.CommonTree;

public sealed class AddinTreeProviderBaseTests
{
    [Fact]
    public async Task FillAsync_DefaultReturnsSuccess()
    {
        TestProvider provider =
            new();

        TestPayload payload =
            new(AddinTreeNodeType.Item, "node-1", "Node 1");

        AddinTreeOperationResult result =
            await provider.FillAsync(payload);

        Assert.True(result.Succeeded);
        Assert.Equal("node-1", result.NodeId);
        Assert.Contains("loaded", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ModifyAsync_DefaultReturnsFailure()
    {
        TestProvider provider =
            new();

        TestPayload payload =
            new(AddinTreeNodeType.Item, "node-1", "Node 1");

        AddinTreeOperationResult result =
            await provider.ModifyAsync(payload);

        Assert.False(result.Succeeded);
        Assert.Equal("node-1", result.NodeId);
        Assert.Contains("does not support modify", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeleteAsync_DefaultReturnsFailure()
    {
        TestProvider provider =
            new();

        TestPayload payload =
            new(AddinTreeNodeType.Item, "node-1", "Node 1");

        AddinTreeOperationResult result =
            await provider.DeleteAsync(payload);

        Assert.False(result.Succeeded);
        Assert.Equal("node-1", result.NodeId);
        Assert.Contains("does not support delete", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task LoadAsync_CanBeImplementedByDerivedProvider()
    {
        TestProvider provider =
            new();

        IReadOnlyList<AddinTreeNode<TestPayload>> nodes =
            await provider.LoadAsync(
                new AddinTreeLoadRequest());

        AddinTreeNode<TestPayload> node =
            Assert.Single(nodes);

        Assert.Equal("node-1", node.Payload.NodeId);
    }

    private sealed class TestProvider : AddinTreeProviderBase<TestPayload>
    {
        public TestProvider()
            : base("TestAddin")
        {
        }

        public override Task<IReadOnlyList<AddinTreeNode<TestPayload>>> LoadAsync(
            AddinTreeLoadRequest request,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<AddinTreeNode<TestPayload>> nodes =
            [
                new AddinTreeNode<TestPayload>
                {
                    Payload = new TestPayload(AddinTreeNodeType.Item, "node-1", "Node 1")
                }
            ];

            return Task.FromResult(nodes);
        }
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
