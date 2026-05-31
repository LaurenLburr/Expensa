using Xunit;

namespace WebsitesAddin.Tests;

public sealed class WebsiteInMemoryDatabaseFactoryStructureTests
{
    [Fact]
    public void InMemoryFactory_UsesSqliteBackupDatabase()
    {
        string text = ReadFile(
            "Extensions",
            "Modules",
            "WebsitesAddin",
            "WebsiteInMemoryDatabaseFactory.cs");

        Assert.Contains("Data Source=:memory:", text);
        Assert.Contains("Mode=ReadOnly", text);
        Assert.Contains("BackupDatabase", text);
    }

    [Fact]
    public void WebsiteLoadCommand_UsesInMemoryCopyForDatabasePath()
    {
        string text = ReadFile(
            "Extensions",
            "Modules",
            "WebsitesAddin",
            "WebsiteLoadCommand.cs");

        Assert.Contains("WebsiteInMemoryDatabaseFactory.OpenMemoryCopy", text);
        Assert.Contains("Connection = memoryConnection", text);
        Assert.Contains("OwnsConnection = true", text);
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
