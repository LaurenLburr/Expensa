using System.Windows.Forms;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class WebsiteTreeNodePayloadContractTests
{
    [Fact]
    public void Payloads_ImplementDefinitiveNodeTypeInterface()
    {
        Assert.True(
            typeof(IHostWebsiteTreeNodePayload).IsAssignableFrom(typeof(HostWebsiteTreeNodePayload)));

        Assert.True(
            typeof(IHostWebsiteTreeNodePayload).IsAssignableFrom(typeof(HostWebsiteCategoryGroupTreeNodePayload)));
    }

    [Fact]
    public void Reader_UsesPayloadInterfaceForNodeType()
    {
        TreeNode websiteNode =
            new("Website")
            {
                Tag = new HostWebsiteTreeNodePayload
                {
                    NodeId = "website-1",
                    WebsiteId = "website-1",
                    DisplayText = "Website",
                    Url = "https://example.test",
                    TagName = "Example",
                    IsActive = true
                }
            };

        TreeNode groupNode =
            new("Example")
            {
                Tag = new HostWebsiteCategoryGroupTreeNodePayload
                {
                    NodeId = "tag:Example",
                    DisplayText = "Example"
                }
            };

        Assert.Equal(
            HostWebsiteTreeNodeType.Website,
            HostWebsiteTreeNodeTagReader.GetNodeType(websiteNode));

        Assert.Equal(
            HostWebsiteTreeNodeType.CategoryGroup,
            HostWebsiteTreeNodeTagReader.GetNodeType(groupNode));

        Assert.True(
            HostWebsiteTreeNodeTagReader.IsWebsiteNode(websiteNode));

        Assert.False(
            HostWebsiteTreeNodeTagReader.IsWebsiteNode(groupNode));
    }
}
