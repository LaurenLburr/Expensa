PRAGMA foreign_keys = ON;

-- =========================================================
-- SqlQuery
-- =========================================================

CREATE TABLE IF NOT EXISTS SqlQuery (
    QueryName   TEXT PRIMARY KEY,
    Description TEXT NOT NULL DEFAULT (''),
    SqlText     TEXT NOT NULL DEFAULT (''),
    CreatedUtc  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Category    TEXT NOT NULL DEFAULT (''),
    IsActive    INTEGER NOT NULL DEFAULT (1),
    UNIQUE (QueryName ASC)
);

-- =========================================================
-- ExecutionHistory
-- =========================================================

CREATE TABLE IF NOT EXISTS ExecutionHistory (
    ExecutionId        TEXT PRIMARY KEY,
    CommandName        TEXT NOT NULL,
    ExecutionKind      TEXT NOT NULL,
    TargetId           TEXT NOT NULL,
    TargetName         TEXT NOT NULL,
    CorrelationId      TEXT NOT NULL DEFAULT '',
    Status             TEXT NOT NULL,
    StartedUtc         TEXT NOT NULL,
    CompletedUtc       TEXT NULL,
    DurationMs         INTEGER NULL,
    ErrorMessage       TEXT NULL,
    InputJson          TEXT NOT NULL DEFAULT '{}',
    OutputJson         TEXT NOT NULL DEFAULT '{}',
    MetadataJson       TEXT NOT NULL DEFAULT '{}'
);

CREATE INDEX IF NOT EXISTS IX_ExecutionHistory_Status
ON ExecutionHistory (Status);

CREATE INDEX IF NOT EXISTS IX_ExecutionHistory_CommandName
ON ExecutionHistory (CommandName);

CREATE INDEX IF NOT EXISTS IX_ExecutionHistory_CorrelationId
ON ExecutionHistory (CorrelationId);

-- =========================================================
-- ExecutionStepHistory
-- =========================================================

