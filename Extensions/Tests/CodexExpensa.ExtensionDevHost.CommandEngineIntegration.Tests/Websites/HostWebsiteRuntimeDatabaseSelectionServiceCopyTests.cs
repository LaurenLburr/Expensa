using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteRuntimeDatabaseSelectionServiceCopyTests
{
    [Fact]
    public void SelectionService_SetActiveRuntimeDatabaseCopiesToCanonicalRuntimePath()
    {
        string repositoryRoot = FindRepositoryRoot();

        string servicePath =
            Path.Combine(
                repositoryRoot,
                "Extensions",
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "HostWebsiteRuntimeDatabaseSelectionService.cs");

        Assert.True(File.Exists(servicePath), $"File was not found: {servicePath}");

        string text =
            File.ReadAllText(servicePath);

        Assert.Contains("File.Copy(", text);
        Assert.Contains("runtimeLocation.DatabasePath", text);
        Assert.Contains("overwrite: true", text);
        Assert.Contains("return runtimeLocation;", text);
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
