using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteTreeNodeTagReaderTests
{
    [Fact]
    public void Reader_ReturnsWebsiteIdFromPayloadInterface()
    {
        TreeNode node = new("Website")
        {
            Tag = new HostWebsiteTreeNodePayload
            {
                NodeId = "website-123",
                WebsiteId = "website-123",
                DisplayText = "Website",
                TagName = "Banking",
                Url = "https://example.test",
                IsActive = true
            }
        };

        Assert.Equal("website-123", HostWebsiteTreeNodeTagReader.ReadWebsiteId(node));
    }

    [Fact]
    public void Reader_ReturnsEmptyWebsiteIdForGroupNode()
    {
        TreeNode node = new("Banking")
        {
            Tag = new HostWebsiteCategoryGroupTreeNodePayload
            {
                NodeId = "tag:Banking",
                DisplayText = "Banking"
            }
        };

        Assert.Equal(string.Empty, HostWebsiteTreeNodeTagReader.ReadWebsiteId(node));
    }

    [Fact]
    public void Reader_UsesCompatibilityFallbackWhenTagIsNotPayloadInterface()
    {
        TreeNode parent = new("Group")
        {
            Name = "tag:Banking"
        };

        TreeNode node = new("Website")
        {
            Name = "website-789",
            Tag = "not a payload"
        };

        parent.Nodes.Add(node);

        Assert.Equal("website-789", HostWebsiteTreeNodeTagReader.ReadWebsiteId(node));
    }

    [Fact]
    public void Reader_DetectsWebsiteAndGroupNodeTypes()
    {
        TreeNode websiteNode = new("Website")
        {
            Tag = new HostWebsiteTreeNodePayload
            {
                NodeId = "website-123",
                WebsiteId = "website-123",
                DisplayText = "Website",
                TagName = "Banking",
                Url = "https://example.test",
                IsActive = true
            }
        };

        TreeNode groupNode = new("Banking")
        {
            Tag = new HostWebsiteCategoryGroupTreeNodePayload
            {
                NodeId = "tag:Banking",
                DisplayText = "Banking"
            }
        };

        Assert.True(HostWebsiteTreeNodeTagReader.IsWebsiteNode(websiteNode));
        Assert.False(HostWebsiteTreeNodeTagReader.IsWebsiteNode(groupNode));
        Assert.True(HostWebsiteTreeNodeTagReader.IsCategoryGroupNode(groupNode));
    }
}
