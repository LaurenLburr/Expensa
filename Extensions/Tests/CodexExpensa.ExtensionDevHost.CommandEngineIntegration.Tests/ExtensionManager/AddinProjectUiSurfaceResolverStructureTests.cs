using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class AddinProjectUiSurfaceResolverStructureTests
{
    [Fact]
    public void MainForm_DoesNotHardCodeSpecificAddinKindsForDatabaseOrTestSurfaces()
    {
      //  string text = ReadFile("Extensions", "Host", "CodexExpensa.ExtensionDevHost", "MainForm.cs");
        string text = ReadFile("Host", "CodexExpensa.ExtensionDevHost", "MainForm.cs");

        Assert.Contains("AddinProjectUiSurfaceResolver", text);
        Assert.Contains("TryCreateDatabaseForm", text);
        Assert.Contains("TryCreateTestForm", text);

        Assert.DoesNotContain("IsWebsitesAddinProject", text);
        Assert.DoesNotContain("IsBudgetsAddinProject", text);
        Assert.DoesNotContain("new WebsitesDatabasePanelForm", text);
        Assert.DoesNotContain("new BudgetsDatabasePanelForm", text);
        Assert.DoesNotContain("new WebsitesTreeLoadVerificationForm", text);
        Assert.DoesNotContain("new BudgetsTreeLoadVerificationForm", text);
    }

    [Fact]
    public void Resolver_UsesAddinNameConventionInsteadOfSpecificIfChains()
    {
        string text = ReadFile(
            
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "ExtensionManager",
            "AddinProjectUiSurfaceResolver.cs");

        Assert.Contains("NormalizeAddinName", text);
        Assert.Contains("EnsurePlural", text);
        Assert.Contains("DatabasePanelForm", text);
        Assert.Contains("TreeLoadVerificationFormCommonTree", text);
        Assert.Contains("Activator.CreateInstance", text);

        Assert.DoesNotContain("WebsitesAddin", text);
        Assert.DoesNotContain("BudgetsAddin", text);
        Assert.DoesNotContain("PayeesAddin", text);
    }

    private static string ReadFile(params string[] parts)
    {
        string repositoryRoot = FindRepositoryRoot();
        string path = Path.Combine([repositoryRoot, .. parts]);

        Assert.True(File.Exists(path), $"File was not found: {path}");

        return File.ReadAllText(path);
    }

    private static string FindRepositoryRoot()
    {
        return TestPathHelper.ExtensionsRoot;
    }

    internal static class TestPathHelper
    {
        public static string ExtensionsRoot
        {
            get
            {
                DirectoryInfo? directory = new(AppContext.BaseDirectory);

                while (directory is not null)
                {
                    if (File.Exists(Path.Combine(directory.FullName, "Extensions.sln")))
                    {
                        return directory.FullName;
                    }

                    if (Directory.Exists(Path.Combine(directory.FullName, "Host"))
                        && Directory.Exists(Path.Combine(directory.FullName, "Modules"))
                        && Directory.Exists(Path.Combine(directory.FullName, "Tests")))
                    {
                        return directory.FullName;
                    }

                    directory = directory.Parent;
                }

                throw new DirectoryNotFoundException(
                    "Could not find Extensions solution root.");
            }
        }

        public static string HostPath(params string[] parts)
        {
            return Path.Combine([ExtensionsRoot, "Host", .. parts]);
        }

        public static string ModulePath(params string[] parts)
        {
            return Path.Combine([ExtensionsRoot, "Modules", .. parts]);
        }

        public static string TestPath(params string[] parts)
        {
            return Path.Combine([ExtensionsRoot, "Tests", .. parts]);
        }
    }
}
