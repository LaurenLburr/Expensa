using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteTreeContributionLoaderEndToEndStructureTests
{
    [Fact]
    public void Loader_ExecutesParsesAndRendersWebsitesLoadResult()
    {
        string text = ReadFile(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Websites",
            "HostWebsiteTreeContributionLoader.cs");

        Assert.Contains("HostWebsiteRuntimeModuleInvoker", text);
        Assert.Contains("HostWebsiteLoadExecutionResultAdapter.FromExecutionResult", text);
        Assert.Contains("HostWebsiteTreeViewRenderer.Render(", text);
        Assert.Contains("treeView,", text);
        Assert.Contains("loadResult", text);
        Assert.Contains("treeView.ExpandAll();", text);
        Assert.Contains("ExecutionResult = executionResult", text);
        Assert.Contains("WebsiteResult = loadResult", text);
        Assert.DoesNotContain("WebsiteNodeCount", text);
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
