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
                "Extensions",
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
        DirectoryInfo? directory =
            new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "Extensions")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not find repository root containing Extensions folder.");
    }
}
