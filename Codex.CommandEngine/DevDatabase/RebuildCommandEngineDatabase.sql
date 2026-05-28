
PRAGMA foreign_keys = OFF;

DROP TABLE IF EXISTS CommandExecutionAudit;
DROP TABLE IF EXISTS WorkflowStepExecution;
DROP TABLE IF EXISTS WorkflowExecution;
DROP TABLE IF EXISTS WorkflowDefinitionStep;
DROP TABLE IF EXISTS WorkflowStep;
DROP TABLE IF EXISTS WorkflowDefinition;
DROP TABLE IF EXISTS ExecutionStepHistory;
DROP TABLE IF EXISTS ExecutionHistory;
DROP TABLE IF EXISTS CommandParameterDefinition;
DROP TABLE IF EXISTS CommandDefinition;
DROP TABLE IF EXISTS AiProviderCapability;
DROP TABLE IF EXISTS AiProvider;
DROP TABLE IF EXISTS ExecutionContext;
DROP TABLE IF EXISTS SqlQuery;
DROP TABLE IF EXISTS DatabaseSchemaSnapshot;
DROP TABLE IF EXISTS MigrationHistory;
DROP TABLE IF EXISTS Log;
DROP TABLE IF EXISTS SchemaVersion;

PRAGMA foreign_keys = ON;

CREATE TABLE [SchemaVersion] (
    [SchemaVersionId] INTEGER PRIMARY KEY,
    [VersionNumber]   INTEGER NOT NULL,
    [AppliedUtc]      TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    [Description]     TEXT NOT NULL DEFAULT ''
);

CREATE TABLE [MigrationHistory] (
    [MigrationId] TEXT PRIMARY KEY,
    [AppliedUtc]  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    [Description] TEXT NOT NULL DEFAULT ''
);

CREATE TABLE [DatabaseSchemaSnapshot] (
    [MigrationId]   TEXT PRIMARY KEY,
    [SchemaVersion] INTEGER NOT NULL,
    [SnapshotSql]   TEXT NOT NULL,
    [SnapshotHash]  TEXT NOT NULL,
    [CreatedUtc]    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    [Description]   TEXT NOT NULL DEFAULT ''
);

CREATE TABLE [SqlQuery] (
    [QueryName]   TEXT PRIMARY KEY,
    [Description] TEXT NOT NULL DEFAULT '',
    [SqlText]     TEXT NOT NULL DEFAULT '',
    [CreatedUtc]  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    [UpdatedUtc]  TEXT NULL,
    [Category]    TEXT NOT NULL DEFAULT '',
    [IsActive]    INTEGER NOT NULL DEFAULT 1,
    UNIQUE ([QueryName] ASC)
);

CREATE TABLE [Log] (
    [LogId]     INTEGER PRIMARY KEY AUTOINCREMENT,
    [Timestamp] TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    [Action]    TEXT NOT NULL,
    [Details]   TEXT NULL
);

CREATE TABLE [CommandDefinition] (
    [CommandId]            TEXT PRIMARY KEY,
    [CommandDefinitionId]  TEXT NOT NULL DEFAULT '',
    [CommandName]          TEXT NOT NULL UNIQUE,
    [DisplayName]          TEXT NOT NULL DEFAULT '',
    [Description]          TEXT NOT NULL DEFAULT '',
    [Category]             TEXT NOT NULL DEFAULT '',
    [Version]              INTEGER NOT NULL DEFAULT 1,
    [HandlerKey]           TEXT NOT NULL DEFAULT '',
    [HandlerType]          TEXT NOT NULL DEFAULT '',
    [InputJson]            TEXT NOT NULL DEFAULT '{}',
    [OutputJson]           TEXT NOT NULL DEFAULT '{}',
    [MetadataJson]         TEXT NOT NULL DEFAULT '{}',
    [IsEnabled]            INTEGER NOT NULL DEFAULT 1,
    [CreatedUtc]           TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    [UpdatedUtc]           TEXT NULL
);

