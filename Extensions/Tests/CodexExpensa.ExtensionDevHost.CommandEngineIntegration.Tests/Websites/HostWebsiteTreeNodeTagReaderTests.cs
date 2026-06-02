using System.Windows.Forms;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteTreeNodeTagReaderTests
{
    [Fact]
    public void Reader_ReadsWebsiteIdFromPayloadInterface()
    {
        TreeNode treeNode =
            new("Banking")
            {
                Tag =
                    new HostWebsiteTreeNodePayload
                    {
                        NodeId = "website-123",
                        WebsiteId = "website-123",
                        DisplayText = "Banking Demo",
                        Url = "https://example.com/bank",
                        TagName = "banking.demo",
                        IsActive = true
                    }
            };

        string websiteId =
            HostWebsiteTreeNodeTagReader.ReadWebsiteId(treeNode);

        Assert.Equal("website-123", websiteId);
    }

    [Fact]
    public void Reader_ReadsWebsiteIdFromNodeMappedPayload()
    {
        HostWebsiteTreeNode websiteNode =
            new()
            {
                NodeId = "website-456",
                DisplayText = "Demo Bank",
                Url = "https://example.com/bank",
                Category = "Banking"
            };

        TreeNode treeNode =
            HostWebsiteTreeViewNodeMapper.ToTreeNode(websiteNode);

        string websiteId =
            HostWebsiteTreeNodeTagReader.ReadWebsiteId(treeNode);

        Assert.Equal("website-456", websiteId);
    }

    [Fact]
    public void Reader_ReturnsEmptyWebsiteIdWhenTagIsNotPayloadInterface()
    {
        TreeNode treeNode =
            new("Banking")
            {
                Tag =
                    new
                    {
                        NodeId = "website-789",
                        Url = "https://example.com/bank"
                    }
            };

        string websiteId =
            HostWebsiteTreeNodeTagReader.ReadWebsiteId(treeNode);

        Assert.Equal(string.Empty, websiteId);
    }
}
