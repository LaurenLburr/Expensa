using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class ExtensionRuntimeDashboardDesignerMenuTests
{
    [Fact]
    public void Designer_IncludesExtensionManagerAndWebsitesTopLevelMenus()
    {
        string repositoryRoot =
            FindRepositoryRoot();

        string designerPath =
            Path.Combine(
                repositoryRoot,
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "ExtensionRuntimeDashboardForm.Designer.cs");

        Assert.True(File.Exists(designerPath), $"File was not found: {designerPath}");

        string text =
            File.ReadAllText(designerPath);

        Assert.Contains("extensionManagerMenuItem", text);
        Assert.Contains("openExtensionManagerAddinTestSurfaceMenuItem", text);
        Assert.Contains("websitesMenuItem", text);
        Assert.Contains("extensionManagerMenuItem,", text);
        Assert.Contains("websitesMenuItem,", text);
        Assert.Contains("OpenExtensionManagerAddinTestSurface", text);
    }

    private static string FindRepositoryRoot()
    {
        return TestPathHelper.ExtensionsRoot;
    }
}
