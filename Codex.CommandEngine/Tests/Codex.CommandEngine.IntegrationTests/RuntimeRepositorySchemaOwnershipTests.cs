using Codex.CommandEngine.Data;
using Xunit;

namespace Codex.CommandEngine.IntegrationTests;

public sealed class RuntimeRepositorySchemaOwnershipTests
{
    [Fact]
    public void ExecutionHistoryRepository_WhenDatabaseIsEmpty_DoesNotCreateSchema()
    {
        CommandEngineConnectionFactory factory =
            CreateEmptyDatabaseFactory();

        DatabaseSchemaException exception =
            Assert.Throws<DatabaseSchemaException>(
                () => new ExecutionHistoryRepository(factory));

        Assert.Contains("Required database table 'ExecutionHistory' was not found", exception.Message);
    }

    [Fact]
    public void WorkflowExecutionRepository_WhenDatabaseIsEmpty_DoesNotCreateSchema()
    {
        CommandEngineConnectionFactory factory =
            CreateEmptyDatabaseFactory();

        DatabaseSchemaException exception =
            Assert.Throws<DatabaseSchemaException>(
                () => new WorkflowExecutionRepository(factory));

        Assert.Contains("Required database table 'WorkflowExecution' was not found", exception.Message);
    }

    private static CommandEngineConnectionFactory CreateEmptyDatabaseFactory()
    {
        string databasePath =
            Path.Combine(
                Path.GetTempPath(),
                "Codex.CommandEngine.Tests",
                nameof(RuntimeRepositorySchemaOwnershipTests),
                Guid.NewGuid().ToString("N"),
                "empty.db");

        Directory.CreateDirectory(Path.GetDirectoryName(databasePath)!);

        return TestDatabasePaths.CreateConnectionFactory(databasePath);
    }
}
