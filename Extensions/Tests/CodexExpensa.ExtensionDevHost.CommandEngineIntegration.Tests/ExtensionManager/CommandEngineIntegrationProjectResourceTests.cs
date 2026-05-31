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
                "Extensions",
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
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "Extensions")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not find repository root containing Extensions folder.");
    }
}
