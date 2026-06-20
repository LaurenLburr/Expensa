using CodexExpensa.App.WinForms.Tests.UI.Addins;
using Xunit;

namespace CodexExpensa.App.WinForms.Tests.UI.Websites;

public sealed class WebsiteAddinAssemblyLocatorStructureTests
{
    [Fact]
    public void WebsiteLocator_PrefersCompiledExpensaExtensionsBeforeModulesFallback()
    {
        string text = TestRepositoryPath.ReadExpensaFile(
            "UI",
            "Websites",
            "WebsiteAddinAssemblyLocator.cs");

        int deployedIndex = text.IndexOf(
            "\"Expensa\"",
            StringComparison.Ordinal);

        int modulesIndex = text.IndexOf(
            "\"Modules\"",
            StringComparison.Ordinal);

        Assert.True(deployedIndex >= 0, "Locator should include Expensa\\Extensions\\WebsitesAddin.");
        Assert.True(modulesIndex >= 0, "Locator should retain the old Modules fallback.");
        Assert.True(deployedIndex < modulesIndex, "Expensa\\Extensions must be searched before Modules.");
    }
}
