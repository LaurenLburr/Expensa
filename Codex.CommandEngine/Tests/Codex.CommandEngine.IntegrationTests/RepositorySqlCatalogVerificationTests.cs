using Microsoft.Data.Sqlite;
using Xunit;

namespace Codex.CommandEngine.IntegrationTests;

public sealed class RepositorySqlCatalogVerificationTests
{
    public static TheoryData<string> RequiredRepositoryQueryNames()
    {
        return new TheoryData<string>
        {
            // SQL catalog infrastructure
            "SqlQuery_SelectByName",
            "SqlQuery_SelectAll",
            "SqlQuery_InsertOrReplace",

            // AI provider foundation
            "AiProvider_InsertOrReplace",
            "AiProvider_SelectByName",
            "AiProvider_SelectAll",
            "AiProviderCapability_InsertOrReplace",
            "AiProviderCapability_SelectByProvider",

            // Command registration
            "CommandDefinition_InsertOrReplace",
            "CommandDefinition_SelectByName",
            "CommandDefinition_SelectAll",
            "CommandParameterDefinition_InsertOrReplace",
            "CommandParameterDefinition_SelectByCommand",

            // Context system
            "EngineContext_InsertOrReplace",
            "EngineContext_SelectByName",
            "EngineContext_SelectAll",

            // Workflow foundation
            "WorkflowDefinition_InsertOrReplace",
            "WorkflowDefinition_SelectByNameAndVersion",
            "WorkflowDefinition_SelectAll",
            "WorkflowStep_InsertOrReplace",
            "WorkflowStep_SelectByWorkflow",

            // Execution history
            "ExecutionHistory_Insert",
            "ExecutionHistory_Complete",
            "ExecutionHistory_SelectById",
            "ExecutionHistory_SelectRecent",
            "ExecutionStepHistory_Insert",
            "ExecutionStepHistory_Complete",
            "ExecutionStepHistory_SelectByExecution"
        };
    }

