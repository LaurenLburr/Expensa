using Xunit;

namespace WebsitesAddin.Tests;

public sealed class SqliteWebsiteRepositoryExistingTagsTests
{
    [Fact]
    public void Repository_GroupsWebsitesUsingExpensaTagAssignments()
    {
        string text = ReadFile(
            "Extensions",
            "Modules",
            "WebsitesAddin",
            "SqliteWebsiteRepository.cs");

        Assert.Contains("LEFT JOIN [TagAssignment] ta", text);
        Assert.Contains("ta.[EntityType] = 'Website'", text);
        Assert.Contains("LEFT JOIN [Tag] t", text);
        Assert.Contains("COALESCE(t.[TagName], 'Uncategorized') AS [TagName]", text);
        Assert.Contains("childrenByTag", text);
        Assert.DoesNotContain("[Category]", text);
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
