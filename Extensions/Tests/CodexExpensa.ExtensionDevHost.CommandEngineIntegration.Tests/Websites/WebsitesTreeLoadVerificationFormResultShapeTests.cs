using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class WebsitesTreeLoadVerificationFormResultShapeTests
{
    [Fact]
    public void VerificationForm_UsesExistingTreeLoadResultShape()
    {
        string text = ReadFile(
            "Extensions",
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Websites",
            "WebsitesTreeLoadVerificationForm.cs");

        Assert.Contains("result.ExecutionResult.Status", text);
        Assert.Contains("result.WebsiteResult.TotalCount", text);
        Assert.Contains("result.WebsiteResult.Message", text);
        Assert.Contains("result.ExecutionResult.OutputJson", text);
        Assert.DoesNotContain("result.WebsiteNodeCount", text);
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
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "Extensions")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException();
    }
}
