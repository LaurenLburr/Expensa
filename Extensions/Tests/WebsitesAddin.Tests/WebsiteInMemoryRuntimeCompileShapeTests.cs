using Xunit;

namespace WebsitesAddin.Tests;

public sealed class WebsiteInMemoryRuntimeCompileShapeTests
{
    [Fact]
    public void Repository_UsesExplicitConnectionSelectionInsteadOfInvalidCoalescingAssignment()
    {
        string text = ReadFile(
            "Extensions",
            "Modules",
            "WebsitesAddin",
            "SqliteWebsiteRepository.cs");

        Assert.Contains("if (_options.Connection is not null)", text);
        Assert.Contains("_ownedConnection ??= new SqliteConnection", text);
        Assert.DoesNotContain("_options.Connection ?? _ownedConnection ??=", text);
    }

    [Fact]
    public void LoadCommand_DoesNotAssignReadOnlyTotalCount()
    {
        string text = ReadFile(
            "Extensions",
            "Modules",
            "WebsitesAddin",
            "WebsiteLoadCommand.cs");

        Assert.DoesNotContain("TotalCount =", text);
        Assert.Contains("Nodes = nodes", text);
        Assert.Contains("WebsiteInMemoryDatabaseFactory.OpenMemoryCopy", text);
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
