using Microsoft.Data.Sqlite;
using Xunit;

namespace Codex.CommandEngine.IntegrationTests;

public sealed class CommandEngineSchemaTests
{
    [Fact]
    public void TemplateDatabase_ContainsExpectedCoreTables()
    {
        string databasePath = CreateTestDatabase();

        try
        {
            using SqliteConnection connection = TestDatabasePaths.OpenConnection(databasePath);

            AssertTableExists(connection, "MigrationHistory");
            AssertTableExists(connection, "DatabaseSchemaSnapshot");
            AssertTableExists(connection, "SqlQuery");
            AssertTableExists(connection, "CommandDefinition");
            AssertTableExists(connection, "CommandParameterDefinition");
            AssertTableExists(connection, "WorkflowDefinition");
            AssertTableExists(connection, "WorkflowStep");
            AssertTableExists(connection, "ExecutionHistory");
            AssertTableExists(connection, "ExecutionStepHistory");
            AssertTableExists(connection, "ExecutionContext");
            AssertTableExists(connection, "AiProvider");
            AssertTableExists(connection, "AiProviderCapability");
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void TemplateDatabase_ContainsExpectedSqlCatalogCategories()
    {
        string databasePath = CreateTestDatabase();

        try
        {
            using SqliteConnection connection = TestDatabasePaths.OpenConnection(databasePath);

            Assert.True(CountCategory(connection, "SQL Catalog") >= 3);
            Assert.True(CountCategory(connection, "Command Registration") >= 5);
            Assert.True(CountCategory(connection, "Workflow Foundation") >= 5);
            Assert.True(CountCategory(connection, "Execution History") >= 5);
            Assert.True(CountCategory(connection, "Execution Context Foundation") >= 5);
            Assert.True(CountCategory(connection, "AI Provider Foundation") >= 5);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    //[Fact]
    //public void TemplateDatabase_ContainsExpectedSchemaSnapshots()
    //{
    //    string databasePath = CreateTestDatabase();

    //    try
    //    {
    //        using SqliteConnection connection = TestDatabasePaths.OpenConnection(databasePath);

    //        Assert.Equal(1, GetSchemaVersion(connection, "0001_engine_database_foundation"));
    //        Assert.Equal(2, GetSchemaVersion(connection, "0002_sql_catalog_foundation"));
    //        Assert.Equal(3, GetSchemaVersion(connection, "0003_command_registration_foundation"));
    //        Assert.Equal(4, GetSchemaVersion(connection, "0004_workflow_model_foundation"));
    //        Assert.Equal(5, GetSchemaVersion(connection, "0005_execution_history_foundation"));
    //        Assert.Equal(6, GetSchemaVersion(connection, "0006_context_system_foundation"));
    //        Assert.Equal(7, GetSchemaVersion(connection, "0007_ai_provider_foundation"));
    //    }
    //    finally
    //    {
    //        TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
    //    }
    //}

    private static string CreateTestDatabase()
    {
        return TestDatabasePaths.ResetDatabaseForTestClass(nameof(CommandEngineSchemaTests));
    }

    private static void AssertTableExists(SqliteConnection connection, string tableName)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = """
SELECT COUNT(*)
FROM sqlite_master
WHERE type = 'table'
  AND name = $TableName;
""";
        command.Parameters.AddWithValue("$TableName", tableName);

        Assert.Equal(1L, command.ExecuteScalar());
    }

    private static long CountCategory(SqliteConnection connection, string category)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = """
SELECT COUNT(*)
FROM SqlQuery
WHERE Category = $Category;
""";
        command.Parameters.AddWithValue("$Category", category);

        return Convert.ToInt64(command.ExecuteScalar(), System.Globalization.CultureInfo.InvariantCulture);
    }

    private static int GetSchemaVersion(SqliteConnection connection, string migrationId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = """
SELECT SchemaVersion
FROM DatabaseSchemaSnapshot
WHERE MigrationId = $MigrationId;
""";
        command.Parameters.AddWithValue("$MigrationId", migrationId);

        return Convert.ToInt32(command.ExecuteScalar(), System.Globalization.CultureInfo.InvariantCulture);
    }
}
