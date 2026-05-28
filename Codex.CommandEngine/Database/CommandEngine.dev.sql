BEGIN TRANSACTION;
CREATE TABLE AiProvider (
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
CREATE TABLE AiProviderCapability (
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
CREATE TABLE CommandDefinition (
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
CREATE TABLE CommandParameterDefinition (
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
CREATE TABLE DatabaseSchemaSnapshot (
    MigrationId   TEXT PRIMARY KEY,
    SchemaVersion INTEGER NOT NULL,
    SnapshotSql   TEXT NOT NULL,
    SnapshotHash  TEXT NOT NULL,
    CreatedUtc    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description   TEXT NOT NULL DEFAULT '',
    FOREIGN KEY (MigrationId) REFERENCES MigrationHistory(MigrationId) ON DELETE CASCADE
);
INSERT INTO "DatabaseSchemaSnapshot" VALUES('0001_engine_database_foundation',1,'CREATE TABLE IF NOT EXISTS MigrationHistory (
    MigrationId TEXT PRIMARY KEY,
    AppliedUtc  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description TEXT NOT NULL DEFAULT ''''
);

CREATE TABLE IF NOT EXISTS DatabaseSchemaSnapshot (
    MigrationId   TEXT PRIMARY KEY,
    SchemaVersion INTEGER NOT NULL,
    SnapshotSql   TEXT NOT NULL,
    SnapshotHash  TEXT NOT NULL,
    CreatedUtc    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description   TEXT NOT NULL DEFAULT '''',
    FOREIGN KEY (MigrationId) REFERENCES MigrationHistory(MigrationId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS SqlQuery (
    QueryName   TEXT PRIMARY KEY,
    Description TEXT NOT NULL DEFAULT '''',
    SqlText     TEXT NOT NULL DEFAULT '''',
    CreatedUtc  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Category    TEXT NOT NULL DEFAULT '''',
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
    DisplayName  TEXT NOT NULL DEFAULT '''',
    Description  TEXT NOT NULL DEFAULT '''',
    Category     TEXT NOT NULL DEFAULT '''',
    Version      INTEGER NOT NULL DEFAULT 1,
    HandlerKey   TEXT NOT NULL DEFAULT '''',
    HandlerType  TEXT NOT NULL DEFAULT '''',
    InputJson    TEXT NOT NULL DEFAULT ''{}'',
    OutputJson   TEXT NOT NULL DEFAULT ''{}'',
    MetadataJson TEXT NOT NULL DEFAULT ''{}'',
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
    DefaultValue                 TEXT NOT NULL DEFAULT '''',
    Description                  TEXT NOT NULL DEFAULT '''',
    SortOrder                    INTEGER NOT NULL DEFAULT 0,
    CreatedUtc                   TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc                   TEXT NULL,
    FOREIGN KEY (CommandId) REFERENCES CommandDefinition(CommandId) ON DELETE CASCADE,
    UNIQUE (CommandId, ParameterName)
);

CREATE TABLE IF NOT EXISTS WorkflowDefinition (
    WorkflowId    TEXT PRIMARY KEY,
    WorkflowName  TEXT NOT NULL,
    DisplayName   TEXT NOT NULL DEFAULT '''',
    Description   TEXT NOT NULL DEFAULT '''',
    Version       INTEGER NOT NULL DEFAULT 1,
    MetadataJson  TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled     INTEGER NOT NULL DEFAULT 1,
    CreatedUtc    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc    TEXT NULL,
    UNIQUE (WorkflowName, Version)
);

CREATE TABLE IF NOT EXISTS WorkflowStep (
    WorkflowStepId TEXT PRIMARY KEY,
    WorkflowId     TEXT NOT NULL,
    StepOrder      INTEGER NOT NULL,
    StepName       TEXT NOT NULL DEFAULT '''',
    CommandId      TEXT NOT NULL,
    InputMapJson   TEXT NOT NULL DEFAULT ''{}'',
    MetadataJson   TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled      INTEGER NOT NULL DEFAULT 1,
    FOREIGN KEY (WorkflowId) REFERENCES WorkflowDefinition(WorkflowId) ON DELETE CASCADE,
    FOREIGN KEY (CommandId) REFERENCES CommandDefinition(CommandId) ON DELETE RESTRICT,
    UNIQUE (WorkflowId, StepOrder)
);

CREATE TABLE IF NOT EXISTS ExecutionHistory (
    ExecutionId   TEXT PRIMARY KEY,
    ExecutionKind TEXT NOT NULL,
    TargetId      TEXT NOT NULL,
    TargetName    TEXT NOT NULL DEFAULT '''',
    Status        TEXT NOT NULL,
    StartedUtc    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CompletedUtc  TEXT NULL,
    CorrelationId TEXT NOT NULL DEFAULT '''',
    InputJson     TEXT NOT NULL DEFAULT ''{}'',
    OutputJson    TEXT NOT NULL DEFAULT ''{}'',
    ErrorMessage  TEXT NOT NULL DEFAULT '''',
    MetadataJson  TEXT NOT NULL DEFAULT ''{}''
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
    InputJson       TEXT NOT NULL DEFAULT ''{}'',
    OutputJson      TEXT NOT NULL DEFAULT ''{}'',
    ErrorMessage    TEXT NOT NULL DEFAULT '''',
    MetadataJson    TEXT NOT NULL DEFAULT ''{}'',
    FOREIGN KEY (ExecutionId) REFERENCES ExecutionHistory(ExecutionId) ON DELETE CASCADE,
    FOREIGN KEY (WorkflowStepId) REFERENCES WorkflowStep(WorkflowStepId) ON DELETE SET NULL,
    FOREIGN KEY (CommandId) REFERENCES CommandDefinition(CommandId) ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS ExecutionContext (
    ContextId    TEXT PRIMARY KEY,
    ContextName  TEXT NOT NULL UNIQUE,
    Scope        TEXT NOT NULL DEFAULT '''',
    Description  TEXT NOT NULL DEFAULT '''',
    ContextJson  TEXT NOT NULL DEFAULT ''{}'',
    MetadataJson TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled    INTEGER NOT NULL DEFAULT 1,
    CreatedUtc   TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc   TEXT NULL
);

CREATE TABLE IF NOT EXISTS AiProvider (
    AiProviderId      TEXT PRIMARY KEY,
    ProviderName      TEXT NOT NULL UNIQUE,
    DisplayName       TEXT NOT NULL DEFAULT '''',
    ProviderKind      TEXT NOT NULL DEFAULT '''',
    Description       TEXT NOT NULL DEFAULT '''',
    ConfigurationJson TEXT NOT NULL DEFAULT ''{}'',
    MetadataJson      TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled         INTEGER NOT NULL DEFAULT 1,
    CreatedUtc        TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc        TEXT NULL
);

CREATE TABLE IF NOT EXISTS AiProviderCapability (
    AiProviderCapabilityId TEXT PRIMARY KEY,
    AiProviderId           TEXT NOT NULL,
    CapabilityName         TEXT NOT NULL,
    Description            TEXT NOT NULL DEFAULT '''',
    MetadataJson           TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled              INTEGER NOT NULL DEFAULT 1,
    CreatedUtc             TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc             TEXT NULL,
    FOREIGN KEY (AiProviderId) REFERENCES AiProvider(AiProviderId) ON DELETE CASCADE,
    UNIQUE (AiProviderId, CapabilityName)
);','dev-snapshot','2026-05-17 16:28:57','Initial command engine database foundation.');
INSERT INTO "DatabaseSchemaSnapshot" VALUES('0002_sql_catalog_foundation',2,'CREATE TABLE IF NOT EXISTS MigrationHistory (
    MigrationId TEXT PRIMARY KEY,
    AppliedUtc  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description TEXT NOT NULL DEFAULT ''''
);

CREATE TABLE IF NOT EXISTS DatabaseSchemaSnapshot (
    MigrationId   TEXT PRIMARY KEY,
    SchemaVersion INTEGER NOT NULL,
    SnapshotSql   TEXT NOT NULL,
    SnapshotHash  TEXT NOT NULL,
    CreatedUtc    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description   TEXT NOT NULL DEFAULT '''',
    FOREIGN KEY (MigrationId) REFERENCES MigrationHistory(MigrationId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS SqlQuery (
    QueryName   TEXT PRIMARY KEY,
    Description TEXT NOT NULL DEFAULT '''',
    SqlText     TEXT NOT NULL DEFAULT '''',
    CreatedUtc  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Category    TEXT NOT NULL DEFAULT '''',
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
    DisplayName  TEXT NOT NULL DEFAULT '''',
    Description  TEXT NOT NULL DEFAULT '''',
    Category     TEXT NOT NULL DEFAULT '''',
    Version      INTEGER NOT NULL DEFAULT 1,
    HandlerKey   TEXT NOT NULL DEFAULT '''',
    HandlerType  TEXT NOT NULL DEFAULT '''',
    InputJson    TEXT NOT NULL DEFAULT ''{}'',
    OutputJson   TEXT NOT NULL DEFAULT ''{}'',
    MetadataJson TEXT NOT NULL DEFAULT ''{}'',
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
    DefaultValue                 TEXT NOT NULL DEFAULT '''',
    Description                  TEXT NOT NULL DEFAULT '''',
    SortOrder                    INTEGER NOT NULL DEFAULT 0,
    CreatedUtc                   TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc                   TEXT NULL,
    FOREIGN KEY (CommandId) REFERENCES CommandDefinition(CommandId) ON DELETE CASCADE,
    UNIQUE (CommandId, ParameterName)
);

CREATE TABLE IF NOT EXISTS WorkflowDefinition (
    WorkflowId    TEXT PRIMARY KEY,
    WorkflowName  TEXT NOT NULL,
    DisplayName   TEXT NOT NULL DEFAULT '''',
    Description   TEXT NOT NULL DEFAULT '''',
    Version       INTEGER NOT NULL DEFAULT 1,
    MetadataJson  TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled     INTEGER NOT NULL DEFAULT 1,
    CreatedUtc    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc    TEXT NULL,
    UNIQUE (WorkflowName, Version)
);

CREATE TABLE IF NOT EXISTS WorkflowStep (
    WorkflowStepId TEXT PRIMARY KEY,
    WorkflowId     TEXT NOT NULL,
    StepOrder      INTEGER NOT NULL,
    StepName       TEXT NOT NULL DEFAULT '''',
    CommandId      TEXT NOT NULL,
    InputMapJson   TEXT NOT NULL DEFAULT ''{}'',
    MetadataJson   TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled      INTEGER NOT NULL DEFAULT 1,
    FOREIGN KEY (WorkflowId) REFERENCES WorkflowDefinition(WorkflowId) ON DELETE CASCADE,
    FOREIGN KEY (CommandId) REFERENCES CommandDefinition(CommandId) ON DELETE RESTRICT,
    UNIQUE (WorkflowId, StepOrder)
);

CREATE TABLE IF NOT EXISTS ExecutionHistory (
    ExecutionId   TEXT PRIMARY KEY,
    ExecutionKind TEXT NOT NULL,
    TargetId      TEXT NOT NULL,
    TargetName    TEXT NOT NULL DEFAULT '''',
    Status        TEXT NOT NULL,
    StartedUtc    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CompletedUtc  TEXT NULL,
    CorrelationId TEXT NOT NULL DEFAULT '''',
    InputJson     TEXT NOT NULL DEFAULT ''{}'',
    OutputJson    TEXT NOT NULL DEFAULT ''{}'',
    ErrorMessage  TEXT NOT NULL DEFAULT '''',
    MetadataJson  TEXT NOT NULL DEFAULT ''{}''
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
    InputJson       TEXT NOT NULL DEFAULT ''{}'',
    OutputJson      TEXT NOT NULL DEFAULT ''{}'',
    ErrorMessage    TEXT NOT NULL DEFAULT '''',
    MetadataJson    TEXT NOT NULL DEFAULT ''{}'',
    FOREIGN KEY (ExecutionId) REFERENCES ExecutionHistory(ExecutionId) ON DELETE CASCADE,
    FOREIGN KEY (WorkflowStepId) REFERENCES WorkflowStep(WorkflowStepId) ON DELETE SET NULL,
    FOREIGN KEY (CommandId) REFERENCES CommandDefinition(CommandId) ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS ExecutionContext (
    ContextId    TEXT PRIMARY KEY,
    ContextName  TEXT NOT NULL UNIQUE,
    Scope        TEXT NOT NULL DEFAULT '''',
    Description  TEXT NOT NULL DEFAULT '''',
    ContextJson  TEXT NOT NULL DEFAULT ''{}'',
    MetadataJson TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled    INTEGER NOT NULL DEFAULT 1,
    CreatedUtc   TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc   TEXT NULL
);

CREATE TABLE IF NOT EXISTS AiProvider (
    AiProviderId      TEXT PRIMARY KEY,
    ProviderName      TEXT NOT NULL UNIQUE,
    DisplayName       TEXT NOT NULL DEFAULT '''',
    ProviderKind      TEXT NOT NULL DEFAULT '''',
    Description       TEXT NOT NULL DEFAULT '''',
    ConfigurationJson TEXT NOT NULL DEFAULT ''{}'',
    MetadataJson      TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled         INTEGER NOT NULL DEFAULT 1,
    CreatedUtc        TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc        TEXT NULL
);

CREATE TABLE IF NOT EXISTS AiProviderCapability (
    AiProviderCapabilityId TEXT PRIMARY KEY,
    AiProviderId           TEXT NOT NULL,
    CapabilityName         TEXT NOT NULL,
    Description            TEXT NOT NULL DEFAULT '''',
    MetadataJson           TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled              INTEGER NOT NULL DEFAULT 1,
    CreatedUtc             TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc             TEXT NULL,
    FOREIGN KEY (AiProviderId) REFERENCES AiProvider(AiProviderId) ON DELETE CASCADE,
    UNIQUE (AiProviderId, CapabilityName)
);','dev-snapshot','2026-05-17 16:28:57','Adds SQL catalog update tracking and catalog service support.');
INSERT INTO "DatabaseSchemaSnapshot" VALUES('0003_command_registration_foundation',3,'CREATE TABLE IF NOT EXISTS MigrationHistory (
    MigrationId TEXT PRIMARY KEY,
    AppliedUtc  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description TEXT NOT NULL DEFAULT ''''
);

CREATE TABLE IF NOT EXISTS DatabaseSchemaSnapshot (
    MigrationId   TEXT PRIMARY KEY,
    SchemaVersion INTEGER NOT NULL,
    SnapshotSql   TEXT NOT NULL,
    SnapshotHash  TEXT NOT NULL,
    CreatedUtc    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description   TEXT NOT NULL DEFAULT '''',
    FOREIGN KEY (MigrationId) REFERENCES MigrationHistory(MigrationId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS SqlQuery (
    QueryName   TEXT PRIMARY KEY,
    Description TEXT NOT NULL DEFAULT '''',
    SqlText     TEXT NOT NULL DEFAULT '''',
    CreatedUtc  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Category    TEXT NOT NULL DEFAULT '''',
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
    DisplayName  TEXT NOT NULL DEFAULT '''',
    Description  TEXT NOT NULL DEFAULT '''',
    Category     TEXT NOT NULL DEFAULT '''',
    Version      INTEGER NOT NULL DEFAULT 1,
    HandlerKey   TEXT NOT NULL DEFAULT '''',
    HandlerType  TEXT NOT NULL DEFAULT '''',
    InputJson    TEXT NOT NULL DEFAULT ''{}'',
    OutputJson   TEXT NOT NULL DEFAULT ''{}'',
    MetadataJson TEXT NOT NULL DEFAULT ''{}'',
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
    DefaultValue                 TEXT NOT NULL DEFAULT '''',
    Description                  TEXT NOT NULL DEFAULT '''',
    SortOrder                    INTEGER NOT NULL DEFAULT 0,
    CreatedUtc                   TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc                   TEXT NULL,
    FOREIGN KEY (CommandId) REFERENCES CommandDefinition(CommandId) ON DELETE CASCADE,
    UNIQUE (CommandId, ParameterName)
);

CREATE TABLE IF NOT EXISTS WorkflowDefinition (
    WorkflowId    TEXT PRIMARY KEY,
    WorkflowName  TEXT NOT NULL,
    DisplayName   TEXT NOT NULL DEFAULT '''',
    Description   TEXT NOT NULL DEFAULT '''',
    Version       INTEGER NOT NULL DEFAULT 1,
    MetadataJson  TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled     INTEGER NOT NULL DEFAULT 1,
    CreatedUtc    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc    TEXT NULL,
    UNIQUE (WorkflowName, Version)
);

CREATE TABLE IF NOT EXISTS WorkflowStep (
    WorkflowStepId TEXT PRIMARY KEY,
    WorkflowId     TEXT NOT NULL,
    StepOrder      INTEGER NOT NULL,
    StepName       TEXT NOT NULL DEFAULT '''',
    CommandId      TEXT NOT NULL,
    InputMapJson   TEXT NOT NULL DEFAULT ''{}'',
    MetadataJson   TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled      INTEGER NOT NULL DEFAULT 1,
    FOREIGN KEY (WorkflowId) REFERENCES WorkflowDefinition(WorkflowId) ON DELETE CASCADE,
    FOREIGN KEY (CommandId) REFERENCES CommandDefinition(CommandId) ON DELETE RESTRICT,
    UNIQUE (WorkflowId, StepOrder)
);

CREATE TABLE IF NOT EXISTS ExecutionHistory (
    ExecutionId   TEXT PRIMARY KEY,
    ExecutionKind TEXT NOT NULL,
    TargetId      TEXT NOT NULL,
    TargetName    TEXT NOT NULL DEFAULT '''',
    Status        TEXT NOT NULL,
    StartedUtc    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CompletedUtc  TEXT NULL,
    CorrelationId TEXT NOT NULL DEFAULT '''',
    InputJson     TEXT NOT NULL DEFAULT ''{}'',
    OutputJson    TEXT NOT NULL DEFAULT ''{}'',
    ErrorMessage  TEXT NOT NULL DEFAULT '''',
    MetadataJson  TEXT NOT NULL DEFAULT ''{}''
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
    InputJson       TEXT NOT NULL DEFAULT ''{}'',
    OutputJson      TEXT NOT NULL DEFAULT ''{}'',
    ErrorMessage    TEXT NOT NULL DEFAULT '''',
    MetadataJson    TEXT NOT NULL DEFAULT ''{}'',
    FOREIGN KEY (ExecutionId) REFERENCES ExecutionHistory(ExecutionId) ON DELETE CASCADE,
    FOREIGN KEY (WorkflowStepId) REFERENCES WorkflowStep(WorkflowStepId) ON DELETE SET NULL,
    FOREIGN KEY (CommandId) REFERENCES CommandDefinition(CommandId) ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS ExecutionContext (
    ContextId    TEXT PRIMARY KEY,
    ContextName  TEXT NOT NULL UNIQUE,
    Scope        TEXT NOT NULL DEFAULT '''',
    Description  TEXT NOT NULL DEFAULT '''',
    ContextJson  TEXT NOT NULL DEFAULT ''{}'',
    MetadataJson TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled    INTEGER NOT NULL DEFAULT 1,
    CreatedUtc   TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc   TEXT NULL
);

CREATE TABLE IF NOT EXISTS AiProvider (
    AiProviderId      TEXT PRIMARY KEY,
    ProviderName      TEXT NOT NULL UNIQUE,
    DisplayName       TEXT NOT NULL DEFAULT '''',
    ProviderKind      TEXT NOT NULL DEFAULT '''',
    Description       TEXT NOT NULL DEFAULT '''',
    ConfigurationJson TEXT NOT NULL DEFAULT ''{}'',
    MetadataJson      TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled         INTEGER NOT NULL DEFAULT 1,
    CreatedUtc        TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc        TEXT NULL
);

CREATE TABLE IF NOT EXISTS AiProviderCapability (
    AiProviderCapabilityId TEXT PRIMARY KEY,
    AiProviderId           TEXT NOT NULL,
    CapabilityName         TEXT NOT NULL,
    Description            TEXT NOT NULL DEFAULT '''',
    MetadataJson           TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled              INTEGER NOT NULL DEFAULT 1,
    CreatedUtc             TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc             TEXT NULL,
    FOREIGN KEY (AiProviderId) REFERENCES AiProvider(AiProviderId) ON DELETE CASCADE,
    UNIQUE (AiProviderId, CapabilityName)
);','dev-snapshot','2026-05-17 16:28:57','Adds command registration metadata and command parameter definitions.');
INSERT INTO "DatabaseSchemaSnapshot" VALUES('0004_workflow_model_foundation',4,'CREATE TABLE IF NOT EXISTS MigrationHistory (
    MigrationId TEXT PRIMARY KEY,
    AppliedUtc  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description TEXT NOT NULL DEFAULT ''''
);

CREATE TABLE IF NOT EXISTS DatabaseSchemaSnapshot (
    MigrationId   TEXT PRIMARY KEY,
    SchemaVersion INTEGER NOT NULL,
    SnapshotSql   TEXT NOT NULL,
    SnapshotHash  TEXT NOT NULL,
    CreatedUtc    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description   TEXT NOT NULL DEFAULT '''',
    FOREIGN KEY (MigrationId) REFERENCES MigrationHistory(MigrationId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS SqlQuery (
    QueryName   TEXT PRIMARY KEY,
    Description TEXT NOT NULL DEFAULT '''',
    SqlText     TEXT NOT NULL DEFAULT '''',
    CreatedUtc  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Category    TEXT NOT NULL DEFAULT '''',
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
    DisplayName  TEXT NOT NULL DEFAULT '''',
    Description  TEXT NOT NULL DEFAULT '''',
    Category     TEXT NOT NULL DEFAULT '''',
    Version      INTEGER NOT NULL DEFAULT 1,
    HandlerKey   TEXT NOT NULL DEFAULT '''',
    HandlerType  TEXT NOT NULL DEFAULT '''',
    InputJson    TEXT NOT NULL DEFAULT ''{}'',
    OutputJson   TEXT NOT NULL DEFAULT ''{}'',
    MetadataJson TEXT NOT NULL DEFAULT ''{}'',
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
    DefaultValue                 TEXT NOT NULL DEFAULT '''',
    Description                  TEXT NOT NULL DEFAULT '''',
    SortOrder                    INTEGER NOT NULL DEFAULT 0,
    CreatedUtc                   TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc                   TEXT NULL,
    FOREIGN KEY (CommandId) REFERENCES CommandDefinition(CommandId) ON DELETE CASCADE,
    UNIQUE (CommandId, ParameterName)
);

CREATE TABLE IF NOT EXISTS WorkflowDefinition (
    WorkflowId    TEXT PRIMARY KEY,
    WorkflowName  TEXT NOT NULL,
    DisplayName   TEXT NOT NULL DEFAULT '''',
    Description   TEXT NOT NULL DEFAULT '''',
    Version       INTEGER NOT NULL DEFAULT 1,
    MetadataJson  TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled     INTEGER NOT NULL DEFAULT 1,
    CreatedUtc    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc    TEXT NULL,
    UNIQUE (WorkflowName, Version)
);

CREATE TABLE IF NOT EXISTS WorkflowStep (
    WorkflowStepId TEXT PRIMARY KEY,
    WorkflowId     TEXT NOT NULL,
    StepOrder      INTEGER NOT NULL,
    StepName       TEXT NOT NULL DEFAULT '''',
    CommandId      TEXT NOT NULL,
    InputMapJson   TEXT NOT NULL DEFAULT ''{}'',
    MetadataJson   TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled      INTEGER NOT NULL DEFAULT 1,
    FOREIGN KEY (WorkflowId) REFERENCES WorkflowDefinition(WorkflowId) ON DELETE CASCADE,
    FOREIGN KEY (CommandId) REFERENCES CommandDefinition(CommandId) ON DELETE RESTRICT,
    UNIQUE (WorkflowId, StepOrder)
);

CREATE TABLE IF NOT EXISTS ExecutionHistory (
    ExecutionId   TEXT PRIMARY KEY,
    ExecutionKind TEXT NOT NULL,
    TargetId      TEXT NOT NULL,
    TargetName    TEXT NOT NULL DEFAULT '''',
    Status        TEXT NOT NULL,
    StartedUtc    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CompletedUtc  TEXT NULL,
    CorrelationId TEXT NOT NULL DEFAULT '''',
    InputJson     TEXT NOT NULL DEFAULT ''{}'',
    OutputJson    TEXT NOT NULL DEFAULT ''{}'',
    ErrorMessage  TEXT NOT NULL DEFAULT '''',
    MetadataJson  TEXT NOT NULL DEFAULT ''{}''
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
    InputJson       TEXT NOT NULL DEFAULT ''{}'',
    OutputJson      TEXT NOT NULL DEFAULT ''{}'',
    ErrorMessage    TEXT NOT NULL DEFAULT '''',
    MetadataJson    TEXT NOT NULL DEFAULT ''{}'',
    FOREIGN KEY (ExecutionId) REFERENCES ExecutionHistory(ExecutionId) ON DELETE CASCADE,
    FOREIGN KEY (WorkflowStepId) REFERENCES WorkflowStep(WorkflowStepId) ON DELETE SET NULL,
    FOREIGN KEY (CommandId) REFERENCES CommandDefinition(CommandId) ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS ExecutionContext (
    ContextId    TEXT PRIMARY KEY,
    ContextName  TEXT NOT NULL UNIQUE,
    Scope        TEXT NOT NULL DEFAULT '''',
    Description  TEXT NOT NULL DEFAULT '''',
    ContextJson  TEXT NOT NULL DEFAULT ''{}'',
    MetadataJson TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled    INTEGER NOT NULL DEFAULT 1,
    CreatedUtc   TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc   TEXT NULL
);

CREATE TABLE IF NOT EXISTS AiProvider (
    AiProviderId      TEXT PRIMARY KEY,
    ProviderName      TEXT NOT NULL UNIQUE,
    DisplayName       TEXT NOT NULL DEFAULT '''',
    ProviderKind      TEXT NOT NULL DEFAULT '''',
    Description       TEXT NOT NULL DEFAULT '''',
    ConfigurationJson TEXT NOT NULL DEFAULT ''{}'',
    MetadataJson      TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled         INTEGER NOT NULL DEFAULT 1,
    CreatedUtc        TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc        TEXT NULL
);

CREATE TABLE IF NOT EXISTS AiProviderCapability (
    AiProviderCapabilityId TEXT PRIMARY KEY,
    AiProviderId           TEXT NOT NULL,
    CapabilityName         TEXT NOT NULL,
    Description            TEXT NOT NULL DEFAULT '''',
    MetadataJson           TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled              INTEGER NOT NULL DEFAULT 1,
    CreatedUtc             TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc             TEXT NULL,
    FOREIGN KEY (AiProviderId) REFERENCES AiProvider(AiProviderId) ON DELETE CASCADE,
    UNIQUE (AiProviderId, CapabilityName)
);','dev-snapshot','2026-05-17 16:28:57','Adds workflow model metadata, workflow step metadata, and workflow SQL catalog entries.');
INSERT INTO "DatabaseSchemaSnapshot" VALUES('0005_execution_history_foundation',5,'CREATE TABLE IF NOT EXISTS MigrationHistory (
    MigrationId TEXT PRIMARY KEY,
    AppliedUtc  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description TEXT NOT NULL DEFAULT ''''
);

CREATE TABLE IF NOT EXISTS DatabaseSchemaSnapshot (
    MigrationId   TEXT PRIMARY KEY,
    SchemaVersion INTEGER NOT NULL,
    SnapshotSql   TEXT NOT NULL,
    SnapshotHash  TEXT NOT NULL,
    CreatedUtc    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description   TEXT NOT NULL DEFAULT '''',
    FOREIGN KEY (MigrationId) REFERENCES MigrationHistory(MigrationId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS SqlQuery (
    QueryName   TEXT PRIMARY KEY,
    Description TEXT NOT NULL DEFAULT '''',
    SqlText     TEXT NOT NULL DEFAULT '''',
    CreatedUtc  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Category    TEXT NOT NULL DEFAULT '''',
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
    DisplayName  TEXT NOT NULL DEFAULT '''',
    Description  TEXT NOT NULL DEFAULT '''',
    Category     TEXT NOT NULL DEFAULT '''',
    Version      INTEGER NOT NULL DEFAULT 1,
    HandlerKey   TEXT NOT NULL DEFAULT '''',
    HandlerType  TEXT NOT NULL DEFAULT '''',
    InputJson    TEXT NOT NULL DEFAULT ''{}'',
    OutputJson   TEXT NOT NULL DEFAULT ''{}'',
    MetadataJson TEXT NOT NULL DEFAULT ''{}'',
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
    DefaultValue                 TEXT NOT NULL DEFAULT '''',
    Description                  TEXT NOT NULL DEFAULT '''',
    SortOrder                    INTEGER NOT NULL DEFAULT 0,
    CreatedUtc                   TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc                   TEXT NULL,
    FOREIGN KEY (CommandId) REFERENCES CommandDefinition(CommandId) ON DELETE CASCADE,
    UNIQUE (CommandId, ParameterName)
);

CREATE TABLE IF NOT EXISTS WorkflowDefinition (
    WorkflowId    TEXT PRIMARY KEY,
    WorkflowName  TEXT NOT NULL,
    DisplayName   TEXT NOT NULL DEFAULT '''',
    Description   TEXT NOT NULL DEFAULT '''',
    Version       INTEGER NOT NULL DEFAULT 1,
    MetadataJson  TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled     INTEGER NOT NULL DEFAULT 1,
    CreatedUtc    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc    TEXT NULL,
    UNIQUE (WorkflowName, Version)
);

CREATE TABLE IF NOT EXISTS WorkflowStep (
    WorkflowStepId TEXT PRIMARY KEY,
    WorkflowId     TEXT NOT NULL,
    StepOrder      INTEGER NOT NULL,
    StepName       TEXT NOT NULL DEFAULT '''',
    CommandId      TEXT NOT NULL,
    InputMapJson   TEXT NOT NULL DEFAULT ''{}'',
    MetadataJson   TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled      INTEGER NOT NULL DEFAULT 1,
    FOREIGN KEY (WorkflowId) REFERENCES WorkflowDefinition(WorkflowId) ON DELETE CASCADE,
    FOREIGN KEY (CommandId) REFERENCES CommandDefinition(CommandId) ON DELETE RESTRICT,
    UNIQUE (WorkflowId, StepOrder)
);

CREATE TABLE IF NOT EXISTS ExecutionHistory (
    ExecutionId   TEXT PRIMARY KEY,
    ExecutionKind TEXT NOT NULL,
    TargetId      TEXT NOT NULL,
    TargetName    TEXT NOT NULL DEFAULT '''',
    Status        TEXT NOT NULL,
    StartedUtc    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CompletedUtc  TEXT NULL,
    CorrelationId TEXT NOT NULL DEFAULT '''',
    InputJson     TEXT NOT NULL DEFAULT ''{}'',
    OutputJson    TEXT NOT NULL DEFAULT ''{}'',
    ErrorMessage  TEXT NOT NULL DEFAULT '''',
    MetadataJson  TEXT NOT NULL DEFAULT ''{}''
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
    InputJson       TEXT NOT NULL DEFAULT ''{}'',
    OutputJson      TEXT NOT NULL DEFAULT ''{}'',
    ErrorMessage    TEXT NOT NULL DEFAULT '''',
    MetadataJson    TEXT NOT NULL DEFAULT ''{}'',
    FOREIGN KEY (ExecutionId) REFERENCES ExecutionHistory(ExecutionId) ON DELETE CASCADE,
    FOREIGN KEY (WorkflowStepId) REFERENCES WorkflowStep(WorkflowStepId) ON DELETE SET NULL,
    FOREIGN KEY (CommandId) REFERENCES CommandDefinition(CommandId) ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS ExecutionContext (
    ContextId    TEXT PRIMARY KEY,
    ContextName  TEXT NOT NULL UNIQUE,
    Scope        TEXT NOT NULL DEFAULT '''',
    Description  TEXT NOT NULL DEFAULT '''',
    ContextJson  TEXT NOT NULL DEFAULT ''{}'',
    MetadataJson TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled    INTEGER NOT NULL DEFAULT 1,
    CreatedUtc   TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc   TEXT NULL
);

CREATE TABLE IF NOT EXISTS AiProvider (
    AiProviderId      TEXT PRIMARY KEY,
    ProviderName      TEXT NOT NULL UNIQUE,
    DisplayName       TEXT NOT NULL DEFAULT '''',
    ProviderKind      TEXT NOT NULL DEFAULT '''',
    Description       TEXT NOT NULL DEFAULT '''',
    ConfigurationJson TEXT NOT NULL DEFAULT ''{}'',
    MetadataJson      TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled         INTEGER NOT NULL DEFAULT 1,
    CreatedUtc        TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc        TEXT NULL
);

CREATE TABLE IF NOT EXISTS AiProviderCapability (
    AiProviderCapabilityId TEXT PRIMARY KEY,
    AiProviderId           TEXT NOT NULL,
    CapabilityName         TEXT NOT NULL,
    Description            TEXT NOT NULL DEFAULT '''',
    MetadataJson           TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled              INTEGER NOT NULL DEFAULT 1,
    CreatedUtc             TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc             TEXT NULL,
    FOREIGN KEY (AiProviderId) REFERENCES AiProvider(AiProviderId) ON DELETE CASCADE,
    UNIQUE (AiProviderId, CapabilityName)
);','dev-snapshot','2026-05-17 16:28:57','Adds execution history repository support, correlation tracking, and execution SQL catalog entries.');
INSERT INTO "DatabaseSchemaSnapshot" VALUES('0006_context_system_foundation',6,'CREATE TABLE IF NOT EXISTS MigrationHistory (
    MigrationId TEXT PRIMARY KEY,
    AppliedUtc  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description TEXT NOT NULL DEFAULT ''''
);

CREATE TABLE IF NOT EXISTS DatabaseSchemaSnapshot (
    MigrationId   TEXT PRIMARY KEY,
    SchemaVersion INTEGER NOT NULL,
    SnapshotSql   TEXT NOT NULL,
    SnapshotHash  TEXT NOT NULL,
    CreatedUtc    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description   TEXT NOT NULL DEFAULT '''',
    FOREIGN KEY (MigrationId) REFERENCES MigrationHistory(MigrationId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS SqlQuery (
    QueryName   TEXT PRIMARY KEY,
    Description TEXT NOT NULL DEFAULT '''',
    SqlText     TEXT NOT NULL DEFAULT '''',
    CreatedUtc  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Category    TEXT NOT NULL DEFAULT '''',
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
    DisplayName  TEXT NOT NULL DEFAULT '''',
    Description  TEXT NOT NULL DEFAULT '''',
    Category     TEXT NOT NULL DEFAULT '''',
    Version      INTEGER NOT NULL DEFAULT 1,
    HandlerKey   TEXT NOT NULL DEFAULT '''',
    HandlerType  TEXT NOT NULL DEFAULT '''',
    InputJson    TEXT NOT NULL DEFAULT ''{}'',
    OutputJson   TEXT NOT NULL DEFAULT ''{}'',
    MetadataJson TEXT NOT NULL DEFAULT ''{}'',
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
    DefaultValue                 TEXT NOT NULL DEFAULT '''',
    Description                  TEXT NOT NULL DEFAULT '''',
    SortOrder                    INTEGER NOT NULL DEFAULT 0,
    CreatedUtc                   TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc                   TEXT NULL,
    FOREIGN KEY (CommandId) REFERENCES CommandDefinition(CommandId) ON DELETE CASCADE,
    UNIQUE (CommandId, ParameterName)
);

CREATE TABLE IF NOT EXISTS WorkflowDefinition (
    WorkflowId    TEXT PRIMARY KEY,
    WorkflowName  TEXT NOT NULL,
    DisplayName   TEXT NOT NULL DEFAULT '''',
    Description   TEXT NOT NULL DEFAULT '''',
    Version       INTEGER NOT NULL DEFAULT 1,
    MetadataJson  TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled     INTEGER NOT NULL DEFAULT 1,
    CreatedUtc    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc    TEXT NULL,
    UNIQUE (WorkflowName, Version)
);

CREATE TABLE IF NOT EXISTS WorkflowStep (
    WorkflowStepId TEXT PRIMARY KEY,
    WorkflowId     TEXT NOT NULL,
    StepOrder      INTEGER NOT NULL,
    StepName       TEXT NOT NULL DEFAULT '''',
    CommandId      TEXT NOT NULL,
    InputMapJson   TEXT NOT NULL DEFAULT ''{}'',
    MetadataJson   TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled      INTEGER NOT NULL DEFAULT 1,
    FOREIGN KEY (WorkflowId) REFERENCES WorkflowDefinition(WorkflowId) ON DELETE CASCADE,
    FOREIGN KEY (CommandId) REFERENCES CommandDefinition(CommandId) ON DELETE RESTRICT,
    UNIQUE (WorkflowId, StepOrder)
);

CREATE TABLE IF NOT EXISTS ExecutionHistory (
    ExecutionId   TEXT PRIMARY KEY,
    ExecutionKind TEXT NOT NULL,
    TargetId      TEXT NOT NULL,
    TargetName    TEXT NOT NULL DEFAULT '''',
    Status        TEXT NOT NULL,
    StartedUtc    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CompletedUtc  TEXT NULL,
    CorrelationId TEXT NOT NULL DEFAULT '''',
    InputJson     TEXT NOT NULL DEFAULT ''{}'',
    OutputJson    TEXT NOT NULL DEFAULT ''{}'',
    ErrorMessage  TEXT NOT NULL DEFAULT '''',
    MetadataJson  TEXT NOT NULL DEFAULT ''{}''
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
    InputJson       TEXT NOT NULL DEFAULT ''{}'',
    OutputJson      TEXT NOT NULL DEFAULT ''{}'',
    ErrorMessage    TEXT NOT NULL DEFAULT '''',
    MetadataJson    TEXT NOT NULL DEFAULT ''{}'',
    FOREIGN KEY (ExecutionId) REFERENCES ExecutionHistory(ExecutionId) ON DELETE CASCADE,
    FOREIGN KEY (WorkflowStepId) REFERENCES WorkflowStep(WorkflowStepId) ON DELETE SET NULL,
    FOREIGN KEY (CommandId) REFERENCES CommandDefinition(CommandId) ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS ExecutionContext (
    ContextId    TEXT PRIMARY KEY,
    ContextName  TEXT NOT NULL UNIQUE,
    Scope        TEXT NOT NULL DEFAULT '''',
    Description  TEXT NOT NULL DEFAULT '''',
    ContextJson  TEXT NOT NULL DEFAULT ''{}'',
    MetadataJson TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled    INTEGER NOT NULL DEFAULT 1,
    CreatedUtc   TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc   TEXT NULL
);

CREATE TABLE IF NOT EXISTS AiProvider (
    AiProviderId      TEXT PRIMARY KEY,
    ProviderName      TEXT NOT NULL UNIQUE,
    DisplayName       TEXT NOT NULL DEFAULT '''',
    ProviderKind      TEXT NOT NULL DEFAULT '''',
    Description       TEXT NOT NULL DEFAULT '''',
    ConfigurationJson TEXT NOT NULL DEFAULT ''{}'',
    MetadataJson      TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled         INTEGER NOT NULL DEFAULT 1,
    CreatedUtc        TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc        TEXT NULL
);

CREATE TABLE IF NOT EXISTS AiProviderCapability (
    AiProviderCapabilityId TEXT PRIMARY KEY,
    AiProviderId           TEXT NOT NULL,
    CapabilityName         TEXT NOT NULL,
    Description            TEXT NOT NULL DEFAULT '''',
    MetadataJson           TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled              INTEGER NOT NULL DEFAULT 1,
    CreatedUtc             TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc             TEXT NULL,
    FOREIGN KEY (AiProviderId) REFERENCES AiProvider(AiProviderId) ON DELETE CASCADE,
    UNIQUE (AiProviderId, CapabilityName)
);','dev-snapshot','2026-05-17 16:28:57','Adds execution context repository support and context SQL catalog entries.');
INSERT INTO "DatabaseSchemaSnapshot" VALUES('0007_ai_provider_foundation',7,'CREATE TABLE IF NOT EXISTS MigrationHistory (
    MigrationId TEXT PRIMARY KEY,
    AppliedUtc  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description TEXT NOT NULL DEFAULT ''''
);

CREATE TABLE IF NOT EXISTS DatabaseSchemaSnapshot (
    MigrationId   TEXT PRIMARY KEY,
    SchemaVersion INTEGER NOT NULL,
    SnapshotSql   TEXT NOT NULL,
    SnapshotHash  TEXT NOT NULL,
    CreatedUtc    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description   TEXT NOT NULL DEFAULT '''',
    FOREIGN KEY (MigrationId) REFERENCES MigrationHistory(MigrationId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS SqlQuery (
    QueryName   TEXT PRIMARY KEY,
    Description TEXT NOT NULL DEFAULT '''',
    SqlText     TEXT NOT NULL DEFAULT '''',
    CreatedUtc  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Category    TEXT NOT NULL DEFAULT '''',
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
    DisplayName  TEXT NOT NULL DEFAULT '''',
    Description  TEXT NOT NULL DEFAULT '''',
    Category     TEXT NOT NULL DEFAULT '''',
    Version      INTEGER NOT NULL DEFAULT 1,
    HandlerKey   TEXT NOT NULL DEFAULT '''',
    HandlerType  TEXT NOT NULL DEFAULT '''',
    InputJson    TEXT NOT NULL DEFAULT ''{}'',
    OutputJson   TEXT NOT NULL DEFAULT ''{}'',
    MetadataJson TEXT NOT NULL DEFAULT ''{}'',
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
    DefaultValue                 TEXT NOT NULL DEFAULT '''',
    Description                  TEXT NOT NULL DEFAULT '''',
    SortOrder                    INTEGER NOT NULL DEFAULT 0,
    CreatedUtc                   TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc                   TEXT NULL,
    FOREIGN KEY (CommandId) REFERENCES CommandDefinition(CommandId) ON DELETE CASCADE,
    UNIQUE (CommandId, ParameterName)
);

CREATE TABLE IF NOT EXISTS WorkflowDefinition (
    WorkflowId    TEXT PRIMARY KEY,
    WorkflowName  TEXT NOT NULL,
    DisplayName   TEXT NOT NULL DEFAULT '''',
    Description   TEXT NOT NULL DEFAULT '''',
    Version       INTEGER NOT NULL DEFAULT 1,
    MetadataJson  TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled     INTEGER NOT NULL DEFAULT 1,
    CreatedUtc    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc    TEXT NULL,
    UNIQUE (WorkflowName, Version)
);

CREATE TABLE IF NOT EXISTS WorkflowStep (
    WorkflowStepId TEXT PRIMARY KEY,
    WorkflowId     TEXT NOT NULL,
    StepOrder      INTEGER NOT NULL,
    StepName       TEXT NOT NULL DEFAULT '''',
    CommandId      TEXT NOT NULL,
    InputMapJson   TEXT NOT NULL DEFAULT ''{}'',
    MetadataJson   TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled      INTEGER NOT NULL DEFAULT 1,
    FOREIGN KEY (WorkflowId) REFERENCES WorkflowDefinition(WorkflowId) ON DELETE CASCADE,
    FOREIGN KEY (CommandId) REFERENCES CommandDefinition(CommandId) ON DELETE RESTRICT,
    UNIQUE (WorkflowId, StepOrder)
);

CREATE TABLE IF NOT EXISTS ExecutionHistory (
    ExecutionId   TEXT PRIMARY KEY,
    ExecutionKind TEXT NOT NULL,
    TargetId      TEXT NOT NULL,
    TargetName    TEXT NOT NULL DEFAULT '''',
    Status        TEXT NOT NULL,
    StartedUtc    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CompletedUtc  TEXT NULL,
    CorrelationId TEXT NOT NULL DEFAULT '''',
    InputJson     TEXT NOT NULL DEFAULT ''{}'',
    OutputJson    TEXT NOT NULL DEFAULT ''{}'',
    ErrorMessage  TEXT NOT NULL DEFAULT '''',
    MetadataJson  TEXT NOT NULL DEFAULT ''{}''
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
    InputJson       TEXT NOT NULL DEFAULT ''{}'',
    OutputJson      TEXT NOT NULL DEFAULT ''{}'',
    ErrorMessage    TEXT NOT NULL DEFAULT '''',
    MetadataJson    TEXT NOT NULL DEFAULT ''{}'',
    FOREIGN KEY (ExecutionId) REFERENCES ExecutionHistory(ExecutionId) ON DELETE CASCADE,
    FOREIGN KEY (WorkflowStepId) REFERENCES WorkflowStep(WorkflowStepId) ON DELETE SET NULL,
    FOREIGN KEY (CommandId) REFERENCES CommandDefinition(CommandId) ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS ExecutionContext (
    ContextId    TEXT PRIMARY KEY,
    ContextName  TEXT NOT NULL UNIQUE,
    Scope        TEXT NOT NULL DEFAULT '''',
    Description  TEXT NOT NULL DEFAULT '''',
    ContextJson  TEXT NOT NULL DEFAULT ''{}'',
    MetadataJson TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled    INTEGER NOT NULL DEFAULT 1,
    CreatedUtc   TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc   TEXT NULL
);

CREATE TABLE IF NOT EXISTS AiProvider (
    AiProviderId      TEXT PRIMARY KEY,
    ProviderName      TEXT NOT NULL UNIQUE,
    DisplayName       TEXT NOT NULL DEFAULT '''',
    ProviderKind      TEXT NOT NULL DEFAULT '''',
    Description       TEXT NOT NULL DEFAULT '''',
    ConfigurationJson TEXT NOT NULL DEFAULT ''{}'',
    MetadataJson      TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled         INTEGER NOT NULL DEFAULT 1,
    CreatedUtc        TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc        TEXT NULL
);

CREATE TABLE IF NOT EXISTS AiProviderCapability (
    AiProviderCapabilityId TEXT PRIMARY KEY,
    AiProviderId           TEXT NOT NULL,
    CapabilityName         TEXT NOT NULL,
    Description            TEXT NOT NULL DEFAULT '''',
    MetadataJson           TEXT NOT NULL DEFAULT ''{}'',
    IsEnabled              INTEGER NOT NULL DEFAULT 1,
    CreatedUtc             TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc             TEXT NULL,
    FOREIGN KEY (AiProviderId) REFERENCES AiProvider(AiProviderId) ON DELETE CASCADE,
    UNIQUE (AiProviderId, CapabilityName)
);','dev-snapshot','2026-05-17 16:28:57','Adds AI provider repository support, provider capabilities, and AI provider SQL catalog entries.');
CREATE TABLE ExecutionContext (
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
CREATE TABLE ExecutionHistory (
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
CREATE TABLE ExecutionStepHistory (
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
CREATE TABLE Log (
    LogId     INTEGER PRIMARY KEY AUTOINCREMENT,
    Timestamp TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Action    TEXT NOT NULL,
    Details   TEXT NULL
);
CREATE TABLE MigrationHistory (
    MigrationId TEXT PRIMARY KEY,
    AppliedUtc  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description TEXT NOT NULL DEFAULT ''
);
INSERT INTO "MigrationHistory" VALUES('0001_engine_database_foundation','2026-05-17 16:28:57','Initial command engine database foundation.');
INSERT INTO "MigrationHistory" VALUES('0002_sql_catalog_foundation','2026-05-17 16:28:57','Adds SQL catalog update tracking and catalog service support.');
INSERT INTO "MigrationHistory" VALUES('0003_command_registration_foundation','2026-05-17 16:28:57','Adds command registration metadata and command parameter definitions.');
INSERT INTO "MigrationHistory" VALUES('0004_workflow_model_foundation','2026-05-17 16:28:57','Adds workflow model metadata, workflow step metadata, and workflow SQL catalog entries.');
INSERT INTO "MigrationHistory" VALUES('0005_execution_history_foundation','2026-05-17 16:28:57','Adds execution history repository support, correlation tracking, and execution SQL catalog entries.');
INSERT INTO "MigrationHistory" VALUES('0006_context_system_foundation','2026-05-17 16:28:57','Adds execution context repository support and context SQL catalog entries.');
INSERT INTO "MigrationHistory" VALUES('0007_ai_provider_foundation','2026-05-17 16:28:57','Adds AI provider repository support, provider capabilities, and AI provider SQL catalog entries.');
CREATE TABLE SqlQuery (
    QueryName   TEXT PRIMARY KEY,
    Description TEXT NOT NULL DEFAULT '',
    SqlText     TEXT NOT NULL DEFAULT '',
    CreatedUtc  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Category    TEXT NOT NULL DEFAULT '',
    IsActive    INTEGER NOT NULL DEFAULT 1,
    UpdatedUtc  TEXT NULL
);
INSERT INTO "SqlQuery" VALUES('SqlQuery_SelectByName','Selects one SQL catalog entry by name.','SELECT QueryName,
       Description,
       SqlText,
       Category,
       IsActive,
       CreatedUtc,
       UpdatedUtc
FROM SqlQuery
WHERE QueryName = $QueryName;','2026-05-17 16:28:57','SQL Catalog',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('SqlQuery_SelectAll','Lists all SQL catalog entries.','SELECT QueryName,
       Description,
       SqlText,
       Category,
       IsActive,
       CreatedUtc,
       UpdatedUtc
FROM SqlQuery
ORDER BY Category ASC,
         QueryName ASC;','2026-05-17 16:28:57','SQL Catalog',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('SqlQuery_InsertOrReplace','Inserts or updates one SQL catalog entry.','INSERT INTO SqlQuery (
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
    UpdatedUtc = CURRENT_TIMESTAMP;','2026-05-17 16:28:57','SQL Catalog',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('CommandDefinition_InsertOrReplace','Inserts or updates a command definition by command name.','INSERT INTO CommandDefinition (
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
    UpdatedUtc = CURRENT_TIMESTAMP;','2026-05-17 16:28:57','Command Registration',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('CommandDefinition_SelectByName','Selects a command definition by command name.','SELECT CommandId,
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
WHERE CommandName = $CommandName;','2026-05-17 16:28:57','Command Registration',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('CommandDefinition_SelectAll','Lists all command definitions ordered for UI display.','SELECT CommandId,
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
         CommandName ASC;','2026-05-17 16:28:57','Command Registration',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('CommandParameterDefinition_InsertOrReplace','Inserts or updates a command parameter definition.','INSERT INTO CommandParameterDefinition (
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
    UpdatedUtc = CURRENT_TIMESTAMP;','2026-05-17 16:28:57','Command Registration',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('CommandParameterDefinition_SelectByCommand','Lists parameter definitions for a command definition.','SELECT CommandParameterDefinitionId,
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
         ParameterName ASC;','2026-05-17 16:28:57','Command Registration',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('WorkflowDefinition_InsertOrReplace','Inserts or updates a workflow definition by workflow name and version.','INSERT INTO WorkflowDefinition (
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
    UpdatedUtc = CURRENT_TIMESTAMP;','2026-05-17 16:28:57','Workflow Foundation',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('WorkflowDefinition_SelectAll','Lists workflow definitions ordered for UI display.','SELECT WorkflowId,
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
         Version DESC;','2026-05-17 16:28:57','Workflow Foundation',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('WorkflowDefinition_SelectByNameAndVersion','Selects a workflow definition by workflow name and version.','SELECT WorkflowId,
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
  AND Version = $Version;','2026-05-17 16:28:57','Workflow Foundation',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('WorkflowStep_InsertOrReplace','Inserts or updates a workflow step by workflow and step order.','INSERT INTO WorkflowStep (
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
    IsEnabled = excluded.IsEnabled;','2026-05-17 16:28:57','Workflow Foundation',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('WorkflowStep_SelectByWorkflow','Lists workflow steps for one workflow definition.','SELECT WorkflowStepId,
       WorkflowId,
       StepOrder,
       StepName,
       CommandId,
       InputMapJson,
       MetadataJson,
       IsEnabled
FROM WorkflowStep
WHERE WorkflowId = $WorkflowId
ORDER BY StepOrder ASC;','2026-05-17 16:28:57','Workflow Foundation',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('ExecutionHistory_Insert','Inserts an execution history record when a command or workflow starts.','INSERT INTO ExecutionHistory (
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
);','2026-05-17 16:28:57','Execution History',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('ExecutionHistory_Complete','Completes an execution history record by execution id.','UPDATE ExecutionHistory
SET Status = $Status,
    CompletedUtc = $CompletedUtc,
    OutputJson = $OutputJson,
    ErrorMessage = $ErrorMessage,
    MetadataJson = $MetadataJson
WHERE ExecutionId = $ExecutionId;','2026-05-17 16:28:57','Execution History',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('ExecutionHistory_SelectRecent','Lists recent execution history records for UI display.','SELECT ExecutionId,
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
LIMIT $Limit;','2026-05-17 16:28:57','Execution History',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('ExecutionStepHistory_Insert','Inserts an execution step history record.','INSERT INTO ExecutionStepHistory (
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
);','2026-05-17 16:28:57','Execution History',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('ExecutionStepHistory_Complete','Completes one execution step history record.','UPDATE ExecutionStepHistory
SET Status = $Status,
    CompletedUtc = $CompletedUtc,
    OutputJson = $OutputJson,
    ErrorMessage = $ErrorMessage,
    MetadataJson = $MetadataJson
WHERE ExecutionStepId = $ExecutionStepId;','2026-05-17 16:28:57','Execution History',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('ExecutionStepHistory_SelectByExecution','Lists execution step history records for an execution.','SELECT ExecutionStepId,
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
ORDER BY StepOrder ASC;','2026-05-17 16:28:57','Execution History',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('ExecutionContext_SelectAll','Lists execution contexts ordered by scope and name.','SELECT ContextId,
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
         ContextName ASC;','2026-05-17 16:28:57','Context System Foundation',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('ExecutionContext_SelectByName','Selects an execution context by context name.','SELECT ContextId,
       ContextName,
       Scope,
       Description,
       ContextJson,
       MetadataJson,
       IsEnabled,
       CreatedUtc,
       UpdatedUtc
FROM ExecutionContext
WHERE ContextName = $ContextName;','2026-05-17 16:28:57','Context System Foundation',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('ExecutionContext_InsertOrReplace','Inserts or updates an execution context by context name.','INSERT INTO ExecutionContext (
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
    UpdatedUtc = CURRENT_TIMESTAMP;','2026-05-17 16:28:57','Context System Foundation',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('EngineContext_SelectAll','Compatibility alias for ExecutionContext_SelectAll.','SELECT ContextId,
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
         ContextName ASC;','2026-05-17 16:28:57','Context System Foundation',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('EngineContext_SelectByName','Compatibility alias for ExecutionContext_SelectByName.','SELECT ContextId,
       ContextName,
       Scope,
       Description,
       ContextJson,
       MetadataJson,
       IsEnabled,
       CreatedUtc,
       UpdatedUtc
FROM ExecutionContext
WHERE ContextName = $ContextName;','2026-05-17 16:28:57','Context System Foundation',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('EngineContext_InsertOrReplace','Compatibility alias for ExecutionContext_InsertOrReplace.','INSERT INTO ExecutionContext (
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
    UpdatedUtc = CURRENT_TIMESTAMP;','2026-05-17 16:28:57','Context System Foundation',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('AiProvider_SelectAll','Lists AI providers ordered by provider kind and name.','SELECT AiProviderId,
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
         ProviderName ASC;','2026-05-17 16:28:57','AI Provider Foundation',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('AiProvider_SelectByName','Selects an AI provider by provider name.','SELECT AiProviderId,
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
WHERE ProviderName = $ProviderName;','2026-05-17 16:28:57','AI Provider Foundation',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('AiProvider_InsertOrReplace','Inserts or updates an AI provider by provider name.','INSERT INTO AiProvider (
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
    UpdatedUtc = CURRENT_TIMESTAMP;','2026-05-17 16:28:57','AI Provider Foundation',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('AiProviderCapability_InsertOrReplace','Inserts or updates an AI provider capability.','INSERT INTO AiProviderCapability (
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
    UpdatedUtc = CURRENT_TIMESTAMP;','2026-05-17 16:28:57','AI Provider Foundation',1,'2026-05-17 16:28:57');
INSERT INTO "SqlQuery" VALUES('AiProviderCapability_SelectByProvider','Lists AI provider capabilities for one provider.','SELECT AiProviderCapabilityId,
       AiProviderId,
       CapabilityName,
       Description,
       MetadataJson,
       IsEnabled,
       CreatedUtc,
       UpdatedUtc
FROM AiProviderCapability
WHERE AiProviderId = $AiProviderId
ORDER BY CapabilityName ASC;','2026-05-17 16:28:57','AI Provider Foundation',1,'2026-05-17 16:28:57');
CREATE TABLE WorkflowDefinition (
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
CREATE TABLE WorkflowStep (
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
CREATE INDEX IX_ExecutionHistory_CorrelationId
ON ExecutionHistory(CorrelationId);
CREATE INDEX IX_ExecutionHistory_StartedUtc
ON ExecutionHistory(StartedUtc);
CREATE INDEX IX_ExecutionStepHistory_ExecutionId
ON ExecutionStepHistory(ExecutionId);
DELETE FROM "sqlite_sequence";
COMMIT;