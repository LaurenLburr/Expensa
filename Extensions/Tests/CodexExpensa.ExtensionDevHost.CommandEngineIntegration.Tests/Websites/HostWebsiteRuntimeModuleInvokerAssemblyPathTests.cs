using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteRuntimeModuleInvokerAssemblyPathTests
{
    [Fact]
    public void Invoker_SearchesSolutionLevelModulesFolderAndBuildOutputs()
    {
        string text = ReadFile(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Websites",
            "HostWebsiteRuntimeModuleInvoker.cs");

        Assert.Contains("GetWebsitesAddinAssemblyCandidatePaths", text);
        Assert.Contains("EnumerateAncestorFolders", text);
        Assert.Contains("Modules", text);
        Assert.Contains("WebsitesAddin", text);
        Assert.Contains("net8.0-windows", text);
        Assert.Contains("net8.0", text);
        Assert.Contains("Searched:", text);
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
