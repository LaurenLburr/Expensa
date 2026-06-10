using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class ExtensionManagerAddinTestSurfaceFormResolverTests
{
    [Fact]
    public void TestSurfaceForm_UsesGenericSurfaceResolverInsteadOfHardcodedWebsitesForm()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "ExtensionManager",
            "ExtensionManagerAddinTestSurfaceForm.cs");

        Assert.Contains("AddinProjectUiSurfaceResolver", text, StringComparison.Ordinal);
        Assert.Contains("TryCreateDatabaseForm", text, StringComparison.Ordinal);
        Assert.Contains("TryCreateTestForm", text, StringComparison.Ordinal);
        Assert.DoesNotContain("action.Addin.AddinId, \"websites\"", text, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("new ExtensionTreeLoadTestForm", text, StringComparison.Ordinal);
    }
}
