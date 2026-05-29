using WebsitesAddin;
using Xunit;

namespace WebsitesAddin.Tests;

public sealed class InMemoryWebsiteRepositoryTests
{
    [Fact]
    public void LoadWebsites_WhenSearchMatchesChild_ReturnsParent()
    {
        InMemoryWebsiteRepository repository = new();

        IReadOnlyList<WebsiteTreeNode> nodes =
            repository.LoadWebsites(new WebsiteLoadRequest
            {
                SearchText = "bank"
            });

        Assert.NotEmpty(nodes);
        Assert.Contains(nodes, node => node.DisplayText == "Banking");
    }

    [Fact]
    public void LoadWebsites_RespectsMaximumRows()
    {
        InMemoryWebsiteRepository repository = new();

        IReadOnlyList<WebsiteTreeNode> nodes =
            repository.LoadWebsites(new WebsiteLoadRequest
            {
                MaximumRows = 1
            });

        Assert.Single(nodes);
    }
}