    [Theory]
    [MemberData(nameof(RequiredRepositoryQueryNames))]
    public void TemplateDatabase_ContainsRequiredRepositoryQuery(string queryName)
    {
        string databasePath = CreateTestDatabase();

        try
        {
            using SqliteConnection connection = TestDatabasePaths.OpenConnection(databasePath);

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = """
SELECT COUNT(*)
FROM SqlQuery
WHERE QueryName = $QueryName
  AND IsActive = 1
  AND trim(SqlText) <> '';
""";
            command.Parameters.AddWithValue("$QueryName", queryName);

            long count = Convert.ToInt64(
                command.ExecuteScalar(),
                System.Globalization.CultureInfo.InvariantCulture);

            Assert.Equal(1L, count);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void TemplateDatabase_ContainsNoDuplicateQueryNames()
    {
        string databasePath = CreateTestDatabase();

        try
        {
            using SqliteConnection connection = TestDatabasePaths.OpenConnection(databasePath);

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = """
SELECT COUNT(*)
FROM (
    SELECT QueryName
    FROM SqlQuery
    GROUP BY QueryName
    HAVING COUNT(*) > 1
);
""";

            long duplicateCount = Convert.ToInt64(
                command.ExecuteScalar(),
                System.Globalization.CultureInfo.InvariantCulture);

            Assert.Equal(0L, duplicateCount);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void TemplateDatabase_RequiredRepositoryQueriesHaveExpectedCategories()
    {
        string databasePath = CreateTestDatabase();

        try
        {
            using SqliteConnection connection = TestDatabasePaths.OpenConnection(databasePath);

            AssertQueryCategory(connection, "AiProvider_InsertOrReplace", "AI Provider Foundation");
            AssertQueryCategory(connection, "AiProviderCapability_InsertOrReplace", "AI Provider Foundation");
            AssertQueryCategory(connection, "CommandDefinition_InsertOrReplace", "Command Registration");
            AssertQueryCategory(connection, "CommandParameterDefinition_InsertOrReplace", "Command Registration");
            AssertQueryCategory(connection, "EngineContext_InsertOrReplace", "Context System Foundation");
            AssertQueryCategory(connection, "WorkflowDefinition_InsertOrReplace", "Workflow Foundation");
            AssertQueryCategory(connection, "WorkflowStep_InsertOrReplace", "Workflow Foundation");
            AssertQueryCategory(connection, "ExecutionHistory_Insert", "Execution History");
            AssertQueryCategory(connection, "ExecutionStepHistory_Insert", "Execution History");
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void TemplateDatabase_AllRequiredRepositoryQueriesPrepareSuccessfully()
    {
        string databasePath = CreateTestDatabase();

        try
        {
            using SqliteConnection connection = TestDatabasePaths.OpenConnection(databasePath);

            foreach (string queryName in RequiredRepositoryQueryNames())
            {
                string sqlText = GetRequiredSqlText(connection, queryName);

                using SqliteCommand command = connection.CreateCommand();
                command.CommandText = $"EXPLAIN {sqlText}";

                AddKnownParameters(command);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqliteException exception)
                {
                    throw new InvalidOperationException(
                        $"Catalog query failed to prepare: {queryName}",
                        exception);
                }
            }
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    private static string CreateTestDatabase()
    {
        return TestDatabasePaths.ResetDatabaseForTestClass(nameof(RepositorySqlCatalogVerificationTests));
    }

    private static void AssertQueryCategory(
        SqliteConnection connection,
        string queryName,
        string expectedCategory)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = """
SELECT Category
FROM SqlQuery
WHERE QueryName = $QueryName
  AND IsActive = 1;
""";
        command.Parameters.AddWithValue("$QueryName", queryName);

        string actualCategory = Convert.ToString(
            command.ExecuteScalar(),
            System.Globalization.CultureInfo.InvariantCulture)
            ?? string.Empty;

        Assert.Equal(expectedCategory, actualCategory);
    }

    private static string GetRequiredSqlText(SqliteConnection connection, string queryName)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = """
SELECT SqlText
FROM SqlQuery
WHERE QueryName = $QueryName
  AND IsActive = 1;
""";
        command.Parameters.AddWithValue("$QueryName", queryName);

        string? sqlText = Convert.ToString(
            command.ExecuteScalar(),
            System.Globalization.CultureInfo.InvariantCulture);

        if (string.IsNullOrWhiteSpace(sqlText))
        {
            throw new InvalidOperationException($"Required SQL query was not found: {queryName}");
        }

        return sqlText;
    }

    private static void AddKnownParameters(SqliteCommand command)
    {
        command.Parameters.AddWithValue("$QueryName", "Verification.Query");
        command.Parameters.AddWithValue("$Description", "Verification description.");
        command.Parameters.AddWithValue("$SqlText", "SELECT 1;");
        command.Parameters.AddWithValue("$Category", "Verification");
        command.Parameters.AddWithValue("$IsActive", 1);

        command.Parameters.AddWithValue("$AiProviderId", "provider-verification");
        command.Parameters.AddWithValue("$ProviderName", "provider.verification");
        command.Parameters.AddWithValue("$DisplayName", "Provider Verification");
        command.Parameters.AddWithValue("$ProviderKind", "Verification");
        command.Parameters.AddWithValue("$ConfigurationJson", "{}");
        command.Parameters.AddWithValue("$MetadataJson", "{}");
        command.Parameters.AddWithValue("$CapabilityName", "verification.capability");
        command.Parameters.AddWithValue("$AiProviderCapabilityId", "provider-capability-verification");

        command.Parameters.AddWithValue("$CommandId", "command-verification");
        command.Parameters.AddWithValue("$CommandDefinitionId", "command-verification");
        command.Parameters.AddWithValue("$CommandName", "verification.command");
        command.Parameters.AddWithValue("$HandlerType", "VerificationHandler");
        command.Parameters.AddWithValue("$HandlerKey", "VerificationHandler");
        command.Parameters.AddWithValue("$InputJson", "{}");
        command.Parameters.AddWithValue("$OutputJson", "{}");

        command.Parameters.AddWithValue("$CommandParameterDefinitionId", "command-parameter-verification");
        command.Parameters.AddWithValue("$ParameterName", "verificationParameter");
        command.Parameters.AddWithValue("$ParameterType", "string");
        command.Parameters.AddWithValue("$IsRequired", 1);
        command.Parameters.AddWithValue("$DefaultValue", string.Empty);
        command.Parameters.AddWithValue("$SortOrder", 1);

        command.Parameters.AddWithValue("$ContextId", "context-verification");
        command.Parameters.AddWithValue("$ContextName", "verification.context");
        command.Parameters.AddWithValue("$Scope", "Verification");
        command.Parameters.AddWithValue("$ContextJson", "{}");

        command.Parameters.AddWithValue("$WorkflowId", "workflow-verification");
        command.Parameters.AddWithValue("$WorkflowDefinitionId", "workflow-verification");
        command.Parameters.AddWithValue("$WorkflowName", "verification.workflow");
        command.Parameters.AddWithValue("$Version", 1);
        command.Parameters.AddWithValue("$WorkflowStepId", "workflow-step-verification");
        command.Parameters.AddWithValue("$WorkflowStepDefinitionId", "workflow-step-verification");
        command.Parameters.AddWithValue("$StepOrder", 1);
        command.Parameters.AddWithValue("$StepName", "Verification Step");

        command.Parameters.AddWithValue("$ExecutionId", "execution-verification");
        command.Parameters.AddWithValue("$ExecutionKind", "Verification");
        command.Parameters.AddWithValue("$TargetId", "target-verification");
        command.Parameters.AddWithValue("$TargetName", "Verification Target");
        command.Parameters.AddWithValue("$Status", "Verification");
        command.Parameters.AddWithValue("$StartedUtc", "2026-01-01T00:00:00Z");
        command.Parameters.AddWithValue("$CompletedUtc", "2026-01-01T00:00:01Z");
        command.Parameters.AddWithValue("$CorrelationId", "correlation-verification");
        command.Parameters.AddWithValue("$ErrorMessage", string.Empty);
        command.Parameters.AddWithValue("$Limit", 10);

        command.Parameters.AddWithValue("$ExecutionStepId", "execution-step-verification");
        command.Parameters.AddWithValue("$WorkflowStepId", DBNull.Value);
    }
}
