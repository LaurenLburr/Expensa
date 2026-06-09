using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class DatabaseFormsUseTemplateTests
{
    [Theory]
    [InlineData("Budgets", "BudgetsDatabasePanelForm")]
    [InlineData("Websites", "WebsitesDatabasePanelForm")]
    public void DatabaseForms_DeriveFromDatabasePanelTemplate(string folderName, string formName)
    {
        string formText = ReadFile("Host", "CodexExpensa.ExtensionDevHost", "CommandEngineIntegration", folderName, formName + ".cs");

        Assert.Contains(formName + " : DatabasePanelTemplate", formText);
        Assert.Contains("ConfigureDatabasePanel", formText);
        Assert.Contains("SetGridDataSource", formText);
        Assert.Contains("SetSummaryText", formText);
        Assert.Contains("SetRowStatus", formText);
    }

    [Theory]
    [InlineData("Budgets", "BudgetsDatabasePanelForm")]
    [InlineData("Websites", "WebsitesDatabasePanelForm")]
    public void DatabaseForms_DoNotOwnTemplateLayout(string folderName, string formName)
    {
        string designerText = ReadFile("Host", "CodexExpensa.ExtensionDevHost", "CommandEngineIntegration", folderName, formName + ".Designer.cs");

        Assert.DoesNotContain("new SplitContainer", designerText);
        Assert.DoesNotContain("new DataGridView", designerText);
        Assert.DoesNotContain("new StatusStrip", designerText);
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
