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
            SeedBaselineMigration(connection, transaction);
            SeedBaselineSchemaSnapshot(connection, transaction);
            ApplySqlCatalogMigration(connection, transaction);
            SeedSqlCatalogSchemaSnapshot(connection, transaction);
            ApplyCommandRegistrationMigration(connection, transaction);
            SeedCommandRegistrationSchemaSnapshot(connection, transaction);
            SeedCommandRegistrationSqlCatalog(connection, transaction);
             ApplyWorkflowFoundationMigration(connection, transaction);
            SeedWorkflowFoundationSchemaSnapshot(connection, transaction);
            SeedWorkflowFoundationSqlCatalog(connection, transaction);
            ApplyExecutionHistoryMigration(connection, transaction);
            SeedExecutionHistorySchemaSnapshot(connection, transaction);
            SeedExecutionHistorySqlCatalog(connection, transaction);
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private static void ApplySqlCatalogMigration(SqliteConnection connection, SqliteTransaction transaction)
    {
        if (!ColumnExists(connection, transaction, "SqlQuery", "UpdatedUtc"))
        {
            ExecuteNonQuery(connection, transaction, CommandEngineSchema.SqlCatalogMigrationSql);
        }

        SeedMigration(
            connection,
            transaction,
            CommandEngineSchema.SqlCatalogMigrationId,
            CommandEngineSchema.SqlCatalogMigrationDescription);
    }

    private static void SeedSqlCatalogSchemaSnapshot(SqliteConnection connection, SqliteTransaction transaction)
    {
        SeedSchemaSnapshot(
            connection,
            transaction,
            CommandEngineSchema.SqlCatalogMigrationId,
            2,
            ReadEmbeddedSnapshotOrFallback("0002_sql_catalog_foundation.sql", CommandEngineSchema.SchemaSql),
            CommandEngineSchema.SqlCatalogMigrationDescription);
    }

    private static void ApplyCommandRegistrationMigration(SqliteConnection connection, SqliteTransaction transaction)
    {
        if (!ColumnExists(connection, transaction, "CommandDefinition", "DisplayName"))
        {
            ExecuteNonQuery(connection, transaction, "ALTER TABLE CommandDefinition ADD COLUMN DisplayName TEXT NOT NULL DEFAULT ''; ");
        }

        if (!ColumnExists(connection, transaction, "CommandDefinition", "HandlerType"))
        {
            ExecuteNonQuery(connection, transaction, "ALTER TABLE CommandDefinition ADD COLUMN HandlerType TEXT NOT NULL DEFAULT ''; ");
        }

        ExecuteNonQuery(connection, transaction, CommandEngineSchema.CommandRegistrationMigrationSql);

        SeedMigration(
            connection,
            transaction,
            CommandEngineSchema.CommandRegistrationMigrationId,
            CommandEngineSchema.CommandRegistrationMigrationDescription);
    }

    private static void SeedCommandRegistrationSchemaSnapshot(SqliteConnection connection, SqliteTransaction transaction)
    {
        SeedSchemaSnapshot(
            connection,
            transaction,
            CommandEngineSchema.CommandRegistrationMigrationId,
            3,
            ReadEmbeddedSnapshotOrFallback("0003_command_registration_foundation.sql", CommandEngineSchema.SchemaSql),
            CommandEngineSchema.CommandRegistrationMigrationDescription);
    }

    private static void SeedCommandRegistrationSqlCatalog(SqliteConnection connection, SqliteTransaction transaction)
    {
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
    }


    private static void ApplyWorkflowFoundationMigration(SqliteConnection connection, SqliteTransaction transaction)
    {
        if (!ColumnExists(connection, transaction, "WorkflowDefinition", "DisplayName"))
        {
            ExecuteNonQuery(connection, transaction, "ALTER TABLE WorkflowDefinition ADD COLUMN DisplayName TEXT NOT NULL DEFAULT ''; ");
        }

        if (!ColumnExists(connection, transaction, "WorkflowStep", "StepName"))
        {
            ExecuteNonQuery(connection, transaction, "ALTER TABLE WorkflowStep ADD COLUMN StepName TEXT NOT NULL DEFAULT ''; ");
        }

        SeedMigration(
            connection,
            transaction,
            CommandEngineSchema.WorkflowFoundationMigrationId,
            CommandEngineSchema.WorkflowFoundationMigrationDescription);
    }

    private static void SeedWorkflowFoundationSchemaSnapshot(SqliteConnection connection, SqliteTransaction transaction)
    {
        SeedSchemaSnapshot(
            connection,
            transaction,
            CommandEngineSchema.WorkflowFoundationMigrationId,
            CommandEngineSchema.CurrentSchemaVersion,
            CommandEngineSchema.SchemaSql,
            CommandEngineSchema.WorkflowFoundationMigrationDescription);
    }

    private static void SeedWorkflowFoundationSqlCatalog(SqliteConnection connection, SqliteTransaction transaction)
    {
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
    }


    private static void ApplyExecutionHistoryMigration(SqliteConnection connection, SqliteTransaction transaction)
    {
        if (!ColumnExists(connection, transaction, "ExecutionHistory", "CorrelationId"))
        {
            ExecuteNonQuery(connection, transaction, "ALTER TABLE ExecutionHistory ADD COLUMN CorrelationId TEXT NOT NULL DEFAULT ''; ");
        }

        ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS IX_ExecutionHistory_Status_StartedUtc ON ExecutionHistory (Status, StartedUtc); ");
        ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS IX_ExecutionHistory_CorrelationId ON ExecutionHistory (CorrelationId); ");
        ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS IX_ExecutionStepHistory_ExecutionId_StepOrder ON ExecutionStepHistory (ExecutionId, StepOrder); ");

        SeedMigration(
            connection,
            transaction,
            CommandEngineSchema.ExecutionHistoryMigrationId,
            CommandEngineSchema.ExecutionHistoryMigrationDescription);
    }

    private static void SeedExecutionHistorySchemaSnapshot(SqliteConnection connection, SqliteTransaction transaction)
    {
        SeedSchemaSnapshot(
            connection,
            transaction,
            CommandEngineSchema.ExecutionHistoryMigrationId,
            CommandEngineSchema.CurrentSchemaVersion,
            CommandEngineSchema.SchemaSql,
            CommandEngineSchema.ExecutionHistoryMigrationDescription);
    }

    private static void SeedExecutionHistorySqlCatalog(SqliteConnection connection, SqliteTransaction transaction)
    {
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

    private static void SeedBaselineMigration(SqliteConnection connection, SqliteTransaction transaction)
    {
        SeedMigration(
            connection,
            transaction,
            CommandEngineSchema.BaselineMigrationId,
            CommandEngineSchema.BaselineMigrationDescription);
    }

    private static void SeedBaselineSchemaSnapshot(SqliteConnection connection, SqliteTransaction transaction)
    {
        SeedSchemaSnapshot(
            connection,
            transaction,
            CommandEngineSchema.BaselineMigrationId,
            1,
            CommandEngineSchema.BaselineSchemaSql,
            CommandEngineSchema.BaselineMigrationDescription);
    }

    private static void SeedMigration(
        SqliteConnection connection,
        SqliteTransaction transaction,
        string migrationId,
        string description)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
INSERT OR IGNORE INTO MigrationHistory (
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
        string snapshotHash = ComputeSha256Hash(snapshotSql);

        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
INSERT OR IGNORE INTO DatabaseSchemaSnapshot (
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
        command.Parameters.AddWithValue("$SnapshotHash", snapshotHash);
        command.Parameters.AddWithValue("$Description", description);
        command.ExecuteNonQuery();
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
            string currentColumnName = reader.GetString(1);

            if (string.Equals(currentColumnName, columnName, StringComparison.OrdinalIgnoreCase))
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

    private static string ReadEmbeddedSnapshotOrFallback(string fileName, string fallback)
    {
        string? directory = AppContext.BaseDirectory;

        for (int i = 0; i < 8 && directory is not null; i++)
        {
            string candidate = Path.Combine(directory, "Docs", "Database", "SchemaSnapshots", fileName);

            if (File.Exists(candidate))
            {
                return File.ReadAllText(candidate);
            }

            directory = Directory.GetParent(directory)?.FullName;
        }

        return fallback;
    }

    private static void ExecuteNonQuery(SqliteConnection connection, SqliteTransaction transaction, string sql)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }
}