CREATE TABLE [CommandParameterDefinition] (
    [CommandParameterDefinitionId] TEXT PRIMARY KEY,
    [CommandId]                    TEXT NOT NULL,
    [CommandDefinitionId]          TEXT NOT NULL DEFAULT '',
    [ParameterName]                TEXT NOT NULL,
    [ParameterType]                TEXT NOT NULL,
    [IsRequired]                   INTEGER NOT NULL DEFAULT 0,
    [DefaultValue]                 TEXT NOT NULL DEFAULT '',
    [Description]                  TEXT NOT NULL DEFAULT '',
    [SortOrder]                    INTEGER NOT NULL DEFAULT 0,
    [CreatedUtc]                   TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    [UpdatedUtc]                   TEXT NULL,
    FOREIGN KEY ([CommandId]) REFERENCES [CommandDefinition]([CommandId]) ON DELETE CASCADE,
    UNIQUE ([CommandId], [ParameterName])
);

CREATE TABLE [WorkflowDefinition] (
    [WorkflowId]           TEXT NOT NULL DEFAULT '',
    [WorkflowDefinitionId] TEXT NOT NULL DEFAULT '',
    [WorkflowName]         TEXT NOT NULL,
    [DisplayName]          TEXT NOT NULL DEFAULT '',
    [Description]          TEXT NOT NULL DEFAULT '',
    [Version]              INTEGER NOT NULL DEFAULT 1,
    [MetadataJson]         TEXT NOT NULL DEFAULT '{}',
    [IsEnabled]            INTEGER NOT NULL DEFAULT 1,
    [IsActive]             INTEGER NOT NULL DEFAULT 1,
    [CreatedUtc]           TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    [UpdatedUtc]           TEXT NULL,
    PRIMARY KEY ([WorkflowId]),
    UNIQUE ([WorkflowName]),
    UNIQUE ([WorkflowName], [Version])
);

CREATE TABLE [WorkflowStep] (
    [WorkflowStepId] TEXT PRIMARY KEY,
    [WorkflowId]     TEXT NOT NULL,
    [StepOrder]      INTEGER NOT NULL,
    [StepName]       TEXT NOT NULL DEFAULT '',
    [CommandId]      TEXT NOT NULL,
    [CommandName]    TEXT NOT NULL DEFAULT '',
    [InputMapJson]   TEXT NOT NULL DEFAULT '{}',
    [MetadataJson]   TEXT NOT NULL DEFAULT '{}',
    [IsEnabled]      INTEGER NOT NULL DEFAULT 1,
    FOREIGN KEY ([WorkflowId]) REFERENCES [WorkflowDefinition]([WorkflowId]) ON DELETE CASCADE,
    FOREIGN KEY ([CommandId]) REFERENCES [CommandDefinition]([CommandId]) ON DELETE RESTRICT,
    UNIQUE ([WorkflowId], [StepOrder])
);

CREATE TABLE [WorkflowDefinitionStep] (
    [WorkflowDefinitionStepId] TEXT NOT NULL DEFAULT '',
    [WorkflowDefinitionId]     TEXT NOT NULL,
    [StepName]                 TEXT NOT NULL,
    [StepOrder]                INTEGER NOT NULL,
    [CommandName]              TEXT NOT NULL,
    [ParametersJson]           TEXT NOT NULL DEFAULT '{}',
    [IsEnabled]                INTEGER NOT NULL DEFAULT 1,
    [CommandDefinitionId]      TEXT NOT NULL DEFAULT '',
    [InputMapJson]             TEXT NOT NULL DEFAULT '{}',
    [CreatedUtc]               TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    [UpdatedUtc]               TEXT NULL,
    PRIMARY KEY ([WorkflowDefinitionStepId]),
    UNIQUE ([WorkflowDefinitionId], [StepOrder])
);

CREATE TABLE [ExecutionHistory] (
    [ExecutionId]          TEXT PRIMARY KEY,
    [ExecutionKind]        TEXT NOT NULL DEFAULT 'Command',
    [TargetId]             TEXT NOT NULL DEFAULT '',
    [TargetName]           TEXT NOT NULL DEFAULT '',
    [CommandName]          TEXT NOT NULL DEFAULT '',
    [CorrelationId]        TEXT NOT NULL DEFAULT '',
    [Status]               TEXT NOT NULL,
    [StartedUtc]           TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    [CompletedUtc]         TEXT NULL,
    [DurationMilliseconds] INTEGER NULL,
    [DurationMs]           INTEGER NULL,
    [RequestJson]          TEXT NOT NULL DEFAULT '{}',
    [InputJson]            TEXT NOT NULL DEFAULT '{}',
    [OutputJson]           TEXT NOT NULL DEFAULT '{}',
    [Message]              TEXT NOT NULL DEFAULT '',
    [ErrorMessage]         TEXT NOT NULL DEFAULT '',
    [ExceptionText]        TEXT NULL,
    [MetadataJson]         TEXT NOT NULL DEFAULT '{}'
);

