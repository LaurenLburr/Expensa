using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class MainAddinProjectDatabaseTestActionTests
{
    [Fact]
    public void MainForm_DatabaseAndTestNodesHaveNavigationActions()
    {
        string text =
            ReadFile(
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "MainForm.cs");

        Assert.Contains("AddinProjectDatabaseNavigationTag", text);
        Assert.Contains("AddinProjectTestNavigationTag", text);
        Assert.Contains("ShowAddinDatabaseNode", text);
        Assert.Contains("OpenAddinTestNode", text);

        Assert.Contains("ShowEmbeddedForm(new WebsitesDatabasePanelForm())", text);
        Assert.True(
            text.Contains("ShowEmbeddedForm(new WebsitesTreeLoadVerificationFormCommonTree())") ||
            text.Contains("ShowEmbeddedForm(new WebsitesTreeLoadVerificationForm())"),
            "MainForm should route the Websites test node to either the CommonTree verification form or the legacy verification form.");

        Assert.Contains("ShowEmbeddedForm(new BudgetsDatabasePanelForm())", text);
        Assert.True(
            text.Contains("ShowEmbeddedForm(new BudgetsTreeLoadVerificationFormCommonTree())") ||
            text.Contains("ShowEmbeddedForm(new BudgetsTreeLoadVerificationForm())"),
            "MainForm should route the Budgets test node to either the CommonTree verification form or the legacy verification form.");
    }

    private static string ReadFile(params string[] parts)
    {
        string repositoryRoot =
            FindRepositoryRoot();

        string path =
            Path.Combine([repositoryRoot, .. parts]);

        Assert.True(
            File.Exists(path),
            $"File was not found: {path}");

        return File.ReadAllText(path);
    }

    private static string FindRepositoryRoot()
    {
        return TestPathHelper.ExtensionsRoot;
    }
}
