using WebsitesAddin;
using Xunit;

namespace WebsitesAddin.Tests;

public sealed class WebsiteLoadCommandTests
{
    [Fact]
    public void Execute_WithRequest_ReturnsWebsiteNodes()
    {
        WebsiteLoadCommand command = new();

        WebsiteLoadResult result =
            command.Execute(new WebsiteLoadRequest());

        Assert.NotEmpty(result.Nodes);
        Assert.Contains("Loaded", result.Message);
    }

    [Fact]
    public void Execute_WithParameters_UsesParser()
    {
        WebsiteLoadCommand command = new();

        WebsiteLoadResult result =
            command.Execute(
                new Dictionary<string, object?>
                {
                    ["searchText"] = "bank",
                    ["maximumRows"] = 10
                });

        Assert.NotEmpty(result.Nodes);
        Assert.Contains(result.Nodes, node => node.DisplayText == "Banking");
    }
}
