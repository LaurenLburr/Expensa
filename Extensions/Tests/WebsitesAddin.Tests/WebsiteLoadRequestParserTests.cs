using WebsitesAddin;
using Xunit;

namespace WebsitesAddin.Tests;

public sealed class WebsiteLoadRequestParserTests
{
    [Fact]
    public void Parse_WithValues_ReturnsRequest()
    {
        WebsiteLoadRequest request =
            WebsiteLoadRequestParser.Parse(
                new Dictionary<string, object?>
                {
                    ["searchText"] = "bank",
                    ["includeDisabled"] = true,
                    ["maximumRows"] = 25
                });

        Assert.Equal("bank", request.SearchText);
        Assert.True(request.IncludeDisabled);
        Assert.Equal(25, request.MaximumRows);
    }

    [Fact]
    public void Parse_WithMissingValues_ReturnsDefaults()
    {
        WebsiteLoadRequest request =
            WebsiteLoadRequestParser.Parse(new Dictionary<string, object?>());

        Assert.Equal(string.Empty, request.SearchText);
        Assert.False(request.IncludeDisabled);
        Assert.Equal(500, request.MaximumRows);
    }
}
