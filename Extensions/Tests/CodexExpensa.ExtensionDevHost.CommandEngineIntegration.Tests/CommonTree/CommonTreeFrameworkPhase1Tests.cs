using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.CommonTree;

public sealed class CommonTreeFrameworkPhase1Tests
{
    [Fact]
    public void CommonPayloadInterface_ExposesDefinitiveNodeType()
    {
        Assert.NotNull(typeof(IAddinTreePayload).GetProperty("AddinName"));
        Assert.NotNull(typeof(IAddinTreePayload).GetProperty("NodeType"));
        Assert.NotNull(typeof(IAddinTreePayload).GetProperty("NodeId"));
        Assert.NotNull(typeof(IAddinTreePayload).GetProperty("DisplayText"));
        Assert.NotNull(typeof(IAddinTreePayload).GetProperty("PayloadJson"));
    }

    [Fact]
    public void WebsiteAndBudgetPayloads_ShareCommonBase()
    {
        Assert.True(typeof(AddinTreePayloadBase).IsAssignableFrom(typeof(WebsiteTreePayload)));
        Assert.True(typeof(AddinTreePayloadBase).IsAssignableFrom(typeof(BudgetTreePayload)));
    }

    [Fact]
    public void Renderer_PlacesPayloadOnTreeNodeTag()
    {
        WebsiteTreePayload payload =
            new(AddinTreeNodeType.Website, "website-1", "Website")
            {
                WebsiteId = "website-1",
                Url = "https://example.test",
                TagName = "Testing"
            };

        AddinTreeNode<WebsiteTreePayload> sourceNode =
            new()
            {
                Payload = payload
            };

        TreeNode treeNode =
            AddinTreeRenderer.CreateTreeNode(sourceNode);

        IAddinTreePayload? readPayload =
            AddinTreePayloadReader.ReadPayload(treeNode);

        Assert.NotNull(readPayload);
        Assert.Equal("WebsitesAddin", readPayload.AddinName);
        Assert.Equal(AddinTreeNodeType.Website, readPayload.NodeType);
        Assert.Equal("website-1", readPayload.NodeId);
    }

    [Fact]
    public async Task ProviderBase_DefaultModifyAndDeleteReturnUnsupported()
    {
        TestProvider provider = new();

        BudgetTreePayload payload =
            new(AddinTreeNodeType.BudgetMonth, "budget-month:2026-06", "June")
            {
                BudgetYear = 2026,
                BudgetMonth = 6,
                MonthKey = "2026-06"
            };

        AddinTreeOperationResult modifyResult =
            await provider.ModifyAsync(payload);

        AddinTreeOperationResult deleteResult =
            await provider.DeleteAsync(payload);

        Assert.False(modifyResult.Succeeded);
        Assert.False(deleteResult.Succeeded);
        Assert.Contains("does not support modify yet", modifyResult.Message);
        Assert.Contains("does not support delete yet", deleteResult.Message);
    }

    private sealed class TestProvider : AddinTreeProviderBase<BudgetTreePayload>
    {
        public TestProvider()
            : base("BudgetsAddin")
        {
        }

        public override Task<IReadOnlyList<AddinTreeNode<BudgetTreePayload>>> LoadAsync(
            AddinTreeLoadRequest request,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<AddinTreeNode<BudgetTreePayload>>>([]);
        }
    }
}
