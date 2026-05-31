using Xunit;

namespace WebsitesAddin.Tests;

public sealed class WebsiteLoadRequestDatabasePathTests
{
    [Fact]
    public void Parser_ReadsDatabasePathFromObjectParameters()
    {
        Dictionary<string, object?> parameters =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["searchText"] = "bank",
                ["includeDisabled"] = false,
                ["maximumRows"] = 25,
                ["databasePath"] = @"C:\Temp\websitesaddin.db"
            };

        WebsiteLoadRequest request =
            WebsiteLoadRequestParser.Parse(parameters);

        Assert.Equal(@"C:\Temp\websitesaddin.db", request.DatabasePath);
    }

    [Fact]
    public void SmokeRunner_PreservesDatabasePathParameter()
    {
        string repositoryRoot =
            FindRepositoryRoot();

        string runnerPath =
            Path.Combine(
                repositoryRoot,
                "Extensions",
                "Modules",
                "WebsitesAddin",
                "WebsiteLoadRuntimeSmokeRunner.cs");

        Assert.True(File.Exists(runnerPath), $"File was not found: {runnerPath}");

        string text =
            File.ReadAllText(runnerPath);

        Assert.Contains("[\"databasePath\"] = request.DatabasePath", text);
    }

    [Fact]
    public void Command_UsesSqliteRepositoryWhenDatabasePathExists()
    {
        string repositoryRoot =
            FindRepositoryRoot();

        string commandPath =
            Path.Combine(
                repositoryRoot,
                "Extensions",
                "Modules",
                "WebsitesAddin",
                "WebsiteLoadCommand.cs");

        Assert.True(File.Exists(commandPath), $"File was not found: {commandPath}");

        string text =
            File.ReadAllText(commandPath);

        Assert.Contains("CreateRepositoryForRequest", text);
        Assert.Contains("request.DatabasePath", text);
        Assert.Contains("SqliteWebsiteRepository", text);
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
