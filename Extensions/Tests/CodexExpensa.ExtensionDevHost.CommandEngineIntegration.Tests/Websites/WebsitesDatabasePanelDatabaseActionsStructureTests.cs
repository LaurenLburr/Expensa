using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class WebsitesDatabasePanelDatabaseActionsStructureTests
{
    [Fact]
    public void Designer_IncludesDatabaseActionLinks()
    {
        string repositoryRoot = FindRepositoryRoot();

        string designerPath =
            Path.Combine(
                repositoryRoot,
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "WebsitesDatabasePanelForm.Designer.cs");

        Assert.True(File.Exists(designerPath), $"File was not found: {designerPath}");

        string text = File.ReadAllText(designerPath);

        Assert.Contains("databaseNameLinkLabel", text);
        Assert.Contains("copyFromDevTemplateLinkLabel", text);
        Assert.Contains("copyFromSandboxLinkLabel", text);
        Assert.Contains("Copy new from dev template", text);
        Assert.Contains("Copy new from Sandbox DB", text);
    }

    private static string FindRepositoryRoot()
    {
        return TestPathHelper.ExtensionsRoot;
    }
}
