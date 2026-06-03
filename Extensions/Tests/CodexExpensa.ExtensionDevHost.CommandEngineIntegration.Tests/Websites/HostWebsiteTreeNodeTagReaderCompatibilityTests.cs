using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteTreeNodeTagReaderCompatibilityTests
{
    [Fact]
    public void ReadWebsiteId_DelegatesToGetWebsiteIdForExistingTests()
    {
        TreeNode node =
            new("Website")
            {
                Tag = new HostWebsiteTreeNodePayload
                {
                    NodeId = "website-1",
                    WebsiteId = "website-1",
                    DisplayText = "Website",
                    TagName = "Example",
                    Url = "https://example.test",
                    IsActive = true
                }
            };

        Assert.Equal(
            HostWebsiteTreeNodeTagReader.GetWebsiteId(node),
            HostWebsiteTreeNodeTagReader.ReadWebsiteId(node));
    }
}
