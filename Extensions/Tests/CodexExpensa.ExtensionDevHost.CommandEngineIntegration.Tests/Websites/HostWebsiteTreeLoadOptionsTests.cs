using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteTreeLoadOptionsTests
{
    [Fact]
    public void Defaults_AreExpected()
    {
        HostWebsiteTreeLoadOptions options = new();

        Assert.Equal(string.Empty, options.SearchText);
        Assert.False(options.IncludeDisabled);
        Assert.Equal(500, options.MaximumRows);
        Assert.True(options.ExpandAll);
    }
}
