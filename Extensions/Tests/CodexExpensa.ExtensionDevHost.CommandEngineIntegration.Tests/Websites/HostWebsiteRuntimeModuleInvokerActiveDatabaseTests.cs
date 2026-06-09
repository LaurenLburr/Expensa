using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteRuntimeModuleInvokerActiveDatabaseTests
{
    [Fact]
    public void Invoker_PassesActiveDatabasePathByReflection()
    {
        string text = ReadFile(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Websites",
            "HostWebsiteRuntimeModuleInvoker.cs");

        Assert.Contains("HostWebsiteRuntimeDatabaseSelectionService", text);
        Assert.Contains("GetActiveRuntimeDatabaseLocation", text);
        Assert.Contains("HostWebsiteDatabaseLocation activeDatabaseLocation", text);
        Assert.Contains("SetProperty(loadRequest, \"DatabasePath\", activeDatabaseLocation.DatabasePath);", text);
        Assert.DoesNotContain("using WebsitesAddin;", text);
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
