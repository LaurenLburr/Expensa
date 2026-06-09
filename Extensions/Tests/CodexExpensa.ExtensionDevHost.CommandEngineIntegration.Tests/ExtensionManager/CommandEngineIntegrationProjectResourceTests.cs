using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class CommandEngineIntegrationProjectResourceTests
{
    [Fact]
    public void Project_ExcludesDuplicateExtensionRuntimeDashboardResources()
    {
        string repositoryRoot = FindRepositoryRoot();

        string projectPath =
            Path.Combine(
                repositoryRoot,
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "CodexExpensa.ExtensionDevHost.CommandEngineIntegration.csproj");

        Assert.True(File.Exists(projectPath), $"File was not found: {projectPath}");

        string text = File.ReadAllText(projectPath);

        Assert.Contains(
            "EmbeddedResource Remove=\"**\\ExtensionRuntimeDashboardForm.resx\"",
            text);
    }

    private static string FindRepositoryRoot()
    {
        return TestPathHelper.ExtensionsRoot;
    }
}
