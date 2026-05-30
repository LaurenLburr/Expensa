using Xunit;

namespace WebsitesAddin.Tests;

public sealed class WebsiteDevDatabaseFileTests
{
    [Fact]
    public void DevDatabaseScripts_ContainSqlQueryCatalogAndWebsiteTable()
    {
        string repositoryRoot =
            FindRepositoryRoot();

        string devDatabaseFolder =
            Path.Combine(
                repositoryRoot,
                "Extensions",
                "Modules",
                "WebsitesAddin",
                "DevDatabase");

        string schemaPath =
            Path.Combine(devDatabaseFolder, "001_Create_Websites_Schema.sql");

        string querySeedPath =
            Path.Combine(devDatabaseFolder, "003_Seed_SqlQuery.sql");

        Assert.True(File.Exists(schemaPath), $"File was not found: {schemaPath}");
        Assert.True(File.Exists(querySeedPath), $"File was not found: {querySeedPath}");

        string schemaText =
            File.ReadAllText(schemaPath);

        string querySeedText =
            File.ReadAllText(querySeedPath);

        Assert.Contains("CREATE TABLE IF NOT EXISTS [SqlQuery]", schemaText);
        Assert.Contains("CREATE TABLE IF NOT EXISTS [Website]", schemaText);
        Assert.Contains("Website.Select.Enabled", querySeedText);
        Assert.Contains("Website.Select.Search.Enabled", querySeedText);
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
