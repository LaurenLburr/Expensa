using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class ExtensionManagerAddinTestCatalogTests
{
    [Fact]
    public void GetAddins_IncludesWebsitesAddin()
    {
        ExtensionManagerAddinTestCatalog catalog = new();

        IReadOnlyList<ExtensionManagerAddinTestNode> addins =
            catalog.GetAddins();

        Assert.Contains(addins, addin => addin.AddinId == "websites");
    }
}
