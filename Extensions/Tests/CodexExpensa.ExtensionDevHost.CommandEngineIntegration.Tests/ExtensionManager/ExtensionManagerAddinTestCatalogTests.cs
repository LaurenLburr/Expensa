using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class ExtensionManagerAddinTestCatalogTests
{
    [Fact]
    public void GetAddins_IncludesWebsitesAndPayeesAddinsWithDatabaseAndTestNames()
    {
        ExtensionManagerAddinTestCatalog catalog = new();

        IReadOnlyList<ExtensionManagerAddinTestNode> addins = catalog.GetAddins();

        ExtensionManagerAddinTestNode websites = Assert.Single(
            addins,
            static item => string.Equals(item.AddinId, "websites", StringComparison.OrdinalIgnoreCase));

        Assert.Equal("Websites Database", websites.DatabaseDisplayName);
        Assert.Equal("Websites Tree Test", websites.TestFormName);

        ExtensionManagerAddinTestNode payees = Assert.Single(
            addins,
            static item => string.Equals(item.AddinId, "payees", StringComparison.OrdinalIgnoreCase));

        Assert.Equal("Payees Add-in", payees.DisplayName);
        Assert.Equal("Payees Database", payees.DatabaseDisplayName);
        Assert.Equal("Payees Tree Test", payees.TestFormName);
        Assert.True(payees.SortOrder > websites.SortOrder);
    }
}