CREATE TABLE IF NOT EXISTS ExecutionStepHistory (
    ExecutionStepId    TEXT PRIMARY KEY,
    ExecutionId        TEXT NOT NULL,
    StepName           TEXT NOT NULL,
    StepOrder          INTEGER NOT NULL,
    Status             TEXT NOT NULL,
    StartedUtc         TEXT NOT NULL,
    CompletedUtc       TEXT NULL,
    DurationMs         INTEGER NULL,
    ErrorMessage       TEXT NULL,
    InputJson          TEXT NOT NULL DEFAULT '{}',
    OutputJson         TEXT NOT NULL DEFAULT '{}',
    MetadataJson       TEXT NOT NULL DEFAULT '{}',

    FOREIGN KEY (ExecutionId)
        REFERENCES ExecutionHistory (ExecutionId)
        ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS IX_ExecutionStepHistory_ExecutionId
ON ExecutionStepHistory (ExecutionId);

CREATE INDEX IF NOT EXISTS IX_ExecutionStepHistory_Status
ON ExecutionStepHistory (Status);

-- =========================================================
-- WorkflowExecution
-- =========================================================

CREATE TABLE IF NOT EXISTS WorkflowExecution (
    WorkflowExecutionId      TEXT PRIMARY KEY,
    WorkflowName             TEXT NOT NULL,
    CorrelationId            TEXT NOT NULL DEFAULT '',
    Status                   TEXT NOT NULL,
    StartedUtc               TEXT NOT NULL,
    CompletedUtc             TEXT NULL,
    LastHeartbeatUtc         TEXT NULL,
    LastCompletedStepOrder   INTEGER NOT NULL DEFAULT 0,
    CurrentStepName          TEXT NOT NULL DEFAULT '',
    RuntimeStateJson         TEXT NOT NULL DEFAULT '{}',
    ResumeToken              TEXT NOT NULL DEFAULT '',
    ErrorMessage             TEXT NULL,
    MetadataJson             TEXT NOT NULL DEFAULT '{}'
);

CREATE INDEX IF NOT EXISTS IX_WorkflowExecution_Status
ON WorkflowExecution (Status);

CREATE INDEX IF NOT EXISTS IX_WorkflowExecution_LastHeartbeatUtc
ON WorkflowExecution (LastHeartbeatUtc);

CREATE INDEX IF NOT EXISTS IX_WorkflowExecution_WorkflowName
ON WorkflowExecution (WorkflowName);

CREATE INDEX IF NOT EXISTS IX_WorkflowExecution_CorrelationId
ON WorkflowExecution (CorrelationId);

-- =========================================================
-- WorkflowExecutionStep
-- =========================================================

CREATE TABLE IF NOT EXISTS WorkflowExecutionStep (
    WorkflowExecutionStepId  TEXT PRIMARY KEY,
    WorkflowExecutionId      TEXT NOT NULL,
    StepName                 TEXT NOT NULL,
    StepOrder                INTEGER NOT NULL,
    CommandName              TEXT NOT NULL,
    Status                   TEXT NOT NULL,
    StartedUtc               TEXT NOT NULL,
    CompletedUtc             TEXT NULL,
    DurationMs               INTEGER NULL,
    ErrorMessage             TEXT NULL,
    InputJson                TEXT NOT NULL DEFAULT '{}',
    OutputJson               TEXT NOT NULL DEFAULT '{}',
    MetadataJson             TEXT NOT NULL DEFAULT '{}',

    FOREIGN KEY (WorkflowExecutionId)
        REFERENCES WorkflowExecution (WorkflowExecutionId)
        ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS IX_WorkflowExecutionStep_WorkflowExecutionId
ON WorkflowExecutionStep (WorkflowExecutionId);

CREATE INDEX IF NOT EXISTS IX_WorkflowExecutionStep_Status
ON WorkflowExecutionStep (Status);

CREATE INDEX IF NOT EXISTS IX_WorkflowExecutionStep_CommandName
ON WorkflowExecutionStep (CommandName);

-- =========================================================
-- WorkflowDefinition
-- =========================================================

CREATE TABLE IF NOT EXISTS WorkflowDefinition (
    WorkflowDefinitionId TEXT NOT NULL DEFAULT '',
    WorkflowName         TEXT NOT NULL UNIQUE,
    DisplayName          TEXT NOT NULL DEFAULT '',
    Description          TEXT NOT NULL DEFAULT '',
    Version              INTEGER NOT NULL DEFAULT 1,
    IsActive             INTEGER NOT NULL DEFAULT 1,
    CreatedUtc           TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc           TEXT NULL
);

CREATE INDEX IF NOT EXISTS IX_WorkflowDefinition_IsActive
ON WorkflowDefinition (IsActive);

CREATE INDEX IF NOT EXISTS IX_WorkflowDefinition_WorkflowName
ON WorkflowDefinition (WorkflowName);

-- =========================================================
-- WorkflowDefinitionStep
-- =========================================================

CREATE TABLE IF NOT EXISTS WorkflowDefinitionStep (
    WorkflowDefinitionStepId TEXT NOT NULL DEFAULT '',
    WorkflowDefinitionId     TEXT NOT NULL,
    StepName                 TEXT NOT NULL,
    StepOrder                INTEGER NOT NULL,
    CommandName              TEXT NOT NULL,

    ParametersJson           TEXT NOT NULL DEFAULT '{}',

    IsEnabled                INTEGER NOT NULL DEFAULT 1,
    CommandDefinitionId      TEXT NOT NULL DEFAULT '',
    InputMapJson             TEXT NOT NULL DEFAULT '{}',

    CreatedUtc               TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc               TEXT NULL,

    UNIQUE (WorkflowDefinitionId, StepOrder)
);

CREATE INDEX IF NOT EXISTS IX_WorkflowDefinitionStep_WorkflowDefinitionId
ON WorkflowDefinitionStep (WorkflowDefinitionId);

CREATE INDEX IF NOT EXISTS IX_WorkflowDefinitionStep_CommandName
ON WorkflowDefinitionStep (CommandName);

CREATE INDEX IF NOT EXISTS IX_WorkflowDefinitionStep_IsEnabled
ON WorkflowDefinitionStep (IsEnabled);

-- =========================================================
-- AiProvider
-- =========================================================

CREATE TABLE IF NOT EXISTS AiProvider (
    AiProviderId         TEXT PRIMARY KEY,
    ProviderName         TEXT NOT NULL UNIQUE,
    DisplayName          TEXT NOT NULL,
    IsEnabled            INTEGER NOT NULL DEFAULT 1,
    BaseUrl              TEXT NOT NULL DEFAULT '',
    ApiKeyEnvironmentVar TEXT NOT NULL DEFAULT '',
    MetadataJson         TEXT NOT NULL DEFAULT '{}',
    CreatedUtc           TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc           TEXT NULL
);

CREATE INDEX IF NOT EXISTS IX_AiProvider_IsEnabled
ON AiProvider (IsEnabled);

CREATE INDEX IF NOT EXISTS IX_AiProvider_ProviderName
ON AiProvider (ProviderName);

-- =========================================================
-- AiProviderCapability
-- =========================================================

CREATE TABLE IF NOT EXISTS AiProviderCapability (
    AiProviderCapabilityId TEXT PRIMARY KEY,
    AiProviderId           TEXT NOT NULL,
    CapabilityKind         TEXT NOT NULL,
    CapabilityValue        TEXT NOT NULL,
    MetadataJson           TEXT NOT NULL DEFAULT '{}',
    UpdatedUtc             TEXT NULL,

    UNIQUE (AiProviderId, CapabilityKind, CapabilityValue),

    FOREIGN KEY (AiProviderId)
        REFERENCES AiProvider (AiProviderId)
        ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS IX_AiProviderCapability_AiProviderId
ON AiProviderCapability (AiProviderId);

CREATE INDEX IF NOT EXISTS IX_AiProviderCapability_CapabilityKind
ON AiProviderCapability (CapabilityKind);

-- =========================================================
-- CommandDefinition
-- =========================================================

CREATE TABLE IF NOT EXISTS CommandDefinition (
    CommandDefinitionId  TEXT PRIMARY KEY,
    CommandName          TEXT NOT NULL UNIQUE,
    DisplayName          TEXT NOT NULL DEFAULT '',
    Description          TEXT NOT NULL DEFAULT '',
    IsEnabled            INTEGER NOT NULL DEFAULT 1,
    HandlerType          TEXT NOT NULL DEFAULT '',
    MetadataJson         TEXT NOT NULL DEFAULT '{}',
    CreatedUtc           TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc           TEXT NULL
);

CREATE INDEX IF NOT EXISTS IX_CommandDefinition_CommandName
ON CommandDefinition (CommandName);

CREATE INDEX IF NOT EXISTS IX_CommandDefinition_IsEnabled
ON CommandDefinition (IsEnabled);

-- =========================================================
-- CommandExecutionAudit
-- =========================================================

CREATE TABLE IF NOT EXISTS CommandExecutionAudit (
    CommandExecutionAuditId TEXT PRIMARY KEY,
    ExecutionId             TEXT NOT NULL,
    EventType               TEXT NOT NULL,
    EventUtc                TEXT NOT NULL,
    Message                 TEXT NOT NULL DEFAULT '',
    MetadataJson            TEXT NOT NULL DEFAULT '{}',

    FOREIGN KEY (ExecutionId)
        REFERENCES ExecutionHistory (ExecutionId)
        ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS IX_CommandExecutionAudit_ExecutionId
ON CommandExecutionAudit (ExecutionId);

CREATE INDEX IF NOT EXISTS IX_CommandExecutionAudit_EventType
ON CommandExecutionAudit (EventType);

-- =========================================================
-- Recommended Future SchemaVersion Table
-- =========================================================

CREATE TABLE IF NOT EXISTS SchemaVersion (
    SchemaVersionId INTEGER PRIMARY KEY,
    VersionNumber   INTEGER NOT NULL,
    AppliedUtc      TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description     TEXT NOT NULL DEFAULT ''
);

-- =========================================================
-- Recommended Seed
-- =========================================================

INSERT OR IGNORE INTO SchemaVersion (
    SchemaVersionId,
    VersionNumber,
    Description
)
VALUES (
    1,
    1,
    'Initial consolidated Codex.CommandEngine schema'
);