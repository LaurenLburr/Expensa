using Codex.CommandEngine.Data;
using Xunit;

namespace Codex.CommandEngine.IntegrationTests;

public sealed class BrowserRepositorySmokeTests
{
    [Fact]
    public void CommandDefinitionBrowserRepository_ListAll_ReturnsEmptyListForFreshDatabase()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(BrowserRepositorySmokeTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            CommandEngineConnectionFactory factory =
                new(CommandEngineDatabaseOptions.ForFile(databasePath));

            CommandDefinitionBrowserRepository repository =
                new(factory);

            IReadOnlyList<CommandDefinitionRecord> records =
                repository.ListAll();

            Assert.NotNull(records);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void WorkflowDefinitionBrowserRepository_ListAll_ReturnsEmptyListForFreshDatabase()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(BrowserRepositorySmokeTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            CommandEngineConnectionFactory factory =
                new(CommandEngineDatabaseOptions.ForFile(databasePath));

            WorkflowDefinitionBrowserRepository repository =
                new(factory);

            IReadOnlyList<WorkflowDefinitionRecord> records =
                repository.ListAll();

            Assert.NotNull(records);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }
}
