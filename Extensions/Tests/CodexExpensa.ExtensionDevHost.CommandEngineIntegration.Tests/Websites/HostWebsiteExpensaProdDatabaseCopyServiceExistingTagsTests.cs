using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteExpensaProdDatabaseCopyServiceExistingTagsTests
{
    [Fact]
    public void CopyService_CopiesExpensaTagAndTagAssignmentTables()
    {
        string text = ReadFile(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Websites",
            "HostWebsiteExpensaProdDatabaseCopyService.cs");

        Assert.Contains("CREATE TABLE IF NOT EXISTS [Tag]", text);
        Assert.Contains("CREATE TABLE IF NOT EXISTS [TagAssignment]", text);
        Assert.Contains("FROM [prod].[Tag]", text);
        Assert.Contains("FROM [prod].[TagAssignment]", text);
        Assert.Contains("WHERE [EntityType] = 'Website'", text);
        Assert.DoesNotContain("CREATE TABLE IF NOT EXISTS [WebsiteTag]", text);
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
