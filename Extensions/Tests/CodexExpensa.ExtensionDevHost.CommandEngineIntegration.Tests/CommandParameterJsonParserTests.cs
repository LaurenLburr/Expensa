using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class CommandParameterJsonParserTests
{
    [Fact]
    public void Parse_WhenEmpty_ReturnsEmptyDictionary()
    {
        IReadOnlyDictionary<string, object?> parameters =
            CommandParameterJsonParser.Parse("");

        Assert.Empty(parameters);
    }

    [Fact]
    public void Parse_WhenObject_ReturnsValues()
    {
        IReadOnlyDictionary<string, object?> parameters =
            CommandParameterJsonParser.Parse("""
            {
              "name": "Lauren",
              "count": 3,
              "enabled": true
            }
            """);

        Assert.Equal("Lauren", parameters["name"]);
        Assert.Equal(3, parameters["count"]);
        Assert.Equal(true, parameters["enabled"]);
    }

    [Fact]
    public void Parse_WhenRootIsArray_Throws()
    {
        Assert.Throws<InvalidOperationException>(
            () => CommandParameterJsonParser.Parse("[1,2,3]"));
    }
}
