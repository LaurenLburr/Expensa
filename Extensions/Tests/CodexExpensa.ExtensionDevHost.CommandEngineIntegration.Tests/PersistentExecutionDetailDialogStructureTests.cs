using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class PersistentExecutionDetailDialogStructureTests
{
    [Fact]
    public void Designer_IncludesExpectedTabsAndButtons()
    {
        string repositoryRoot = FindRepositoryRoot();

        string designerPath =
            Path.Combine(
                repositoryRoot,
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "PersistentExecutionDetailDialog.Designer.cs");

        Assert.True(File.Exists(designerPath), $"File was not found: {designerPath}");

        string text = File.ReadAllText(designerPath);

        Assert.Contains("summaryTabPage", text);
        Assert.Contains("parametersTabPage", text);
        Assert.Contains("outputTabPage", text);
        Assert.Contains("copySummaryButton", text);
        Assert.Contains("copyParametersButton", text);
        Assert.Contains("copyOutputButton", text);
    }

    private static string FindRepositoryRoot()
    {
        return TestPathHelper.ExtensionsRoot;
    }
}