CREATE TABLE [ExecutionStepHistory] (
    [ExecutionStepHistoryId]   TEXT PRIMARY KEY,
    [ExecutionStepId]          TEXT NOT NULL DEFAULT '',
    [ExecutionId]              TEXT NOT NULL DEFAULT '',
    [WorkflowStepId]           TEXT NULL,
    [WorkflowStepDefinitionId] TEXT NULL,
    [CommandId]                TEXT NULL,
    [CommandDefinitionId]      TEXT NULL,
    [StepOrder]                INTEGER NOT NULL DEFAULT 0,
    [StepName]                 TEXT NOT NULL DEFAULT '',
    [Status]                   TEXT NOT NULL DEFAULT '',
    [StartedUtc]               TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    [CompletedUtc]             TEXT NULL,
    [DurationMilliseconds]     INTEGER NULL,
    [DurationMs]               INTEGER NULL,
    [InputJson]                TEXT NOT NULL DEFAULT '{}',
    [OutputJson]               TEXT NOT NULL DEFAULT '{}',
    [Message]                  TEXT NOT NULL DEFAULT '',
    [ErrorMessage]             TEXT NOT NULL DEFAULT '',
    [MetadataJson]             TEXT NOT NULL DEFAULT '{}',
    FOREIGN KEY ([ExecutionId]) REFERENCES [ExecutionHistory]([ExecutionId]) ON DELETE CASCADE
);

CREATE TABLE [ExecutionContext] (
    [ContextId]    TEXT PRIMARY KEY,
    [ContextName]  TEXT NOT NULL UNIQUE,
    [Scope]        TEXT NOT NULL DEFAULT '',
    [Description]  TEXT NOT NULL DEFAULT '',
    [ContextJson]  TEXT NOT NULL DEFAULT '{}',
    [MetadataJson] TEXT NOT NULL DEFAULT '{}',
    [IsEnabled]    INTEGER NOT NULL DEFAULT 1,
    [CreatedUtc]   TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    [UpdatedUtc]   TEXT NULL
);

CREATE TABLE [AiProvider] (
    [AiProviderId]         TEXT PRIMARY KEY,
    [ProviderName]         TEXT NOT NULL UNIQUE,
    [DisplayName]          TEXT NOT NULL DEFAULT '',
    [ProviderKind]         TEXT NOT NULL DEFAULT '',
    [Description]          TEXT NOT NULL DEFAULT '',
    [ConfigurationJson]    TEXT NOT NULL DEFAULT '{}',
    [BaseUrl]              TEXT NOT NULL DEFAULT '',
    [ApiKeyEnvironmentVar] TEXT NOT NULL DEFAULT '',
    [MetadataJson]         TEXT NOT NULL DEFAULT '{}',
    [IsEnabled]            INTEGER NOT NULL DEFAULT 1,
    [CreatedUtc]           TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    [UpdatedUtc]           TEXT NULL
);

CREATE TABLE [AiProviderCapability] (
    [AiProviderCapabilityId] TEXT PRIMARY KEY,
    [AiProviderId]           TEXT NOT NULL,
    [CapabilityName]         TEXT NOT NULL,
    [CapabilityKind]         TEXT NOT NULL DEFAULT '',
    [CapabilityValue]        TEXT NOT NULL DEFAULT '',
    [Description]            TEXT NOT NULL DEFAULT '',
    [MetadataJson]           TEXT NOT NULL DEFAULT '{}',
    [IsEnabled]              INTEGER NOT NULL DEFAULT 1,
    [CreatedUtc]             TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    [UpdatedUtc]             TEXT NULL,
    FOREIGN KEY ([AiProviderId]) REFERENCES [AiProvider]([AiProviderId]) ON DELETE CASCADE,
    UNIQUE ([AiProviderId], [CapabilityName])
);

