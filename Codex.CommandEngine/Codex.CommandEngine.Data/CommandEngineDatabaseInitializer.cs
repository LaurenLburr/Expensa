using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;

namespace Codex.CommandEngine.Data;

public sealed class CommandEngineDatabaseInitializer
{
    private readonly CommandEngineConnectionFactory _connectionFactory;

    public CommandEngineDatabaseInitializer(CommandEngineConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);

        _connectionFactory = connectionFactory;
    }

    public void EnsureCreated()
    {
        using SqliteConnection connection = _connectionFactory.OpenConnection();

        if (!_connectionFactory.ShouldRunInitializerAfterOpen)
        {
            return;
        }

        EnsureCreated(connection);
    }

    public static void EnsureCreated(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        if (connection.State != System.Data.ConnectionState.Open)
        {
            connection.Open();
        }

        using SqliteTransaction transaction = connection.BeginTransaction();

        try
        {
            ExecuteNonQuery(connection, transaction, CommandEngineSchema.SchemaSql);
            EnsureCompatibilityColumns(connection, transaction);
            ExecuteNonQuery(connection, transaction, CommandEngineSchema.IndexSql);

            SeedMigrationRows(connection, transaction);
            SeedSchemaSnapshotRows(connection, transaction);
            SeedSqlCatalog(connection, transaction);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private static void EnsureCompatibilityColumns(SqliteConnection connection, SqliteTransaction transaction)
    {
        EnsureColumn(connection, transaction, "SqlQuery", "UpdatedUtc", "TEXT NULL");

        EnsureColumn(connection, transaction, "CommandDefinition", "DisplayName", "TEXT NOT NULL DEFAULT ''");
        EnsureColumn(connection, transaction, "CommandDefinition", "HandlerType", "TEXT NOT NULL DEFAULT ''");

        EnsureColumn(connection, transaction, "WorkflowDefinition", "DisplayName", "TEXT NOT NULL DEFAULT ''");
        EnsureColumn(connection, transaction, "WorkflowStep", "StepName", "TEXT NOT NULL DEFAULT ''");

        EnsureColumn(connection, transaction, "ExecutionHistory", "CorrelationId", "TEXT NOT NULL DEFAULT ''");

        EnsureColumn(connection, transaction, "ExecutionContext", "Scope", "TEXT NOT NULL DEFAULT ''");
        EnsureColumn(connection, transaction, "ExecutionContext", "Description", "TEXT NOT NULL DEFAULT ''");
        EnsureColumn(connection, transaction, "ExecutionContext", "MetadataJson", "TEXT NOT NULL DEFAULT '{}'");
        EnsureColumn(connection, transaction, "ExecutionContext", "IsEnabled", "INTEGER NOT NULL DEFAULT 1");

        EnsureColumn(connection, transaction, "AiProvider", "DisplayName", "TEXT NOT NULL DEFAULT ''");
        EnsureColumn(connection, transaction, "AiProvider", "Description", "TEXT NOT NULL DEFAULT ''");
        EnsureColumn(connection, transaction, "AiProvider", "MetadataJson", "TEXT NOT NULL DEFAULT '{}'");
    }

    private static void SeedMigrationRows(SqliteConnection connection, SqliteTransaction transaction)
    {
        SeedMigration(connection, transaction, CommandEngineSchema.BaselineMigrationId, CommandEngineSchema.BaselineMigrationDescription);
        SeedMigration(connection, transaction, CommandEngineSchema.SqlCatalogMigrationId, CommandEngineSchema.SqlCatalogMigrationDescription);
        SeedMigration(connection, transaction, CommandEngineSchema.CommandRegistrationMigrationId, CommandEngineSchema.CommandRegistrationMigrationDescription);
        SeedMigration(connection, transaction, CommandEngineSchema.WorkflowFoundationMigrationId, CommandEngineSchema.WorkflowFoundationMigrationDescription);
        SeedMigration(connection, transaction, CommandEngineSchema.ExecutionHistoryMigrationId, CommandEngineSchema.ExecutionHistoryMigrationDescription);
        SeedMigration(connection, transaction, CommandEngineSchema.ContextSystemMigrationId, CommandEngineSchema.ContextSystemMigrationDescription);
        SeedMigration(connection, transaction, CommandEngineSchema.AiProviderFoundationMigrationId, CommandEngineSchema.AiProviderFoundationMigrationDescription);
    }

    private static void SeedSchemaSnapshotRows(SqliteConnection connection, SqliteTransaction transaction)
    {
        SeedSchemaSnapshot(connection, transaction, CommandEngineSchema.BaselineMigrationId, CommandEngineSchema.CurrentSchemaVersion, CommandEngineSchema.SchemaSql, CommandEngineSchema.BaselineMigrationDescription);
        SeedSchemaSnapshot(connection, transaction, CommandEngineSchema.SqlCatalogMigrationId, CommandEngineSchema.CurrentSchemaVersion, CommandEngineSchema.SchemaSql, CommandEngineSchema.SqlCatalogMigrationDescription);
        SeedSchemaSnapshot(connection, transaction, CommandEngineSchema.CommandRegistrationMigrationId, CommandEngineSchema.CurrentSchemaVersion, CommandEngineSchema.SchemaSql, CommandEngineSchema.CommandRegistrationMigrationDescription);
        SeedSchemaSnapshot(connection, transaction, CommandEngineSchema.WorkflowFoundationMigrationId, CommandEngineSchema.CurrentSchemaVersion, CommandEngineSchema.SchemaSql, CommandEngineSchema.WorkflowFoundationMigrationDescription);
        SeedSchemaSnapshot(connection, transaction, CommandEngineSchema.ExecutionHistoryMigrationId, CommandEngineSchema.CurrentSchemaVersion, CommandEngineSchema.SchemaSql, CommandEngineSchema.ExecutionHistoryMigrationDescription);
        SeedSchemaSnapshot(connection, transaction, CommandEngineSchema.ContextSystemMigrationId, 6, CommandEngineSchema.SchemaSql, CommandEngineSchema.ContextSystemMigrationDescription);
        SeedSchemaSnapshot(connection, transaction, CommandEngineSchema.AiProviderFoundationMigrationId, 7, CommandEngineSchema.SchemaSql, CommandEngineSchema.AiProviderFoundationMigrationDescription);
    }

    private static void SeedSqlCatalog(SqliteConnection connection, SqliteTransaction transaction)
    {
        // SQL Catalog
        UpsertSqlCatalog(
            connection,
            transaction,
            "SqlQuery_SelectByName",
            "Selects one SQL catalog entry by name.",
            "SQL Catalog",
            """
SELECT QueryName,
       Description,
       SqlText,
       Category,
       IsActive,
       CreatedUtc,
       UpdatedUtc
FROM SqlQuery
WHERE QueryName = $QueryName;
""");

        UpsertSqlCatalog(
            connection,
            transaction,
            "SqlQuery_SelectAll",
            "Lists all SQL catalog entries.",
            "SQL Catalog",
            """
SELECT QueryName,
       Description,
       SqlText,
       Category,
       IsActive,
       CreatedUtc,
       UpdatedUtc
FROM SqlQuery
ORDER BY Category ASC,
         QueryName ASC;
""");

        UpsertSqlCatalog(
            connection,
            transaction,
            "SqlQuery_InsertOrReplace",
            "Inserts or updates one SQL catalog entry.",
            "SQL Catalog",
            """
INSERT INTO SqlQuery (
    QueryName,
    Description,
    SqlText,
    Category,
    IsActive
)
VALUES (
    $QueryName,
    $Description,
    $SqlText,
    $Category,
    $IsActive
)
ON CONFLICT(QueryName) DO UPDATE SET
    Description = excluded.Description,
    SqlText = excluded.SqlText,
    Category = excluded.Category,
    IsActive = excluded.IsActive,
    UpdatedUtc = CURRENT_TIMESTAMP;
""");

        // Command Registration
        UpsertSqlCatalog(
            connection,
            transaction,
            "CommandDefinition_InsertOrReplace",
            "Inserts or updates a command definition by command name.",
            "Command Registration",
            """
INSERT INTO CommandDefinition (
    CommandId,
    CommandName,
    DisplayName,
    Description,
    Category,
    Version,
    IsEnabled,
    HandlerType
)
VALUES (
    $CommandId,
    $CommandName,
    $DisplayName,
    $Description,
    $Category,
    $Version,
    $IsEnabled,
    $HandlerType
)
ON CONFLICT(CommandName) DO UPDATE SET
    DisplayName = excluded.DisplayName,
    Description = excluded.Description,
    Category = excluded.Category,
    Version = excluded.Version,
    IsEnabled = excluded.IsEnabled,
    HandlerType = excluded.HandlerType,
    UpdatedUtc = CURRENT_TIMESTAMP;
""");

        UpsertSqlCatalog(
            connection,
            transaction,
            "CommandDefinition_SelectByName",
            "Selects a command definition by command name.",
            "Command Registration",
            """
SELECT CommandId,
       CommandName,
       DisplayName,
       Description,
       Category,
       Version,
       IsEnabled,
       HandlerType,
       CreatedUtc,
       UpdatedUtc
FROM CommandDefinition
WHERE CommandName = $CommandName;
""");

        UpsertSqlCatalog(
            connection,
            transaction,
            "CommandDefinition_SelectAll",
            "Lists all command definitions ordered for UI display.",
            "Command Registration",
            """
SELECT CommandId,
       CommandName,
       DisplayName,
       Description,
       Category,
       Version,
       IsEnabled,
       HandlerType,
       CreatedUtc,
       UpdatedUtc
FROM CommandDefinition
ORDER BY Category ASC,
         CommandName ASC;
""");

        UpsertSqlCatalog(
            connection,
            transaction,
            "CommandParameterDefinition_InsertOrReplace",
            "Inserts or updates a command parameter definition.",
            "Command Registration",
            """
INSERT INTO CommandParameterDefinition (
    CommandParameterDefinitionId,
    CommandId,
    ParameterName,
    ParameterType,
    IsRequired,
    DefaultValue,
    Description,
    SortOrder
)
VALUES (
    $CommandParameterDefinitionId,
    $CommandId,
    $ParameterName,
    $ParameterType,
    $IsRequired,
    $DefaultValue,
    $Description,
    $SortOrder
)
ON CONFLICT(CommandId, ParameterName) DO UPDATE SET
    ParameterType = excluded.ParameterType,
    IsRequired = excluded.IsRequired,
    DefaultValue = excluded.DefaultValue,
    Description = excluded.Description,
    SortOrder = excluded.SortOrder,
    UpdatedUtc = CURRENT_TIMESTAMP;
""");

        UpsertSqlCatalog(
            connection,
            transaction,
            "CommandParameterDefinition_SelectByCommand",
            "Lists parameter definitions for a command definition.",
            "Command Registration",
            """
SELECT CommandParameterDefinitionId,
       CommandId,
       ParameterName,
       ParameterType,
       IsRequired,
       DefaultValue,
       Description,
       SortOrder,
       CreatedUtc,
       UpdatedUtc
FROM CommandParameterDefinition
WHERE CommandId = $CommandId
ORDER BY SortOrder ASC,
         ParameterName ASC;
""");

        // Workflow Foundation
        UpsertSqlCatalog(
            connection,
            transaction,
            "WorkflowDefinition_InsertOrReplace",
            "Inserts or updates a workflow definition by workflow name and version.",
            "Workflow Foundation",
            """
INSERT INTO WorkflowDefinition (
    WorkflowId,
    WorkflowName,
    DisplayName,
    Description,
    Version,
    MetadataJson,
    IsEnabled
)
VALUES (
    $WorkflowId,
    $WorkflowName,
    $DisplayName,
    $Description,
    $Version,
    $MetadataJson,
    $IsEnabled
)
ON CONFLICT(WorkflowName, Version) DO UPDATE SET
    DisplayName = excluded.DisplayName,
    Description = excluded.Description,
    MetadataJson = excluded.MetadataJson,
    IsEnabled = excluded.IsEnabled,
    UpdatedUtc = CURRENT_TIMESTAMP;
""");

        UpsertSqlCatalog(
            connection,
            transaction,
            "WorkflowDefinition_SelectAll",
            "Lists workflow definitions ordered for UI display.",
            "Workflow Foundation",
            """
SELECT WorkflowId,
       WorkflowName,
       DisplayName,
       Description,
       Version,
       IsEnabled,
       MetadataJson,
       CreatedUtc,
       UpdatedUtc
FROM WorkflowDefinition
ORDER BY WorkflowName ASC,
         Version DESC;
""");

        UpsertSqlCatalog(
            connection,
            transaction,
            "WorkflowDefinition_SelectByNameAndVersion",
            "Selects a workflow definition by workflow name and version.",
            "Workflow Foundation",
            """
SELECT WorkflowId,
       WorkflowName,
       DisplayName,
       Description,
       Version,
       IsEnabled,
       MetadataJson,
       CreatedUtc,
       UpdatedUtc
FROM WorkflowDefinition
WHERE WorkflowName = $WorkflowName
  AND Version = $Version;
""");

        UpsertSqlCatalog(
            connection,
            transaction,
            "WorkflowStep_InsertOrReplace",
            "Inserts or updates a workflow step by workflow and step order.",
            "Workflow Foundation",
            """
INSERT INTO WorkflowStep (
    WorkflowStepId,
    WorkflowId,
    StepOrder,
    StepName,
    CommandId,
    InputMapJson,
    MetadataJson,
    IsEnabled
)
VALUES (
    $WorkflowStepId,
    $WorkflowId,
    $StepOrder,
    $StepName,
    $CommandId,
    $InputMapJson,
    $MetadataJson,
    $IsEnabled
)
ON CONFLICT(WorkflowId, StepOrder) DO UPDATE SET
    StepName = excluded.StepName,
    CommandId = excluded.CommandId,
    InputMapJson = excluded.InputMapJson,
    MetadataJson = excluded.MetadataJson,
    IsEnabled = excluded.IsEnabled;
""");

        UpsertSqlCatalog(
            connection,
            transaction,
            "WorkflowStep_SelectByWorkflow",
            "Lists workflow steps for one workflow definition.",
            "Workflow Foundation",
            """
SELECT WorkflowStepId,
       WorkflowId,
       StepOrder,
       StepName,
       CommandId,
       InputMapJson,
       MetadataJson,
       IsEnabled
FROM WorkflowStep
WHERE WorkflowId = $WorkflowId
ORDER BY StepOrder ASC;
""");

        // Execution History
        UpsertSqlCatalog(
            connection,
            transaction,
            "ExecutionHistory_Insert",
            "Inserts an execution history record when a command or workflow starts.",
            "Execution History",
            """
INSERT INTO ExecutionHistory (
    ExecutionId,
    ExecutionKind,
    TargetId,
    TargetName,
    Status,
    StartedUtc,
    CorrelationId,
    InputJson,
    OutputJson,
    ErrorMessage,
    MetadataJson
)
VALUES (
    $ExecutionId,
    $ExecutionKind,
    $TargetId,
    $TargetName,
    $Status,
    $StartedUtc,
    $CorrelationId,
    $InputJson,
    $OutputJson,
    $ErrorMessage,
    $MetadataJson
);
""");

        UpsertSqlCatalog(
            connection,
            transaction,
            "ExecutionHistory_Complete",
            "Completes an execution history record by execution id.",
            "Execution History",
            """
UPDATE ExecutionHistory
SET Status = $Status,
    CompletedUtc = $CompletedUtc,
    OutputJson = $OutputJson,
    ErrorMessage = $ErrorMessage,
    MetadataJson = $MetadataJson
WHERE ExecutionId = $ExecutionId;
""");

        UpsertSqlCatalog(
            connection,
            transaction,
            "ExecutionHistory_SelectRecent",
            "Lists recent execution history records for UI display.",
            "Execution History",
            """
SELECT ExecutionId,
       ExecutionKind,
       TargetId,
       TargetName,
       Status,
       StartedUtc,
       CompletedUtc,
       CorrelationId,
       InputJson,
       OutputJson,
       ErrorMessage,
       MetadataJson
FROM ExecutionHistory
ORDER BY StartedUtc DESC
LIMIT $Limit;
""");

        UpsertSqlCatalog(
            connection,
            transaction,
            "ExecutionStepHistory_Insert",
            "Inserts an execution step history record.",
            "Execution History",
            """
INSERT INTO ExecutionStepHistory (
    ExecutionStepId,
    ExecutionId,
    WorkflowStepId,
    CommandId,
    StepOrder,
    Status,
    StartedUtc,
    InputJson,
    OutputJson,
    ErrorMessage,
    MetadataJson
)
VALUES (
    $ExecutionStepId,
    $ExecutionId,
    $WorkflowStepId,
    $CommandId,
    $StepOrder,
    $Status,
    $StartedUtc,
    $InputJson,
    $OutputJson,
    $ErrorMessage,
    $MetadataJson
);
""");

        UpsertSqlCatalog(
            connection,
            transaction,
            "ExecutionStepHistory_Complete",
            "Completes one execution step history record.",
            "Execution History",
            """
UPDATE ExecutionStepHistory
SET Status = $Status,
    CompletedUtc = $CompletedUtc,
    OutputJson = $OutputJson,
    ErrorMessage = $ErrorMessage,
    MetadataJson = $MetadataJson
WHERE ExecutionStepId = $ExecutionStepId;
""");

        UpsertSqlCatalog(
            connection,
            transaction,
            "ExecutionStepHistory_SelectByExecution",
            "Lists execution step history records for an execution.",
            "Execution History",
            """
SELECT ExecutionStepId,
       ExecutionId,
       WorkflowStepId,
       CommandId,
       StepOrder,
       Status,
       StartedUtc,
       CompletedUtc,
       InputJson,
       OutputJson,
       ErrorMessage,
       MetadataJson
FROM ExecutionStepHistory
WHERE ExecutionId = $ExecutionId
ORDER BY StepOrder ASC;
""");

        // Context System Foundation
        UpsertSqlCatalog(
            connection,
            transaction,
            "ExecutionContext_SelectAll",
            "Lists execution contexts ordered by scope and name.",
            "Context System Foundation",
            """
SELECT ContextId,
       ContextName,
       Scope,
       Description,
       ContextJson,
       MetadataJson,
       IsEnabled,
       CreatedUtc,
       UpdatedUtc
FROM ExecutionContext
ORDER BY Scope ASC,
         ContextName ASC;
""");

        UpsertSqlCatalog(
            connection,
            transaction,
            "ExecutionContext_SelectByName",
            "Selects an execution context by context name.",
            "Context System Foundation",
            """
SELECT ContextId,
       ContextName,
       Scope,
       Description,
       ContextJson,
       MetadataJson,
       IsEnabled,
       CreatedUtc,
       UpdatedUtc
FROM ExecutionContext
WHERE ContextName = $ContextName;
""");

        UpsertSqlCatalog(
            connection,
            transaction,
            "ExecutionContext_InsertOrReplace",
            "Inserts or updates an execution context by context name.",
            "Context System Foundation",
            """
INSERT INTO ExecutionContext (
    ContextId,
    ContextName,
    Scope,
    Description,
    ContextJson,
    MetadataJson,
    IsEnabled
)
VALUES (
    $ContextId,
    $ContextName,
    $Scope,
    $Description,
    $ContextJson,
    $MetadataJson,
    $IsEnabled
)
ON CONFLICT(ContextName) DO UPDATE SET
    Scope = excluded.Scope,
    Description = excluded.Description,
    ContextJson = excluded.ContextJson,
    MetadataJson = excluded.MetadataJson,
    IsEnabled = excluded.IsEnabled,
    UpdatedUtc = CURRENT_TIMESTAMP;
""");

        UpsertSqlCatalog(
            connection,
            transaction,
            "EngineContext_SelectAll",
            "Compatibility alias for ExecutionContext_SelectAll.",
            "Context System Foundation",
            """
SELECT ContextId,
       ContextName,
       Scope,
       Description,
       ContextJson,
       MetadataJson,
       IsEnabled,
       CreatedUtc,
       UpdatedUtc
FROM ExecutionContext
ORDER BY Scope ASC,
         ContextName ASC;
""");

        UpsertSqlCatalog(
            connection,
            transaction,
            "EngineContext_SelectByName",
            "Compatibility alias for ExecutionContext_SelectByName.",
            "Context System Foundation",
            """
SELECT ContextId,
       ContextName,
       Scope,
       Description,
       ContextJson,
       MetadataJson,
       IsEnabled,
       CreatedUtc,
       UpdatedUtc
FROM ExecutionContext
WHERE ContextName = $ContextName;
""");

        UpsertSqlCatalog(
            connection,
            transaction,
            "EngineContext_InsertOrReplace",
            "Compatibility alias for ExecutionContext_InsertOrReplace.",
            "Context System Foundation",
            """
INSERT INTO ExecutionContext (
    ContextId,
    ContextName,
    Scope,
    Description,
    ContextJson,
    MetadataJson,
    IsEnabled
)
VALUES (
    $ContextId,
    $ContextName,
    $Scope,
    $Description,
    $ContextJson,
    $MetadataJson,
    $IsEnabled
)
ON CONFLICT(ContextName) DO UPDATE SET
    Scope = excluded.Scope,
    Description = excluded.Description,
    ContextJson = excluded.ContextJson,
    MetadataJson = excluded.MetadataJson,
    IsEnabled = excluded.IsEnabled,
    UpdatedUtc = CURRENT_TIMESTAMP;
""");

        // AI Provider Foundation
        UpsertSqlCatalog(
            connection,
            transaction,
            "AiProvider_SelectAll",
            "Lists AI providers ordered by provider kind and name.",
            "AI Provider Foundation",
            """
SELECT AiProviderId,
       ProviderName,
       DisplayName,
       ProviderKind,
       Description,
       ConfigurationJson,
       MetadataJson,
       IsEnabled,
       CreatedUtc,
       UpdatedUtc
FROM AiProvider
ORDER BY ProviderKind ASC,
         ProviderName ASC;
""");

        UpsertSqlCatalog(
            connection,
            transaction,
            "AiProvider_SelectByName",
            "Selects an AI provider by provider name.",
            "AI Provider Foundation",
            """
SELECT AiProviderId,
       ProviderName,
       DisplayName,
       ProviderKind,
       Description,
       ConfigurationJson,
       MetadataJson,
       IsEnabled,
       CreatedUtc,
       UpdatedUtc
FROM AiProvider
WHERE ProviderName = $ProviderName;
""");

        UpsertSqlCatalog(
            connection,
            transaction,
            "AiProvider_InsertOrReplace",
            "Inserts or updates an AI provider by provider name.",
            "AI Provider Foundation",
            """
INSERT INTO AiProvider (
    AiProviderId,
    ProviderName,
    DisplayName,
    ProviderKind,
    Description,
    ConfigurationJson,
    MetadataJson,
    IsEnabled
)
VALUES (
    $AiProviderId,
    $ProviderName,
    $DisplayName,
    $ProviderKind,
    $Description,
    $ConfigurationJson,
    $MetadataJson,
    $IsEnabled
)
ON CONFLICT(ProviderName) DO UPDATE SET
    DisplayName = excluded.DisplayName,
    ProviderKind = excluded.ProviderKind,
    Description = excluded.Description,
    ConfigurationJson = excluded.ConfigurationJson,
    MetadataJson = excluded.MetadataJson,
    IsEnabled = excluded.IsEnabled,
    UpdatedUtc = CURRENT_TIMESTAMP;
""");

        UpsertSqlCatalog(
            connection,
            transaction,
            "AiProviderCapability_InsertOrReplace",
            "Inserts or updates an AI provider capability.",
            "AI Provider Foundation",
            """
INSERT INTO AiProviderCapability (
    AiProviderCapabilityId,
    AiProviderId,
    CapabilityName,
    Description,
    MetadataJson,
    IsEnabled
)
VALUES (
    $AiProviderCapabilityId,
    $AiProviderId,
    $CapabilityName,
    $Description,
    $MetadataJson,
    $IsEnabled
)
ON CONFLICT(AiProviderId, CapabilityName) DO UPDATE SET
    Description = excluded.Description,
    MetadataJson = excluded.MetadataJson,
    IsEnabled = excluded.IsEnabled,
    UpdatedUtc = CURRENT_TIMESTAMP;
""");

        UpsertSqlCatalog(
            connection,
            transaction,
            "AiProviderCapability_SelectByProvider",
            "Lists AI provider capabilities for one provider.",
            "AI Provider Foundation",
            """
SELECT AiProviderCapabilityId,
       AiProviderId,
       CapabilityName,
       Description,
       MetadataJson,
       IsEnabled,
       CreatedUtc,
       UpdatedUtc
FROM AiProviderCapability
WHERE AiProviderId = $AiProviderId
ORDER BY CapabilityName ASC;
""");
    }


    private static void UpsertSqlCatalog(
        SqliteConnection connection,
        SqliteTransaction transaction,
        string queryName,
        string description,
        string category,
        string sqlText)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
INSERT INTO SqlQuery (
    QueryName,
    Description,
    SqlText,
    Category,
    IsActive
)
VALUES (
    $QueryName,
    $Description,
    $SqlText,
    $Category,
    1
)
ON CONFLICT(QueryName) DO UPDATE SET
    Description = excluded.Description,
    SqlText = excluded.SqlText,
    Category = excluded.Category,
    IsActive = excluded.IsActive,
    UpdatedUtc = CURRENT_TIMESTAMP;
""";
        command.Parameters.AddWithValue("$QueryName", queryName);
        command.Parameters.AddWithValue("$Description", description);
        command.Parameters.AddWithValue("$SqlText", sqlText);
        command.Parameters.AddWithValue("$Category", category);
        command.ExecuteNonQuery();
    }

    private static void SeedMigration(SqliteConnection connection, SqliteTransaction transaction, string migrationId, string description)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
INSERT OR REPLACE INTO MigrationHistory (
    MigrationId,
    Description
)
VALUES (
    $MigrationId,
    $Description
);
""";
        command.Parameters.AddWithValue("$MigrationId", migrationId);
        command.Parameters.AddWithValue("$Description", description);
        command.ExecuteNonQuery();
    }

    private static void SeedSchemaSnapshot(
        SqliteConnection connection,
        SqliteTransaction transaction,
        string migrationId,
        int schemaVersion,
        string snapshotSql,
        string description)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
INSERT OR REPLACE INTO DatabaseSchemaSnapshot (
    MigrationId,
    SchemaVersion,
    SnapshotSql,
    SnapshotHash,
    Description
)
VALUES (
    $MigrationId,
    $SchemaVersion,
    $SnapshotSql,
    $SnapshotHash,
    $Description
);
""";
        command.Parameters.AddWithValue("$MigrationId", migrationId);
        command.Parameters.AddWithValue("$SchemaVersion", schemaVersion);
        command.Parameters.AddWithValue("$SnapshotSql", snapshotSql);
        command.Parameters.AddWithValue("$SnapshotHash", ComputeSha256Hash(snapshotSql));
        command.Parameters.AddWithValue("$Description", description);
        command.ExecuteNonQuery();
    }

    private static void EnsureColumn(
        SqliteConnection connection,
        SqliteTransaction transaction,
        string tableName,
        string columnName,
        string columnDefinition)
    {
        if (ColumnExists(connection, transaction, tableName, columnName))
        {
            return;
        }

        ExecuteNonQuery(connection, transaction, $"ALTER TABLE {tableName} ADD COLUMN {columnName} {columnDefinition};");
    }

    private static bool ColumnExists(
        SqliteConnection connection,
        SqliteTransaction transaction,
        string tableName,
        string columnName)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = $"PRAGMA table_info({tableName});";

        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            if (string.Equals(reader.GetString(1), columnName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static string ComputeSha256Hash(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        byte[] bytes = Encoding.UTF8.GetBytes(text);
        byte[] hash = SHA256.HashData(bytes);

        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static void ExecuteNonQuery(SqliteConnection connection, SqliteTransaction transaction, string sql)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }
}
