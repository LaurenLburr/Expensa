using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteLoadResultParserTests
{
    [Fact]
    public void Parse_WhenJsonContainsNodes_ReturnsResult()
    {
        const string json = """
        {
          "nodes": [
            {
              "nodeId": "banking",
              "displayText": "Banking",
              "url": "",
              "category": "Root",
              "isEnabled": true,
              "children": [
                {
                  "nodeId": "banking.demo",
                  "displayText": "Demo Bank",
                  "url": "https://example.com/bank",
                  "category": "Banking",
                  "isEnabled": true,
                  "children": []
                }
              ]
            }
          ],
          "totalCount": 1,
          "message": "Loaded 1 website node(s)."
        }
        """;

        HostWebsiteLoadResult result =
            HostWebsiteLoadResultParser.Parse(json);

        Assert.Single(result.Nodes);
        Assert.Equal("Banking", result.Nodes[0].DisplayText);
        Assert.Single(result.Nodes[0].Children);
    }

    [Fact]
    public void Parse_WhenBlank_ReturnsEmptyResult()
    {
        HostWebsiteLoadResult result =
            HostWebsiteLoadResultParser.Parse("");

        Assert.Empty(result.Nodes);
        Assert.Contains("No website output", result.Message);
    }
}