CREATE TABLE [WorkflowExecution] (
    [WorkflowExecutionId]     TEXT PRIMARY KEY,
    [WorkflowName]            TEXT NOT NULL,
    [CorrelationId]           TEXT NOT NULL DEFAULT '',
    [Status]                  TEXT NOT NULL,
    [StartedUtc]              TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    [CompletedUtc]            TEXT NULL,
    [CurrentStepOrder]        INTEGER NOT NULL DEFAULT 0,
    [LastCompletedStepOrder]  INTEGER NOT NULL DEFAULT 0,
    [IsResumable]             INTEGER NOT NULL DEFAULT 1,
    [ResumeToken]             TEXT NOT NULL DEFAULT '',
    [LastHeartbeatUtc]        TEXT NULL,
    [RuntimeStateJson]        TEXT NOT NULL DEFAULT '{}',
    [ContextJson]             TEXT NOT NULL DEFAULT '{}',
    [Message]                 TEXT NOT NULL DEFAULT '',
    [ErrorMessage]            TEXT NULL,
    [MetadataJson]            TEXT NOT NULL DEFAULT '{}'
);

CREATE TABLE [WorkflowStepExecution] (
    [WorkflowStepExecutionId] TEXT PRIMARY KEY,
    [WorkflowExecutionId]     TEXT NOT NULL,
    [StepName]                TEXT NOT NULL,
    [StepOrder]               INTEGER NOT NULL,
    [CommandName]             TEXT NOT NULL,
    [Status]                  TEXT NOT NULL,
    [StartedUtc]              TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    [CompletedUtc]            TEXT NULL,
    [Message]                 TEXT NOT NULL DEFAULT '',
    [CommandExecutionId]      TEXT NULL,
    FOREIGN KEY ([WorkflowExecutionId]) REFERENCES [WorkflowExecution]([WorkflowExecutionId]) ON DELETE CASCADE
);

CREATE TABLE [CommandExecutionAudit] (
    [CommandExecutionAuditId] TEXT PRIMARY KEY,
    [ExecutionId]             TEXT NOT NULL,
    [EventType]               TEXT NOT NULL,
    [EventUtc]                TEXT NOT NULL,
    [Message]                 TEXT NOT NULL DEFAULT '',
    [MetadataJson]            TEXT NOT NULL DEFAULT '{}',
    FOREIGN KEY ([ExecutionId]) REFERENCES [ExecutionHistory]([ExecutionId]) ON DELETE CASCADE
);

CREATE INDEX [IX_CommandDefinition_CommandName] ON [CommandDefinition] ([CommandName]);
CREATE INDEX [IX_CommandDefinition_IsEnabled] ON [CommandDefinition] ([IsEnabled]);
CREATE INDEX [IX_CommandParameterDefinition_CommandId] ON [CommandParameterDefinition] ([CommandId]);
CREATE INDEX [IX_WorkflowDefinition_WorkflowName] ON [WorkflowDefinition] ([WorkflowName]);
CREATE INDEX [IX_WorkflowDefinition_IsActive] ON [WorkflowDefinition] ([IsActive]);
CREATE INDEX [IX_WorkflowStep_WorkflowId] ON [WorkflowStep] ([WorkflowId]);
CREATE INDEX [IX_WorkflowDefinitionStep_WorkflowDefinitionId] ON [WorkflowDefinitionStep] ([WorkflowDefinitionId]);
CREATE INDEX [IX_WorkflowDefinitionStep_CommandName] ON [WorkflowDefinitionStep] ([CommandName]);
CREATE INDEX [IX_ExecutionHistory_Status_StartedUtc] ON [ExecutionHistory] ([Status], [StartedUtc]);
CREATE INDEX [IX_ExecutionHistory_CorrelationId] ON [ExecutionHistory] ([CorrelationId]);
CREATE INDEX [IX_ExecutionHistory_CommandName] ON [ExecutionHistory] ([CommandName]);
CREATE INDEX [IX_ExecutionStepHistory_ExecutionId_StepOrder] ON [ExecutionStepHistory] ([ExecutionId], [StepOrder]);
CREATE INDEX [IX_ExecutionContext_ContextName] ON [ExecutionContext] ([ContextName]);
CREATE INDEX [IX_AiProvider_ProviderName] ON [AiProvider] ([ProviderName]);
CREATE INDEX [IX_AiProviderCapability_AiProviderId] ON [AiProviderCapability] ([AiProviderId]);
CREATE INDEX [IX_WorkflowExecution_Status] ON [WorkflowExecution] ([Status]);
CREATE INDEX [IX_WorkflowExecution_CorrelationId] ON [WorkflowExecution] ([CorrelationId]);
CREATE INDEX [IX_WorkflowExecution_IsResumable] ON [WorkflowExecution] ([IsResumable]);
CREATE INDEX [IX_WorkflowStepExecution_WorkflowExecutionId] ON [WorkflowStepExecution] ([WorkflowExecutionId]);
CREATE INDEX [IX_CommandExecutionAudit_ExecutionId] ON [CommandExecutionAudit] ([ExecutionId]);

