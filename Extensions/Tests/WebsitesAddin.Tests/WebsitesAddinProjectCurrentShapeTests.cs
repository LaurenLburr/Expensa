using Xunit;

namespace WebsitesAddin.Tests;

public sealed class WebsitesAddinProjectCurrentShapeTests
{
    [Fact]
    public void Project_FileStillReferencesNavigationAbstractions()
    {
        string projectFile =
            ReadModuleFile("WebsitesAddin.csproj");

        Assert.Contains("CodexExpensa.Navigation.Abstractions", projectFile);
        Assert.Contains("net8.0-windows", projectFile);
    }

    [Fact]
    public void Project_ContainsCurrentExtensionClass()
    {
        string extensionFile =
            ReadModuleFile("WebsitesAddinExtension.cs");

        Assert.Contains("public sealed class WebsitesAddinExtension", extensionFile);
        Assert.Contains("ITreeNodeExtension", extensionFile);
        Assert.Contains("ExtensionKey => \"WebsitesAddin\"", extensionFile);
    }

    [Fact]
    public void Project_DoesNotCurrentlyContainLegacyRuntimeCommandClasses()
    {
        string moduleFolder =
            FindModuleFolder();

        Assert.False(
            File.Exists(Path.Combine(moduleFolder, "WebsiteLoadCommand.cs")),
            "WebsiteLoadCommand.cs is from the older WebsitesAddin runtime-command design and should not be required by current tests.");

        Assert.False(
            File.Exists(Path.Combine(moduleFolder, "SqliteWebsiteRepository.cs")),
            "SqliteWebsiteRepository.cs is from the older WebsitesAddin database-command design and should not be required by current tests.");
    }

    private static string ReadModuleFile(string fileName)
    {
        string path =
            Path.Combine(FindModuleFolder(), fileName);

        Assert.True(File.Exists(path), $"File was not found: {path}");

        return File.ReadAllText(path);
    }

    private static string FindModuleFolder()
    {
        string repositoryRoot =
            FindRepositoryRoot();

        string moduleFolder =
            Path.Combine(
                repositoryRoot,
                "Extensions",
                "Modules",
                "WebsitesAddin");

        Assert.True(Directory.Exists(moduleFolder), $"Module folder was not found: {moduleFolder}");

        return moduleFolder;
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory =
            new(AppContext.BaseDirectory);

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
