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