INSERT INTO [SchemaVersion] ([SchemaVersionId], [VersionNumber], [Description])
VALUES (1, 1, 'Rebuilt Codex.CommandEngine canonical development schema');

INSERT INTO [MigrationHistory] ([MigrationId], [Description]) VALUES
('0001_engine_database_foundation', 'Initial command engine database foundation.'),
('0002_sql_catalog_foundation', 'Adds SQL catalog update tracking and catalog service support.'),
('0003_command_registration_foundation', 'Adds command registration metadata and command parameter definitions.'),
('0004_workflow_model_foundation', 'Adds workflow model metadata, workflow step metadata, and workflow SQL catalog entries.'),
('0005_execution_history_foundation', 'Adds execution history repository support, correlation tracking, and execution SQL catalog entries.'),
('0006_context_system_foundation', 'Adds execution context repository support and SQL catalog entries.'),
('0007_ai_provider_foundation', 'Adds AI provider repository support and SQL catalog entries.'),
('0008_rebuilt_canonical_dev_schema', 'Rebuilt canonical development schema from current repository expectations.');


-- =========================================================
-- Required SQL catalog seed data
-- =========================================================

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('SqlQuery_SelectByName', 'Selects one SQL catalog query by name.', 'SELECT QueryName, Description, SqlText, CreatedUtc, UpdatedUtc, Category, IsActive FROM SqlQuery WHERE QueryName = $QueryName;', 'SQL Catalog', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('SqlQuery_SelectAll', 'Lists all SQL catalog queries.', 'SELECT QueryName, Description, SqlText, CreatedUtc, UpdatedUtc, Category, IsActive FROM SqlQuery ORDER BY Category ASC, QueryName ASC;', 'SQL Catalog', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('SqlQuery_InsertOrReplace', 'Inserts or updates a SQL catalog query.', 'INSERT INTO SqlQuery (QueryName, Description, SqlText, Category, IsActive, UpdatedUtc)
VALUES ($QueryName, $Description, $SqlText, $Category, $IsActive, CURRENT_TIMESTAMP)
ON CONFLICT(QueryName) DO UPDATE SET
    Description = excluded.Description,
    SqlText = excluded.SqlText,
    Category = excluded.Category,
    IsActive = excluded.IsActive,
    UpdatedUtc = CURRENT_TIMESTAMP;', 'SQL Catalog', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('CommandDefinition_InsertOrReplace', 'Inserts or updates a command definition.', 'INSERT INTO CommandDefinition (CommandId, CommandDefinitionId, CommandName, DisplayName, Description, Category, Version, IsEnabled, HandlerType, HandlerKey, InputJson, OutputJson, MetadataJson)
VALUES ($CommandId, $CommandDefinitionId, $CommandName, $DisplayName, $Description, $Category, $Version, $IsEnabled, $HandlerType, $HandlerKey, $InputJson, $OutputJson, $MetadataJson)
ON CONFLICT(CommandName) DO UPDATE SET
    CommandDefinitionId = excluded.CommandDefinitionId,
    DisplayName = excluded.DisplayName,
    Description = excluded.Description,
    Category = excluded.Category,
    Version = excluded.Version,
    IsEnabled = excluded.IsEnabled,
    HandlerType = excluded.HandlerType,
    HandlerKey = excluded.HandlerKey,
    InputJson = excluded.InputJson,
    OutputJson = excluded.OutputJson,
    MetadataJson = excluded.MetadataJson,
    UpdatedUtc = CURRENT_TIMESTAMP;', 'Command Registration', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('CommandDefinition_SelectByName', 'Selects a command definition by name.', 'SELECT CommandId, CommandName, DisplayName, Description, Category, Version, IsEnabled, HandlerType, CreatedUtc, UpdatedUtc FROM CommandDefinition WHERE CommandName = $CommandName;', 'Command Registration', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('CommandDefinition_SelectAll', 'Lists command definitions.', 'SELECT CommandId, CommandName, DisplayName, Description, Category, Version, IsEnabled, HandlerType, CreatedUtc, UpdatedUtc FROM CommandDefinition ORDER BY Category ASC, CommandName ASC;', 'Command Registration', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('CommandParameterDefinition_InsertOrReplace', 'Inserts or updates a command parameter definition.', 'INSERT INTO CommandParameterDefinition (CommandParameterDefinitionId, CommandId, CommandDefinitionId, ParameterName, ParameterType, IsRequired, DefaultValue, Description, SortOrder)
VALUES ($CommandParameterDefinitionId, $CommandId, $CommandDefinitionId, $ParameterName, $ParameterType, $IsRequired, $DefaultValue, $Description, $SortOrder)
ON CONFLICT(CommandId, ParameterName) DO UPDATE SET
    CommandDefinitionId = excluded.CommandDefinitionId,
    ParameterType = excluded.ParameterType,
    IsRequired = excluded.IsRequired,
    DefaultValue = excluded.DefaultValue,
    Description = excluded.Description,
    SortOrder = excluded.SortOrder,
    UpdatedUtc = CURRENT_TIMESTAMP;', 'Command Registration', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('CommandParameterDefinition_SelectByCommand', 'Lists parameter definitions for a command.', 'SELECT CommandParameterDefinitionId, CommandId, ParameterName, ParameterType, IsRequired, DefaultValue, Description, SortOrder, CreatedUtc, UpdatedUtc FROM CommandParameterDefinition WHERE CommandId = $CommandId ORDER BY SortOrder ASC, ParameterName ASC;', 'Command Registration', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('WorkflowDefinition_InsertOrReplace', 'Inserts or updates a workflow definition.', 'INSERT INTO WorkflowDefinition (WorkflowId, WorkflowDefinitionId, WorkflowName, DisplayName, Description, Version, MetadataJson, IsEnabled, IsActive)
VALUES ($WorkflowId, $WorkflowId, $WorkflowName, $DisplayName, $Description, $Version, $MetadataJson, $IsEnabled, $IsEnabled)
ON CONFLICT(WorkflowName, Version) DO UPDATE SET
    WorkflowDefinitionId = excluded.WorkflowDefinitionId,
    DisplayName = excluded.DisplayName,
    Description = excluded.Description,
    MetadataJson = excluded.MetadataJson,
    IsEnabled = excluded.IsEnabled,
    IsActive = excluded.IsActive,
    UpdatedUtc = CURRENT_TIMESTAMP;', 'Workflow Foundation', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('WorkflowDefinition_SelectAll', 'Lists workflow definitions.', 'SELECT WorkflowId, WorkflowDefinitionId, WorkflowName, DisplayName, Description, Version, IsEnabled, IsActive, MetadataJson, CreatedUtc, UpdatedUtc FROM WorkflowDefinition ORDER BY WorkflowName ASC, Version DESC;', 'Workflow Foundation', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('WorkflowDefinition_SelectByNameAndVersion', 'Selects workflow definition by name and version.', 'SELECT WorkflowId, WorkflowDefinitionId, WorkflowName, DisplayName, Description, Version, IsEnabled, IsActive, MetadataJson, CreatedUtc, UpdatedUtc FROM WorkflowDefinition WHERE WorkflowName = $WorkflowName AND Version = $Version;', 'Workflow Foundation', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('WorkflowStep_InsertOrReplace', 'Inserts or updates a workflow step.', 'INSERT INTO WorkflowStep (WorkflowStepId, WorkflowId, StepOrder, StepName, CommandId, CommandName, InputMapJson, MetadataJson, IsEnabled)
VALUES ($WorkflowStepId, $WorkflowId, $StepOrder, $StepName, $CommandId, $CommandId, $InputMapJson, $MetadataJson, $IsEnabled)
ON CONFLICT(WorkflowId, StepOrder) DO UPDATE SET
    StepName = excluded.StepName,
    CommandId = excluded.CommandId,
    CommandName = excluded.CommandName,
    InputMapJson = excluded.InputMapJson,
    MetadataJson = excluded.MetadataJson,
    IsEnabled = excluded.IsEnabled;', 'Workflow Foundation', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('WorkflowStep_SelectByWorkflow', 'Lists workflow steps for a workflow.', 'SELECT WorkflowStepId, WorkflowId, StepOrder, StepName, CommandId, InputMapJson, MetadataJson, IsEnabled FROM WorkflowStep WHERE WorkflowId = $WorkflowId ORDER BY StepOrder ASC;', 'Workflow Foundation', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('ExecutionHistory_Insert', 'Inserts an execution history row.', 'INSERT INTO ExecutionHistory (ExecutionId, ExecutionKind, TargetId, TargetName, CommandName, CorrelationId, Status, StartedUtc, RequestJson, InputJson, MetadataJson)
VALUES ($ExecutionId, $ExecutionKind, $TargetId, $TargetName, $TargetName, $CorrelationId, $Status, $StartedUtc, $InputJson, $InputJson, $MetadataJson);', 'Execution History', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('ExecutionHistory_Complete', 'Completes an execution history row.', 'UPDATE ExecutionHistory SET Status = $Status, CompletedUtc = $CompletedUtc, OutputJson = $OutputJson, ErrorMessage = $ErrorMessage, MetadataJson = $MetadataJson WHERE ExecutionId = $ExecutionId;', 'Execution History', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('ExecutionHistory_SelectById', 'Selects execution history by id.', 'SELECT ExecutionId, ExecutionKind, TargetId, TargetName, CommandName, CorrelationId, Status, StartedUtc, CompletedUtc, RequestJson, InputJson, OutputJson, Message, ErrorMessage, ExceptionText, MetadataJson FROM ExecutionHistory WHERE ExecutionId = $ExecutionId;', 'Execution History', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('ExecutionHistory_SelectRecent', 'Lists recent execution history.', 'SELECT ExecutionId, ExecutionKind, TargetId, TargetName, CommandName, CorrelationId, Status, StartedUtc, CompletedUtc, RequestJson, InputJson, OutputJson, Message, ErrorMessage, ExceptionText, MetadataJson FROM ExecutionHistory ORDER BY StartedUtc DESC LIMIT $Limit;', 'Execution History', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('ExecutionStepHistory_Insert', 'Inserts an execution step history row.', 'INSERT INTO ExecutionStepHistory (ExecutionStepHistoryId, ExecutionStepId, ExecutionId, WorkflowStepId, CommandId, StepOrder, StepName, Status, StartedUtc, InputJson, MetadataJson)
VALUES ($ExecutionStepId, $ExecutionStepId, $ExecutionId, $WorkflowStepId, $CommandId, $StepOrder, $StepName, $Status, $StartedUtc, $InputJson, $MetadataJson);', 'Execution History', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('ExecutionStepHistory_Complete', 'Completes an execution step history row.', 'UPDATE ExecutionStepHistory SET Status = $Status, CompletedUtc = $CompletedUtc, OutputJson = $OutputJson, ErrorMessage = $ErrorMessage, MetadataJson = $MetadataJson WHERE ExecutionStepId = $ExecutionStepId OR ExecutionStepHistoryId = $ExecutionStepId;', 'Execution History', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('ExecutionStepHistory_SelectByExecution', 'Lists execution steps for one execution.', 'SELECT ExecutionStepHistoryId, ExecutionStepId, ExecutionId, WorkflowStepId, WorkflowStepDefinitionId, CommandId, CommandDefinitionId, StepOrder, StepName, Status, StartedUtc, CompletedUtc, InputJson, OutputJson, Message, ErrorMessage, MetadataJson FROM ExecutionStepHistory WHERE ExecutionId = $ExecutionId ORDER BY StepOrder ASC;', 'Execution History', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('EngineContext_InsertOrReplace', 'Inserts or updates execution context.', 'INSERT INTO ExecutionContext (ContextId, ContextName, Scope, Description, ContextJson, MetadataJson, IsEnabled)
VALUES ($ContextId, $ContextName, $Scope, $Description, $ContextJson, $MetadataJson, $IsEnabled)
ON CONFLICT(ContextName) DO UPDATE SET
    Scope = excluded.Scope,
    Description = excluded.Description,
    ContextJson = excluded.ContextJson,
    MetadataJson = excluded.MetadataJson,
    IsEnabled = excluded.IsEnabled,
    UpdatedUtc = CURRENT_TIMESTAMP;', 'Execution Context Foundation', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('EngineContext_SelectByName', 'Selects context by name.', 'SELECT ContextId, ContextName, Scope, Description, ContextJson, MetadataJson, IsEnabled, CreatedUtc, UpdatedUtc FROM ExecutionContext WHERE ContextName = $ContextName;', 'Execution Context Foundation', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('EngineContext_SelectAll', 'Lists contexts.', 'SELECT ContextId, ContextName, Scope, Description, ContextJson, MetadataJson, IsEnabled, CreatedUtc, UpdatedUtc FROM ExecutionContext ORDER BY Scope ASC, ContextName ASC;', 'Execution Context Foundation', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('EngineContext_SelectEnabled', 'Lists enabled contexts.', 'SELECT ContextId, ContextName, Scope, Description, ContextJson, MetadataJson, IsEnabled, CreatedUtc, UpdatedUtc FROM ExecutionContext WHERE IsEnabled = 1 ORDER BY Scope ASC, ContextName ASC;', 'Execution Context Foundation', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('EngineContext_Deactivate', 'Disables a context.', 'UPDATE ExecutionContext SET IsEnabled = 0, UpdatedUtc = CURRENT_TIMESTAMP WHERE ContextId = $ContextId;', 'Execution Context Foundation', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('AiProvider_InsertOrReplace', 'Inserts or updates AI provider.', 'INSERT INTO AiProvider (AiProviderId, ProviderName, DisplayName, ProviderKind, Description, ConfigurationJson, MetadataJson, IsEnabled)
VALUES ($AiProviderId, $ProviderName, $DisplayName, $ProviderKind, $Description, $ConfigurationJson, $MetadataJson, $IsEnabled)
ON CONFLICT(ProviderName) DO UPDATE SET
    DisplayName = excluded.DisplayName,
    ProviderKind = excluded.ProviderKind,
    Description = excluded.Description,
    ConfigurationJson = excluded.ConfigurationJson,
    MetadataJson = excluded.MetadataJson,
    IsEnabled = excluded.IsEnabled,
    UpdatedUtc = CURRENT_TIMESTAMP;', 'AI Provider Foundation', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('AiProvider_SelectByName', 'Selects AI provider by name.', 'SELECT AiProviderId, ProviderName, DisplayName, ProviderKind, Description, ConfigurationJson, MetadataJson, IsEnabled, CreatedUtc, UpdatedUtc FROM AiProvider WHERE ProviderName = $ProviderName;', 'AI Provider Foundation', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('AiProvider_SelectAll', 'Lists AI providers.', 'SELECT AiProviderId, ProviderName, DisplayName, ProviderKind, Description, ConfigurationJson, MetadataJson, IsEnabled, CreatedUtc, UpdatedUtc FROM AiProvider ORDER BY ProviderName ASC;', 'AI Provider Foundation', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('AiProviderCapability_InsertOrReplace', 'Inserts or updates AI provider capability.', 'INSERT INTO AiProviderCapability (AiProviderCapabilityId, AiProviderId, CapabilityName, CapabilityKind, Description, MetadataJson, IsEnabled)
VALUES ($AiProviderCapabilityId, $AiProviderId, $CapabilityName, $CapabilityKind, $Description, $MetadataJson, $IsEnabled)
ON CONFLICT(AiProviderId, CapabilityName) DO UPDATE SET
    CapabilityKind = excluded.CapabilityKind,
    Description = excluded.Description,
    MetadataJson = excluded.MetadataJson,
    IsEnabled = excluded.IsEnabled,
    UpdatedUtc = CURRENT_TIMESTAMP;', 'AI Provider Foundation', 1);

INSERT INTO [SqlQuery] ([QueryName], [Description], [SqlText], [Category], [IsActive])
VALUES ('AiProviderCapability_SelectByProvider', 'Lists AI provider capabilities.', 'SELECT AiProviderCapabilityId, AiProviderId, CapabilityName, CapabilityKind, Description, MetadataJson, IsEnabled, CreatedUtc, UpdatedUtc FROM AiProviderCapability WHERE AiProviderId = $AiProviderId ORDER BY CapabilityName ASC;', 'AI Provider Foundation', 1);

INSERT INTO [DatabaseSchemaSnapshot] ([MigrationId], [SchemaVersion], [SnapshotSql], [SnapshotHash], [Description])
VALUES ('0008_rebuilt_canonical_dev_schema', 8, 'Canonical rebuilt schema stored in DevDatabase/RebuildCommandEngineDatabase.sql', 'manual', 'Rebuilt canonical development database.');

PRAGMA foreign_keys = ON;
