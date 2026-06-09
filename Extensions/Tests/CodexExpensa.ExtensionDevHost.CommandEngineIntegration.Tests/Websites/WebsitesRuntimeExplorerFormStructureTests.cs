using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class WebsitesRuntimeExplorerFormStructureTests
{
    [Fact]
    public void Designer_IncludesExpectedRuntimeExplorerControls()
    {
        string repositoryRoot = FindRepositoryRoot();

        string designerPath =
            Path.Combine(
                repositoryRoot,
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "WebsitesRuntimeExplorerForm.Designer.cs");

        Assert.True(File.Exists(designerPath), $"File was not found: {designerPath}");

        string text = File.ReadAllText(designerPath);

        Assert.Contains("websitesTreeView", text);
        Assert.Contains("loadButton", text);
        Assert.Contains("searchTextBox", text);
        Assert.Contains("maximumRowsNumericUpDown", text);
        Assert.Contains("includeDisabledCheckBox", text);
    }

    private static string FindRepositoryRoot()
    {
        return TestPathHelper.ExtensionsRoot;
    }
}
