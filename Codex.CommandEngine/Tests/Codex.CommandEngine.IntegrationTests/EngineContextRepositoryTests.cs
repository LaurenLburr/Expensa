using Codex.CommandEngine.Data;
using Microsoft.Data.Sqlite;
using Xunit;

namespace Codex.CommandEngine.IntegrationTests;

public sealed class EngineContextRepositoryTests
{
    [Fact]
    public void Upsert_ThenFindByName_ReturnsContext()
    {
        string databasePath = CreateTestDatabase();

        try
        {
            EngineContextRepository repository = CreateRepository(databasePath);

            repository.Upsert(new EngineContextUpsert
            {
                ContextId = "context-001",
                ContextName = "active.solution",
                Scope = "Solution",
                Description = "Current solution context for command execution.",
                ContextJson = "{\"root\":\"D:/Git/CodexExpensa/Codex.CommandEngine\"}",
                MetadataJson = "{\"source\":\"test\"}",
                IsEnabled = true
            });

            EngineContextRecord? record = repository.FindByName("active.solution");

            Assert.NotNull(record);
            Assert.Equal("context-001", record.ContextId);
            Assert.Equal("Solution", record.Scope);
            Assert.Equal("Current solution context for command execution.", record.Description);
            Assert.Contains("Codex.CommandEngine", record.ContextJson, StringComparison.Ordinal);
            Assert.True(record.IsEnabled);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void Upsert_WithSameName_UpdatesContext()
    {
        string databasePath = CreateTestDatabase();

        try
        {
            EngineContextRepository repository = CreateRepository(databasePath);

            repository.Upsert(new EngineContextUpsert
            {
                ContextId = "context-001",
                ContextName = "active.project",
                Scope = "Project",
                Description = "Original description.",
                ContextJson = "{\"project\":\"old\"}",
                MetadataJson = "{}",
                IsEnabled = true
            });

            repository.Upsert(new EngineContextUpsert
            {
                ContextId = "context-001",
                ContextName = "active.project",
                Scope = "Project",
                Description = "Updated description.",
                ContextJson = "{\"project\":\"new\"}",
                MetadataJson = "{\"updated\":true}",
                IsEnabled = false
            });

            EngineContextRecord? record = repository.FindByName("active.project");

            Assert.NotNull(record);
            Assert.Equal("Updated description.", record.Description);
            Assert.Contains("new", record.ContextJson, StringComparison.Ordinal);
            Assert.False(record.IsEnabled);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void ListAll_ReturnsContextsOrderedByScopeThenName()
    {
        string databasePath = CreateTestDatabase();

        try
        {
            EngineContextRepository repository = CreateRepository(databasePath);

            repository.Upsert(new EngineContextUpsert
            {
                ContextId = "context-zeta",
                ContextName = "zeta.context",
                Scope = "Workflow",
                Description = "Workflow context.",
                ContextJson = "{}",
                MetadataJson = "{}",
                IsEnabled = true
            });

            repository.Upsert(new EngineContextUpsert
            {
                ContextId = "context-alpha",
                ContextName = "alpha.context",
                Scope = "Solution",
                Description = "Solution context.",
                ContextJson = "{}",
                MetadataJson = "{}",
                IsEnabled = true
            });

            IReadOnlyList<EngineContextRecord> records = repository.ListAll();

            Assert.Contains(records, x => x.ContextName == "alpha.context");
            Assert.Contains(records, x => x.ContextName == "zeta.context");
            Assert.True(FindIndex(records, "alpha.context") < FindIndex(records, "zeta.context"));
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void TemplateDatabase_ContainsContextSystemCatalogQueries()
    {
        string databasePath = CreateTestDatabase();

        try
        {
            using SqliteConnection connection = TestDatabasePaths.OpenConnection(databasePath);
            long count = CountRows(
                connection,
                "SELECT COUNT(*) FROM SqlQuery WHERE Category = 'Execution Context Foundation';");

            Assert.True(count >= 5);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }


    private static EngineContextRepository CreateRepository(string databasePath)
    {
        CommandEngineConnectionFactory factory = TestDatabasePaths.CreateConnectionFactory(databasePath);
        return new EngineContextRepository(factory);
    }

    private static string CreateTestDatabase()
    {
        return TestDatabasePaths.ResetDatabaseForTestClass(nameof(EngineContextRepositoryTests));
    }

    private static int FindIndex(IReadOnlyList<EngineContextRecord> records, string contextName)
    {
        for (int index = 0; index < records.Count; index++)
        {
            if (string.Equals(records[index].ContextName, contextName, StringComparison.Ordinal))
            {
                return index;
            }
        }

        return -1;
    }

    private static long CountRows(SqliteConnection connection, string sql)
    {
        return Convert.ToInt64(ExecuteScalar(connection, sql), System.Globalization.CultureInfo.InvariantCulture);
    }

    private static object ExecuteScalar(SqliteConnection connection, string sql)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;
        return command.ExecuteScalar()
            ?? throw new InvalidOperationException($"Query returned no value: {sql}");
    }
}
