using Codex.CommandEngine.Data;
using Xunit;

namespace Codex.CommandEngine.IntegrationTests;

public sealed class RepositorySchemaOwnershipTests
{
    [Fact]
    public void SqlQueryCatalog_WhenDatabaseIsEmpty_DoesNotCreateSchema()
    {
        CommandEngineConnectionFactory factory =
            CreateEmptyDatabaseFactory();

        DatabaseSchemaException exception =
            Assert.Throws<DatabaseSchemaException>(
                () => new SqlQueryCatalog(factory));

        Assert.Contains("Required database table 'SqlQuery' was not found", exception.Message);
    }

    [Fact]
    public void SqlCatalogRepository_WhenDatabaseIsEmpty_DoesNotCreateSchema()
    {
        CommandEngineConnectionFactory factory =
            CreateEmptyDatabaseFactory();

        DatabaseSchemaException exception =
            Assert.Throws<DatabaseSchemaException>(
                () => new SqlCatalogRepository(factory));

        Assert.Contains("Required database table 'SqlQuery' was not found", exception.Message);
    }

    [Fact]
    public void CommandDefinitionBrowserRepository_WhenDatabaseIsEmpty_DoesNotCreateSchema()
    {
        CommandEngineConnectionFactory factory =
            CreateEmptyDatabaseFactory();

        DatabaseSchemaException exception =
            Assert.Throws<DatabaseSchemaException>(
                () => new CommandDefinitionBrowserRepository(factory));

        Assert.Contains("Required database table 'CommandDefinition' was not found", exception.Message);
    }

    private static CommandEngineConnectionFactory CreateEmptyDatabaseFactory()
    {
        string databasePath =
            Path.Combine(
                Path.GetTempPath(),
                "Codex.CommandEngine.Tests",
                nameof(RepositorySchemaOwnershipTests),
                Guid.NewGuid().ToString("N"),
                "empty.db");

        Directory.CreateDirectory(Path.GetDirectoryName(databasePath)!);

        return TestDatabasePaths.CreateConnectionFactory(databasePath);
    }
}
