using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class WebsitesDatabasePanelActiveDatabaseStructureTests
{
    [Fact]
    public void Designer_ContainsCopyFromExpensaProdAction()
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

        Assert.Contains("copyFromExpensaProdLinkLabel", text);
        Assert.Contains("Copy from Expensa Prod", text);
    }

    private static string FindRepositoryRoot()
    {
        return TestPathHelper.ExtensionsRoot;
    }
}
