using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class ImportExistingAddinProjectStructureTests
{
    [Fact]
    public void MainForm_AddsImportExistingNodeUnderAddinProjects()
    {
        string text = ReadFile("Host", "CodexExpensa.ExtensionDevHost", "MainForm.cs");
        Assert.Contains("Import Existing Add-in Project", text);
        Assert.Contains("ImportExistingAddinProjectNavigationTag", text);
        Assert.Contains("ShowImportExistingAddinProjectDialog", text);
    }

    [Fact]
    public void ImportService_DerivesRegistrationFromProjectFile()
    {
        string text = ReadFile("Host", "CodexExpensa.ExtensionDevHost", "Services", "ExistingAddinProjectImportService.cs");
        Assert.Contains("ReadTargetFramework", text);
        Assert.Contains("ReadAssemblyName", text);
        Assert.Contains("RelativeBinPath", text);
        Assert.Contains("Upsert", text);
    }

    [Fact]
    public void MainForm_DoesNotHardCodeSpecificAddinKindsForDatabaseOrTestSurfaces()
    {
        string text = ReadFile( "Host", "CodexExpensa.ExtensionDevHost", "MainForm.cs");
        Assert.Contains("AddinProjectUiSurfaceResolver", text);
        Assert.DoesNotContain("IsWebsitesAddinProject", text);
        Assert.DoesNotContain("IsBudgetsAddinProject", text);
        Assert.DoesNotContain("new WebsitesDatabasePanelForm", text);
        Assert.DoesNotContain("new BudgetsDatabasePanelForm", text);
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
}
