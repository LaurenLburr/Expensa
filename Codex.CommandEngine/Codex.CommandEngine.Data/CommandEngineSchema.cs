using Microsoft.Data.Sqlite;

namespace Codex.CommandEngine.Data;

public static class CommandEngineSchema
{
    public const int CurrentSchemaVersion = 7;

    public const string BaselineMigrationId = "0001_engine_database_foundation";
    public const string BaselineMigrationDescription = "Initial command engine database foundation.";

    public const string SqlCatalogMigrationId = "0002_sql_catalog_foundation";
    public const string SqlCatalogMigrationDescription = "Adds SQL catalog update tracking and catalog service support.";

    public const string CommandRegistrationMigrationId = "0003_command_registration_foundation";
    public const string CommandRegistrationMigrationDescription = "Adds command registration metadata and command parameter definitions.";

    public const string WorkflowFoundationMigrationId = "0004_workflow_model_foundation";
    public const string WorkflowFoundationMigrationDescription = "Adds workflow model metadata, workflow step metadata, and workflow SQL catalog entries.";

    public const string ExecutionHistoryMigrationId = "0005_execution_history_foundation";
    public const string ExecutionHistoryMigrationDescription = "Adds execution history repository support, correlation tracking, and execution SQL catalog entries.";

    public const string ContextSystemMigrationId = "0006_context_system_foundation";
    public const string ContextSystemMigrationDescription = "Adds execution context repository support and context SQL catalog entries.";

    public const string AiProviderFoundationMigrationId = "0007_ai_provider_foundation";
    public const string AiProviderFoundationMigrationDescription = "Adds AI provider repository support, provider capabilities, and AI provider SQL catalog entries.";

