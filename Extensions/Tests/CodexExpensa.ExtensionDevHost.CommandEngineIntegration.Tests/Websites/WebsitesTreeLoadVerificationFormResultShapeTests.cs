using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class WebsitesTreeLoadVerificationFormResultShapeTests
{
    [Fact]
    public void VerificationForm_UsesCurrentTreeLoadResultShape()
    {
        string text = ReadFile(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Websites",
            "WebsitesTreeLoadVerificationForm.cs");

        Assert.Contains("ExecutionResult.Status", text);
        Assert.Contains("WebsiteResult.TotalCount", text);
        Assert.Contains("WebsiteResult.Message", text);
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
