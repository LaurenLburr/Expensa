using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class AddinModuleSqlitePoolingStructureTests
{
    [Theory]
    [InlineData("Modules", "PayeesAddin", "SqlitePayeeRepository.cs")]
    [InlineData("Modules", "PayeesAddin", "PayeeScreenRuntimeRunner.cs")]
    [InlineData("Modules", "BudgetsAddin", "BudgetScreenRuntimeRunner.cs")]
    [InlineData("Modules", "WebsitesAddin", "WebsiteInMemoryDatabaseFactory.cs")]
    [InlineData("Modules", "WebsitesAddin", "SqliteWebsiteRepository.cs")]
    [InlineData("Modules", "WebsitesAddin", "WebsiteSqlQueryCatalog.cs")]
    public void DirectAddinSqliteConnections_DisablePooling(
        params string[] pathParts)
    {
        string text = ReadFile(pathParts);

        Assert.Contains("Pooling=False", text);
    }

    [Fact]
    public void WebsitesLoadCommand_UsesMemoryCopyForRuntimeDatabasePath()
    {
        string text = ReadFile("Modules", "WebsitesAddin", "WebsiteLoadCommand.cs");

        Assert.Contains("WebsiteInMemoryDatabaseFactory.OpenMemoryCopy(request.DatabasePath)", text);
        Assert.Contains("Connection = memoryConnection", text);
        Assert.Contains("OwnsConnection = true", text);
    }

    private static string ReadFile(params string[] parts)
    {
        string path = Path.Combine([TestPathHelper.ExtensionsRoot, .. parts]);

        Assert.True(File.Exists(path), $"File was not found: {path}");

        return File.ReadAllText(path);
    }
}
