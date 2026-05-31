using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteExpensaProdDatabaseCopyServiceDynamicColumnTests
{
    [Fact]
    public void CopyService_InspectsSourceColumnsBeforeBuildingImportSql()
    {
        string text = ReadFile(
            "Extensions",
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Websites",
            "HostWebsiteExpensaProdDatabaseCopyService.cs");

        Assert.Contains("GetSourceColumns", text);
        Assert.Contains("pragma_table_info", text);
        Assert.Contains("group_concat([name], '|')", text);
        Assert.Contains("BuildFirstExistingColumnExpression", text);
        Assert.Contains("WebsiteName", text);
        Assert.Contains("WebsiteURL", text);
        Assert.Contains("Uncategorized", text);
    }

    [Fact]
    public void CopyService_DoesNotHardcodeDisplayNameColumnInSelectExpression()
    {
        string text = ReadFile(
            "Extensions",
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Websites",
            "HostWebsiteExpensaProdDatabaseCopyService.cs");

        Assert.DoesNotContain("NULLIF(TRIM([DisplayName])", text);
        Assert.DoesNotContain("COALESCE(NULLIF(TRIM([Name])", text);
    }

    [Fact]
    public void CopyService_DoesNotGenerateSingleArgumentCoalesce()
    {
        string text = ReadFile(
            "Extensions",
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Websites",
            "HostWebsiteExpensaProdDatabaseCopyService.cs");

        Assert.Contains("if (expressions.Count == 0)", text);
        Assert.Contains("return fallbackExpression;", text);
        Assert.Contains("if (expressions.Count == 1)", text);
        Assert.Contains("return expressions[0];", text);
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
