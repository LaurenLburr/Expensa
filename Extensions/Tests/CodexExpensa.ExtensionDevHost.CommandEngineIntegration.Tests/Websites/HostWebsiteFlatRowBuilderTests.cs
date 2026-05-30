using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteFlatRowBuilderTests
{
    [Fact]
    public void BuildRows_FlattensWebsiteTreeNodesWithUrls()
    {
        HostWebsiteLoadResult result = new()
        {
            Nodes =
            [
                new HostWebsiteTreeNode
                {
                    NodeId = "category.Banking",
                    DisplayText = "Banking",
                    Category = "Root",
                    Children =
                    [
                        new HostWebsiteTreeNode
                        {
                            NodeId = "banking.demo",
                            DisplayText = "Demo Bank",
                            Url = "https://example.com/bank",
                            Category = "Banking",
                            IsEnabled = true
                        }
                    ]
                }
            ]
        };

        IReadOnlyList<HostWebsiteFlatRow> rows =
            HostWebsiteFlatRowBuilder.BuildRows(result);

        HostWebsiteFlatRow row =
            Assert.Single(rows);

        Assert.Equal("banking.demo", row.NodeId);
        Assert.Equal("Demo Bank", row.DisplayText);
        Assert.Equal("https://example.com/bank", row.Url);
        Assert.Equal("Banking", row.Category);
        Assert.True(row.IsEnabled);
    }
}
