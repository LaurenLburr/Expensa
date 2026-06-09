using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteExpensaProdDatabaseCopyServiceDynamicColumnTests
{
    [Fact]
    public void CopyService_UsesExistingExpensaWebsiteTagSchema()
    {
        string text = ReadFile(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Websites",
            "HostWebsiteExpensaProdDatabaseCopyService.cs");

        Assert.Contains("ValidateRequiredSourceTables", text);
        Assert.Contains("Website", text);
        Assert.Contains("Tag", text);
        Assert.Contains("TagAssignment", text);
        Assert.Contains("FROM [prod].[Website]", text);
        Assert.Contains("FROM [prod].[Tag]", text);
        Assert.Contains("FROM [prod].[TagAssignment]", text);
        Assert.Contains("WHERE [EntityType] = 'Website'", text);
    }

    [Fact]
    public void CopyService_NoLongerUsesDynamicColumnGuessing()
    {
        string text = ReadFile(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Websites",
            "HostWebsiteExpensaProdDatabaseCopyService.cs");

        Assert.DoesNotContain("GetSourceColumns", text);
        Assert.DoesNotContain("BuildFirstExistingColumnExpression", text);
        Assert.DoesNotContain("COALESCE(NULLIF(TRIM([DisplayName])", text);
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
