using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class ExtensionManagerAddinTestCatalogTests
{
    [Fact]
    public void GetAddins_IncludesWebsitesAddinWithDatabaseAndTestNames()
    {
        ExtensionManagerAddinTestCatalog catalog = new();

        ExtensionManagerAddinTestNode websites =
            Assert.Single(catalog.GetAddins());

        Assert.Equal("websites", websites.AddinId);
        Assert.Equal("Websites Database", websites.DatabaseDisplayName);
        Assert.Equal("Websites Tree Test", websites.TestFormName);
    }
}