    public const string SchemaSql = """
CREATE TABLE IF NOT EXISTS MigrationHistory (
    MigrationId TEXT PRIMARY KEY,
    AppliedUtc  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description TEXT NOT NULL DEFAULT ''
);

CREATE TABLE IF NOT EXISTS DatabaseSchemaSnapshot (
    MigrationId   TEXT PRIMARY KEY,
    SchemaVersion INTEGER NOT NULL,
    SnapshotSql   TEXT NOT NULL,
    SnapshotHash  TEXT NOT NULL,
    CreatedUtc    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description   TEXT NOT NULL DEFAULT '',
    FOREIGN KEY (MigrationId) REFERENCES MigrationHistory(MigrationId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS SqlQuery (
    QueryName   TEXT PRIMARY KEY,
    Description TEXT NOT NULL DEFAULT '',
    SqlText     TEXT NOT NULL DEFAULT '',
    CreatedUtc  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Category    TEXT NOT NULL DEFAULT '',
    IsActive    INTEGER NOT NULL DEFAULT 1,
    UpdatedUtc  TEXT NULL
);

CREATE TABLE IF NOT EXISTS Log (
    LogId     INTEGER PRIMARY KEY AUTOINCREMENT,
    Timestamp TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Action    TEXT NOT NULL,
    Details   TEXT NULL
);

CREATE TABLE IF NOT EXISTS CommandDefinition (
    CommandId    TEXT PRIMARY KEY,
    CommandName  TEXT NOT NULL UNIQUE,
    DisplayName  TEXT NOT NULL DEFAULT '',
    Description  TEXT NOT NULL DEFAULT '',
    Category     TEXT NOT NULL DEFAULT '',
    Version      INTEGER NOT NULL DEFAULT 1,
    HandlerKey   TEXT NOT NULL DEFAULT '',
    HandlerType  TEXT NOT NULL DEFAULT '',
    InputJson    TEXT NOT NULL DEFAULT '{}',
    OutputJson   TEXT NOT NULL DEFAULT '{}',
    MetadataJson TEXT NOT NULL DEFAULT '{}',
    IsEnabled    INTEGER NOT NULL DEFAULT 1,
    CreatedUtc   TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc   TEXT NULL
);

CREATE TABLE IF NOT EXISTS CommandParameterDefinition (
    CommandParameterDefinitionId TEXT PRIMARY KEY,
    CommandId                    TEXT NOT NULL,
    ParameterName                TEXT NOT NULL,
    ParameterType                TEXT NOT NULL,
    IsRequired                   INTEGER NOT NULL DEFAULT 0,
    DefaultValue                 TEXT NOT NULL DEFAULT '',
    Description                  TEXT NOT NULL DEFAULT '',
    SortOrder                    INTEGER NOT NULL DEFAULT 0,
    CreatedUtc                   TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc                   TEXT NULL,
    FOREIGN KEY (CommandId) REFERENCES CommandDefinition(CommandId) ON DELETE CASCADE,
    UNIQUE (CommandId, ParameterName)
);

CREATE TABLE IF NOT EXISTS WorkflowDefinition (
    WorkflowId    TEXT PRIMARY KEY,
    WorkflowName  TEXT NOT NULL,
    DisplayName   TEXT NOT NULL DEFAULT '',
    Description   TEXT NOT NULL DEFAULT '',
    Version       INTEGER NOT NULL DEFAULT 1,
    MetadataJson  TEXT NOT NULL DEFAULT '{}',
    IsEnabled     INTEGER NOT NULL DEFAULT 1,
    CreatedUtc    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc    TEXT NULL,
    UNIQUE (WorkflowName, Version)
);

CREATE TABLE IF NOT EXISTS WorkflowStep (
    WorkflowStepId TEXT PRIMARY KEY,
    WorkflowId     TEXT NOT NULL,
    StepOrder      INTEGER NOT NULL,
    StepName       TEXT NOT NULL DEFAULT '',
    CommandId      TEXT NOT NULL,
    InputMapJson   TEXT NOT NULL DEFAULT '{}',
    MetadataJson   TEXT NOT NULL DEFAULT '{}',
    IsEnabled      INTEGER NOT NULL DEFAULT 1,
    FOREIGN KEY (WorkflowId) REFERENCES WorkflowDefinition(WorkflowId) ON DELETE CASCADE,
    FOREIGN KEY (CommandId) REFERENCES CommandDefinition(CommandId) ON DELETE RESTRICT,
    UNIQUE (WorkflowId, StepOrder)
);

CREATE TABLE IF NOT EXISTS ExecutionHistory (
    ExecutionId   TEXT PRIMARY KEY,
    ExecutionKind TEXT NOT NULL,
    TargetId      TEXT NOT NULL,
    TargetName    TEXT NOT NULL DEFAULT '',
    Status        TEXT NOT NULL,
    StartedUtc    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CompletedUtc  TEXT NULL,
    CorrelationId TEXT NOT NULL DEFAULT '',
    InputJson     TEXT NOT NULL DEFAULT '{}',
    OutputJson    TEXT NOT NULL DEFAULT '{}',
    ErrorMessage  TEXT NOT NULL DEFAULT '',
    MetadataJson  TEXT NOT NULL DEFAULT '{}'
);

CREATE TABLE IF NOT EXISTS ExecutionStepHistory (
    ExecutionStepId TEXT PRIMARY KEY,
    ExecutionId     TEXT NOT NULL,
    WorkflowStepId  TEXT NULL,
    CommandId       TEXT NULL,
    StepOrder       INTEGER NOT NULL,
    Status          TEXT NOT NULL,
    StartedUtc      TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CompletedUtc    TEXT NULL,
    InputJson       TEXT NOT NULL DEFAULT '{}',
    OutputJson      TEXT NOT NULL DEFAULT '{}',
    ErrorMessage    TEXT NOT NULL DEFAULT '',
    MetadataJson    TEXT NOT NULL DEFAULT '{}',
    FOREIGN KEY (ExecutionId) REFERENCES ExecutionHistory(ExecutionId) ON DELETE CASCADE,
    FOREIGN KEY (WorkflowStepId) REFERENCES WorkflowStep(WorkflowStepId) ON DELETE SET NULL,
    FOREIGN KEY (CommandId) REFERENCES CommandDefinition(CommandId) ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS ExecutionContext (
    ContextId    TEXT PRIMARY KEY,
    ContextName  TEXT NOT NULL UNIQUE,
    Scope        TEXT NOT NULL DEFAULT '',
    Description  TEXT NOT NULL DEFAULT '',
    ContextJson  TEXT NOT NULL DEFAULT '{}',
    MetadataJson TEXT NOT NULL DEFAULT '{}',
    IsEnabled    INTEGER NOT NULL DEFAULT 1,
    CreatedUtc   TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc   TEXT NULL
);

CREATE TABLE IF NOT EXISTS AiProvider (
    AiProviderId      TEXT PRIMARY KEY,
    ProviderName      TEXT NOT NULL UNIQUE,
    DisplayName       TEXT NOT NULL DEFAULT '',
    ProviderKind      TEXT NOT NULL DEFAULT '',
    Description       TEXT NOT NULL DEFAULT '',
    ConfigurationJson TEXT NOT NULL DEFAULT '{}',
    MetadataJson      TEXT NOT NULL DEFAULT '{}',
    IsEnabled         INTEGER NOT NULL DEFAULT 1,
    CreatedUtc        TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc        TEXT NULL
);

CREATE TABLE IF NOT EXISTS AiProviderCapability (
    AiProviderCapabilityId TEXT PRIMARY KEY,
    AiProviderId           TEXT NOT NULL,
    CapabilityName         TEXT NOT NULL,
    Description            TEXT NOT NULL DEFAULT '',
    MetadataJson           TEXT NOT NULL DEFAULT '{}',
    IsEnabled              INTEGER NOT NULL DEFAULT 1,
    CreatedUtc             TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc             TEXT NULL,
    FOREIGN KEY (AiProviderId) REFERENCES AiProvider(AiProviderId) ON DELETE CASCADE,
    UNIQUE (AiProviderId, CapabilityName)
);
""";

    public const string IndexSql = """
CREATE INDEX IF NOT EXISTS IX_ExecutionHistory_CorrelationId
ON ExecutionHistory(CorrelationId);

CREATE INDEX IF NOT EXISTS IX_ExecutionHistory_StartedUtc
ON ExecutionHistory(StartedUtc);

CREATE INDEX IF NOT EXISTS IX_ExecutionStepHistory_ExecutionId
ON ExecutionStepHistory(ExecutionId);
""";

    public static void EnsureCreated(string databasePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);

        CommandEngineDatabaseOptions options = CommandEngineDatabaseOptions.ForFile(databasePath);
        CommandEngineConnectionFactory connectionFactory = new(options);
        CommandEngineDatabaseInitializer initializer = new(connectionFactory);

        initializer.EnsureCreated();
    }

    public static void EnsureCreated(SqliteConnection connection)
    {
        CommandEngineDatabaseInitializer.EnsureCreated(connection);
    }
}
