using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteExpensaProdDatabaseCopyServiceColumnScanTests
{
    [Fact]
    public void CopyService_DoesNotUsePragmaColumnScanNowThatExpensaSchemaIsKnown()
    {
        string text = ReadFile(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Websites",
            "HostWebsiteExpensaProdDatabaseCopyService.cs");

        Assert.Contains("ValidateRequiredSourceTables", text);
        Assert.Contains("Expensa production database is missing required table", text);
        Assert.DoesNotContain("pragma_table_info", text);
        Assert.DoesNotContain("group_concat([name], '|')", text);
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
        return TestPathHelper.ExtensionsRoot;
    }
}
