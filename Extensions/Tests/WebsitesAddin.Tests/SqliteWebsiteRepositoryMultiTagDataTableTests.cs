using Xunit;

namespace WebsitesAddin.Tests;

public sealed class SqliteWebsiteRepositoryMultiTagDataTableTests
{
    [Fact]
    public void Repository_ManuallyLoadsTreeQueryIntoUnconstrainedDataTable()
    {
        string text = ReadFile(
            "Extensions",
            "Modules",
            "WebsitesAddin",
            "SqliteWebsiteRepository.cs");

        Assert.Contains("ExecuteToUnconstrainedDataTable", text);
        Assert.Contains("table.Columns.Add", text);
        Assert.Contains("table.BeginLoadData();", text);
        Assert.Contains("table.EndLoadData();", text);
        Assert.DoesNotContain("table.Load(reader)", text);
    }

    [Fact]
    public void Repository_StillGroupsWebsitesByExpensaTagName()
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
