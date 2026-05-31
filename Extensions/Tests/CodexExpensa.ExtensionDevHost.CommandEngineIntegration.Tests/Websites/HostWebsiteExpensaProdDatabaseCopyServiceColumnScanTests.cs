using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteExpensaProdDatabaseCopyServiceColumnScanTests
{
    [Fact]
    public void CopyService_ColumnScanDoesNotUseDataTableLoadForPragmaInfo()
    {
        string text = ReadFile(
            "Extensions",
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Websites",
            "HostWebsiteExpensaProdDatabaseCopyService.cs");

        Assert.Contains("pragma_table_info", text);
        Assert.Contains("group_concat([name], '|')", text);
        Assert.Contains("ExecuteScalar", text);
        Assert.DoesNotContain("PRAGMA table_info", text);
        Assert.DoesNotContain("table.Load(reader)", text);
        Assert.DoesNotContain("DataTable table", text);
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
