--
-- File generated with SQLiteStudio v3.4.15 on Sat May 23 14:39:50 2026
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: AiProvider
DROP TABLE IF EXISTS AiProvider;

CREATE TABLE IF NOT EXISTS AiProvider (
    AiProviderId         TEXT    PRIMARY KEY,
    ProviderName         TEXT    NOT NULL
                                 UNIQUE,
    DisplayName          TEXT    NOT NULL
                                 DEFAULT '',
    ProviderKind         TEXT    NOT NULL
                                 DEFAULT '',
    Description          TEXT    NOT NULL
                                 DEFAULT '',
    ConfigurationJson    TEXT    NOT NULL
                                 DEFAULT '{}',
    BaseUrl              TEXT    NOT NULL
                                 DEFAULT '',
    ApiKeyEnvironmentVar TEXT    NOT NULL
                                 DEFAULT '',
    MetadataJson         TEXT    NOT NULL
                                 DEFAULT '{}',
    IsEnabled            INTEGER NOT NULL
                                 DEFAULT 1,
    CreatedUtc           TEXT    NOT NULL
                                 DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc           TEXT    NULL
);


-- Table: AiProviderCapability
DROP TABLE IF EXISTS AiProviderCapability;

CREATE TABLE IF NOT EXISTS AiProviderCapability (
    AiProviderCapabilityId TEXT    PRIMARY KEY,
    AiProviderId           TEXT    NOT NULL,
    CapabilityName         TEXT    NOT NULL,
    CapabilityKind         TEXT    NOT NULL
                                   DEFAULT '',
    CapabilityValue        TEXT    NOT NULL
                                   DEFAULT '',
    Description            TEXT    NOT NULL
                                   DEFAULT '',
    MetadataJson           TEXT    NOT NULL
                                   DEFAULT '{}',
    IsEnabled              INTEGER NOT NULL
                                   DEFAULT 1,
    CreatedUtc             TEXT    NOT NULL
                                   DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc             TEXT    NULL,
    FOREIGN KEY (
        AiProviderId
    )
    REFERENCES AiProvider (AiProviderId) ON DELETE CASCADE,
    UNIQUE (
        AiProviderId,
        CapabilityName
    )
);


-- Table: CommandDefinition
DROP TABLE IF EXISTS CommandDefinition;

CREATE TABLE IF NOT EXISTS CommandDefinition (
    CommandId           TEXT    PRIMARY KEY,
    CommandDefinitionId TEXT    NOT NULL
                                DEFAULT '',
    CommandName         TEXT    NOT NULL
                                UNIQUE,
    DisplayName         TEXT    NOT NULL
                                DEFAULT '',
    Description         TEXT    NOT NULL
                                DEFAULT '',
    Category            TEXT    NOT NULL
                                DEFAULT '',
    Version             INTEGER NOT NULL
                                DEFAULT 1,
    HandlerKey          TEXT    NOT NULL
                                DEFAULT '',
    HandlerType         TEXT    NOT NULL
                                DEFAULT '',
    InputJson           TEXT    NOT NULL
                                DEFAULT '{}',
    OutputJson          TEXT    NOT NULL
                                DEFAULT '{}',
    MetadataJson        TEXT    NOT NULL
                                DEFAULT '{}',
    IsEnabled           INTEGER NOT NULL
                                DEFAULT 1,
    CreatedUtc          TEXT    NOT NULL
                                DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc          TEXT    NULL
);


-- Table: CommandExecutionAudit
DROP TABLE IF EXISTS CommandExecutionAudit;

CREATE TABLE IF NOT EXISTS CommandExecutionAudit (
    CommandExecutionAuditId TEXT PRIMARY KEY,
    ExecutionId             TEXT NOT NULL,
    EventType               TEXT NOT NULL,
    EventUtc                TEXT NOT NULL,
    Message                 TEXT NOT NULL
                                 DEFAULT '',
    MetadataJson            TEXT NOT NULL
                                 DEFAULT '{}',
    FOREIGN KEY (
        ExecutionId
    )
    REFERENCES ExecutionHistory (ExecutionId) ON DELETE CASCADE
);


-- Table: CommandParameterDefinition
DROP TABLE IF EXISTS CommandParameterDefinition;

CREATE TABLE IF NOT EXISTS CommandParameterDefinition (
    CommandParameterDefinitionId TEXT    PRIMARY KEY,
    CommandId                    TEXT    NOT NULL,
    CommandDefinitionId          TEXT    NOT NULL
                                         DEFAULT '',
    ParameterName                TEXT    NOT NULL,
    ParameterType                TEXT    NOT NULL,
    IsRequired                   INTEGER NOT NULL
                                         DEFAULT 0,
    DefaultValue                 TEXT    NOT NULL
                                         DEFAULT '',
    Description                  TEXT    NOT NULL
                                         DEFAULT '',
    SortOrder                    INTEGER NOT NULL
                                         DEFAULT 0,
    CreatedUtc                   TEXT    NOT NULL
                                         DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc                   TEXT    NULL,
    FOREIGN KEY (
        CommandId
    )
    REFERENCES CommandDefinition (CommandId) ON DELETE CASCADE,
    UNIQUE (
        CommandId,
        ParameterName
    )
);


-- Table: DatabaseSchemaSnapshot
DROP TABLE IF EXISTS DatabaseSchemaSnapshot;

CREATE TABLE IF NOT EXISTS DatabaseSchemaSnapshot (
    MigrationId   TEXT    PRIMARY KEY,
    SchemaVersion INTEGER NOT NULL,
    SnapshotSql   TEXT    NOT NULL,
    SnapshotHash  TEXT    NOT NULL,
    CreatedUtc    TEXT    NOT NULL
                          DEFAULT CURRENT_TIMESTAMP,
    Description   TEXT    NOT NULL
                          DEFAULT ''
);

INSERT INTO DatabaseSchemaSnapshot (
                                       MigrationId,
                                       SchemaVersion,
                                       SnapshotSql,
                                       SnapshotHash,
                                       CreatedUtc,
                                       Description
                                   )
                                   VALUES (
                                       '0008_rebuilt_canonical_dev_schema',
                                       8,
                                       'Canonical rebuilt schema stored in DevDatabase/RebuildCommandEngineDatabase.sql',
                                       'manual',
                                       '2026-05-23 21:13:49',
                                       'Rebuilt canonical development database.'
                                   );


-- Table: ExecutionContext
DROP TABLE IF EXISTS ExecutionContext;

CREATE TABLE IF NOT EXISTS ExecutionContext (
    ContextId    TEXT    PRIMARY KEY,
    ContextName  TEXT    NOT NULL
                         UNIQUE,
    Scope        TEXT    NOT NULL
                         DEFAULT '',
    Description  TEXT    NOT NULL
                         DEFAULT '',
    ContextJson  TEXT    NOT NULL
                         DEFAULT '{}',
    MetadataJson TEXT    NOT NULL
                         DEFAULT '{}',
    IsEnabled    INTEGER NOT NULL
                         DEFAULT 1,
    CreatedUtc   TEXT    NOT NULL
                         DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc   TEXT    NULL
);


-- Table: ExecutionHistory
DROP TABLE IF EXISTS ExecutionHistory;

CREATE TABLE IF NOT EXISTS ExecutionHistory (
    ExecutionId          TEXT    PRIMARY KEY,
    ExecutionKind        TEXT    NOT NULL
                                 DEFAULT 'Command',
    TargetId             TEXT    NOT NULL
                                 DEFAULT '',
    TargetName           TEXT    NOT NULL
                                 DEFAULT '',
    CommandName          TEXT    NOT NULL
                                 DEFAULT '',
    CorrelationId        TEXT    NOT NULL
                                 DEFAULT '',
    Status               TEXT    NOT NULL,
    StartedUtc           TEXT    NOT NULL
                                 DEFAULT CURRENT_TIMESTAMP,
    CompletedUtc         TEXT    NULL,
    DurationMilliseconds INTEGER NULL,
    DurationMs           INTEGER NULL,
    RequestJson          TEXT    NOT NULL
                                 DEFAULT '{}',
    InputJson            TEXT    NOT NULL
                                 DEFAULT '{}',
    OutputJson           TEXT    NOT NULL
                                 DEFAULT '{}',
    Message              TEXT    NOT NULL
                                 DEFAULT '',
    ErrorMessage         TEXT    NOT NULL
                                 DEFAULT '',
    ExceptionText        TEXT    NULL,
    MetadataJson         TEXT    NOT NULL
                                 DEFAULT '{}'
);


-- Table: ExecutionStepHistory
DROP TABLE IF EXISTS ExecutionStepHistory;

CREATE TABLE IF NOT EXISTS ExecutionStepHistory (
    ExecutionStepHistoryId   TEXT    PRIMARY KEY,
    ExecutionStepId          TEXT    NOT NULL
                                     DEFAULT '',
    ExecutionId              TEXT    NOT NULL
                                     DEFAULT '',
    WorkflowStepId           TEXT    NULL,
    WorkflowStepDefinitionId TEXT    NULL,
    CommandId                TEXT    NULL,
    CommandDefinitionId      TEXT    NULL,
    StepOrder                INTEGER NOT NULL
                                     DEFAULT 0,
    StepName                 TEXT    NOT NULL
                                     DEFAULT '',
    Status                   TEXT    NOT NULL
                                     DEFAULT '',
    StartedUtc               TEXT    NOT NULL
                                     DEFAULT CURRENT_TIMESTAMP,
    CompletedUtc             TEXT    NULL,
    DurationMilliseconds     INTEGER NULL,
    DurationMs               INTEGER NULL,
    InputJson                TEXT    NOT NULL
                                     DEFAULT '{}',
    OutputJson               TEXT    NOT NULL
                                     DEFAULT '{}',
    Message                  TEXT    NOT NULL
                                     DEFAULT '',
    ErrorMessage             TEXT    NOT NULL
                                     DEFAULT '',
    MetadataJson             TEXT    NOT NULL
                                     DEFAULT '{}',
    FOREIGN KEY (
        ExecutionId
    )
    REFERENCES ExecutionHistory (ExecutionId) ON DELETE CASCADE
);


-- Table: Log
DROP TABLE IF EXISTS Log;

CREATE TABLE IF NOT EXISTS Log (
    LogId     INTEGER PRIMARY KEY AUTOINCREMENT,
    Timestamp TEXT    NOT NULL
                      DEFAULT CURRENT_TIMESTAMP,
    Action    TEXT    NOT NULL,
    Details   TEXT    NULL
);


-- Table: MigrationHistory
DROP TABLE IF EXISTS MigrationHistory;

CREATE TABLE IF NOT EXISTS MigrationHistory (
    MigrationId TEXT PRIMARY KEY,
    AppliedUtc  TEXT NOT NULL
                     DEFAULT CURRENT_TIMESTAMP,
    Description TEXT NOT NULL
                     DEFAULT ''
);

INSERT INTO MigrationHistory (
                                 MigrationId,
                                 AppliedUtc,
                                 Description
                             )
                             VALUES (
                                 '0001_engine_database_foundation',
                                 '2026-05-23 21:13:49',
                                 'Initial command engine database foundation.'
                             );

INSERT INTO MigrationHistory (
                                 MigrationId,
                                 AppliedUtc,
                                 Description
                             )
                             VALUES (
                                 '0002_sql_catalog_foundation',
                                 '2026-05-23 21:13:49',
                                 'Adds SQL catalog update tracking and catalog service support.'
                             );

INSERT INTO MigrationHistory (
                                 MigrationId,
                                 AppliedUtc,
                                 Description
                             )
                             VALUES (
                                 '0003_command_registration_foundation',
                                 '2026-05-23 21:13:49',
                                 'Adds command registration metadata and command parameter definitions.'
                             );

INSERT INTO MigrationHistory (
                                 MigrationId,
                                 AppliedUtc,
                                 Description
                             )
                             VALUES (
                                 '0004_workflow_model_foundation',
                                 '2026-05-23 21:13:49',
                                 'Adds workflow model metadata, workflow step metadata, and workflow SQL catalog entries.'
                             );

INSERT INTO MigrationHistory (
                                 MigrationId,
                                 AppliedUtc,
                                 Description
                             )
                             VALUES (
                                 '0005_execution_history_foundation',
                                 '2026-05-23 21:13:49',
                                 'Adds execution history repository support, correlation tracking, and execution SQL catalog entries.'
                             );

INSERT INTO MigrationHistory (
                                 MigrationId,
                                 AppliedUtc,
                                 Description
                             )
                             VALUES (
                                 '0006_context_system_foundation',
                                 '2026-05-23 21:13:49',
                                 'Adds execution context repository support and SQL catalog entries.'
                             );

INSERT INTO MigrationHistory (
                                 MigrationId,
                                 AppliedUtc,
                                 Description
                             )
                             VALUES (
                                 '0007_ai_provider_foundation',
                                 '2026-05-23 21:13:49',
                                 'Adds AI provider repository support and SQL catalog entries.'
                             );

INSERT INTO MigrationHistory (
                                 MigrationId,
                                 AppliedUtc,
                                 Description
                             )
                             VALUES (
                                 '0008_rebuilt_canonical_dev_schema',
                                 '2026-05-23 21:13:49',
                                 'Rebuilt canonical development schema from current repository expectations.'
                             );


-- Table: SchemaVersion
DROP TABLE IF EXISTS SchemaVersion;

CREATE TABLE IF NOT EXISTS SchemaVersion (
    SchemaVersionId INTEGER PRIMARY KEY,
    VersionNumber   INTEGER NOT NULL,
    AppliedUtc      TEXT    NOT NULL
                            DEFAULT CURRENT_TIMESTAMP,
    Description     TEXT    NOT NULL
                            DEFAULT ''
);

INSERT INTO SchemaVersion (
                              SchemaVersionId,
                              VersionNumber,
                              AppliedUtc,
                              Description
                          )
                          VALUES (
                              1,
                              1,
                              '2026-05-23 21:13:49',
                              'Rebuilt Codex.CommandEngine canonical development schema'
                          );


-- Table: SqlQuery
DROP TABLE IF EXISTS SqlQuery;

CREATE TABLE IF NOT EXISTS SqlQuery (
    QueryName   TEXT    PRIMARY KEY,
    Description TEXT    NOT NULL
                        DEFAULT '',
    SqlText     TEXT    NOT NULL
                        DEFAULT '',
    CreatedUtc  TEXT    NOT NULL
                        DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc  TEXT    NULL,
    Category    TEXT    NOT NULL
                        DEFAULT '',
    IsActive    INTEGER NOT NULL
                        DEFAULT 1,
    UNIQUE (
        QueryName ASC
    )
);

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'SqlQuery_SelectByName',
                         'Selects one SQL catalog query by name.',
                         'SELECT QueryName, Description, SqlText, CreatedUtc, UpdatedUtc, Category, IsActive FROM SqlQuery WHERE QueryName = $QueryName;',
                         '2026-05-23 21:13:49',
                         NULL,
                         'SQL Catalog',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'SqlQuery_SelectAll',
                         'Lists all SQL catalog queries.',
                         'SELECT QueryName, Description, SqlText, CreatedUtc, UpdatedUtc, Category, IsActive FROM SqlQuery ORDER BY Category ASC, QueryName ASC;',
                         '2026-05-23 21:13:49',
                         NULL,
                         'SQL Catalog',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'SqlQuery_InsertOrReplace',
                         'Inserts or updates a SQL catalog query.',
                         'INSERT INTO SqlQuery (QueryName, Description, SqlText, Category, IsActive, UpdatedUtc)
VALUES ($QueryName, $Description, $SqlText, $Category, $IsActive, CURRENT_TIMESTAMP)
ON CONFLICT(QueryName) DO UPDATE SET
    Description = excluded.Description,
    SqlText = excluded.SqlText,
    Category = excluded.Category,
    IsActive = excluded.IsActive,
    UpdatedUtc = CURRENT_TIMESTAMP;',
                         '2026-05-23 21:13:49',
                         NULL,
                         'SQL Catalog',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'CommandDefinition_InsertOrReplace',
                         'Inserts or updates a command definition.',
                         'INSERT INTO CommandDefinition (CommandId, CommandDefinitionId, CommandName, DisplayName, Description, Category, Version, IsEnabled, HandlerType, HandlerKey, InputJson, OutputJson, MetadataJson)
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
    UpdatedUtc = CURRENT_TIMESTAMP;',
                         '2026-05-23 21:13:49',
                         NULL,
                         'Command Registration',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'CommandDefinition_SelectByName',
                         'Selects a command definition by name.',
                         'SELECT CommandId, CommandName, DisplayName, Description, Category, Version, IsEnabled, HandlerType, CreatedUtc, UpdatedUtc FROM CommandDefinition WHERE CommandName = $CommandName;',
                         '2026-05-23 21:13:49',
                         NULL,
                         'Command Registration',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'CommandDefinition_SelectAll',
                         'Lists command definitions.',
                         'SELECT CommandId, CommandName, DisplayName, Description, Category, Version, IsEnabled, HandlerType, CreatedUtc, UpdatedUtc FROM CommandDefinition ORDER BY Category ASC, CommandName ASC;',
                         '2026-05-23 21:13:49',
                         NULL,
                         'Command Registration',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'CommandParameterDefinition_InsertOrReplace',
                         'Inserts or updates a command parameter definition.',
                         'INSERT INTO CommandParameterDefinition (CommandParameterDefinitionId, CommandId, CommandDefinitionId, ParameterName, ParameterType, IsRequired, DefaultValue, Description, SortOrder)
VALUES ($CommandParameterDefinitionId, $CommandId, $CommandDefinitionId, $ParameterName, $ParameterType, $IsRequired, $DefaultValue, $Description, $SortOrder)
ON CONFLICT(CommandId, ParameterName) DO UPDATE SET
    CommandDefinitionId = excluded.CommandDefinitionId,
    ParameterType = excluded.ParameterType,
    IsRequired = excluded.IsRequired,
    DefaultValue = excluded.DefaultValue,
    Description = excluded.Description,
    SortOrder = excluded.SortOrder,
    UpdatedUtc = CURRENT_TIMESTAMP;',
                         '2026-05-23 21:13:49',
                         NULL,
                         'Command Registration',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'CommandParameterDefinition_SelectByCommand',
                         'Lists parameter definitions for a command.',
                         'SELECT CommandParameterDefinitionId, CommandId, ParameterName, ParameterType, IsRequired, DefaultValue, Description, SortOrder, CreatedUtc, UpdatedUtc FROM CommandParameterDefinition WHERE CommandId = $CommandId ORDER BY SortOrder ASC, ParameterName ASC;',
                         '2026-05-23 21:13:49',
                         NULL,
                         'Command Registration',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'WorkflowDefinition_InsertOrReplace',
                         'Inserts or updates a workflow definition.',
                         'INSERT INTO WorkflowDefinition (WorkflowId, WorkflowDefinitionId, WorkflowName, DisplayName, Description, Version, MetadataJson, IsEnabled, IsActive)
VALUES ($WorkflowId, $WorkflowId, $WorkflowName, $DisplayName, $Description, $Version, $MetadataJson, $IsEnabled, $IsEnabled)
ON CONFLICT(WorkflowName, Version) DO UPDATE SET
    WorkflowDefinitionId = excluded.WorkflowDefinitionId,
    DisplayName = excluded.DisplayName,
    Description = excluded.Description,
    MetadataJson = excluded.MetadataJson,
    IsEnabled = excluded.IsEnabled,
    IsActive = excluded.IsActive,
    UpdatedUtc = CURRENT_TIMESTAMP;',
                         '2026-05-23 21:13:49',
                         NULL,
                         'Workflow Foundation',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'WorkflowDefinition_SelectAll',
                         'Lists workflow definitions.',
                         'SELECT WorkflowId, WorkflowDefinitionId, WorkflowName, DisplayName, Description, Version, IsEnabled, IsActive, MetadataJson, CreatedUtc, UpdatedUtc FROM WorkflowDefinition ORDER BY WorkflowName ASC, Version DESC;',
                         '2026-05-23 21:13:49',
                         NULL,
                         'Workflow Foundation',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'WorkflowDefinition_SelectByNameAndVersion',
                         'Selects workflow definition by name and version.',
                         'SELECT WorkflowId, WorkflowDefinitionId, WorkflowName, DisplayName, Description, Version, IsEnabled, IsActive, MetadataJson, CreatedUtc, UpdatedUtc FROM WorkflowDefinition WHERE WorkflowName = $WorkflowName AND Version = $Version;',
                         '2026-05-23 21:13:49',
                         NULL,
                         'Workflow Foundation',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'WorkflowStep_InsertOrReplace',
                         'Inserts or updates a workflow step.',
                         'INSERT INTO WorkflowStep (WorkflowStepId, WorkflowId, StepOrder, StepName, CommandId, CommandName, InputMapJson, MetadataJson, IsEnabled)
VALUES ($WorkflowStepId, $WorkflowId, $StepOrder, $StepName, $CommandId, $CommandId, $InputMapJson, $MetadataJson, $IsEnabled)
ON CONFLICT(WorkflowId, StepOrder) DO UPDATE SET
    StepName = excluded.StepName,
    CommandId = excluded.CommandId,
    CommandName = excluded.CommandName,
    InputMapJson = excluded.InputMapJson,
    MetadataJson = excluded.MetadataJson,
    IsEnabled = excluded.IsEnabled;',
                         '2026-05-23 21:13:49',
                         NULL,
                         'Workflow Foundation',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'WorkflowStep_SelectByWorkflow',
                         'Lists workflow steps for a workflow.',
                         'SELECT WorkflowStepId, WorkflowId, StepOrder, StepName, CommandId, InputMapJson, MetadataJson, IsEnabled FROM WorkflowStep WHERE WorkflowId = $WorkflowId ORDER BY StepOrder ASC;',
                         '2026-05-23 21:13:49',
                         NULL,
                         'Workflow Foundation',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'ExecutionHistory_Insert',
                         'Inserts an execution history row.',
                         'INSERT INTO ExecutionHistory (ExecutionId, ExecutionKind, TargetId, TargetName, CommandName, CorrelationId, Status, StartedUtc, RequestJson, InputJson, MetadataJson)
VALUES ($ExecutionId, $ExecutionKind, $TargetId, $TargetName, $TargetName, $CorrelationId, $Status, $StartedUtc, $InputJson, $InputJson, $MetadataJson);',
                         '2026-05-23 21:13:49',
                         NULL,
                         'Execution History',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'ExecutionHistory_Complete',
                         'Completes an execution history row.',
                         'UPDATE ExecutionHistory SET Status = $Status, CompletedUtc = $CompletedUtc, OutputJson = $OutputJson, ErrorMessage = $ErrorMessage, MetadataJson = $MetadataJson WHERE ExecutionId = $ExecutionId;',
                         '2026-05-23 21:13:49',
                         NULL,
                         'Execution History',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'ExecutionHistory_SelectById',
                         'Selects execution history by id.',
                         'SELECT ExecutionId, ExecutionKind, TargetId, TargetName, CommandName, CorrelationId, Status, StartedUtc, CompletedUtc, RequestJson, InputJson, OutputJson, Message, ErrorMessage, ExceptionText, MetadataJson FROM ExecutionHistory WHERE ExecutionId = $ExecutionId;',
                         '2026-05-23 21:13:49',
                         NULL,
                         'Execution History',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'ExecutionHistory_SelectRecent',
                         'Lists recent execution history.',
                         'SELECT ExecutionId, ExecutionKind, TargetId, TargetName, CommandName, CorrelationId, Status, StartedUtc, CompletedUtc, RequestJson, InputJson, OutputJson, Message, ErrorMessage, ExceptionText, MetadataJson FROM ExecutionHistory ORDER BY StartedUtc DESC LIMIT $Limit;',
                         '2026-05-23 21:13:49',
                         NULL,
                         'Execution History',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'ExecutionStepHistory_Insert',
                         'Inserts an execution step history row.',
                         'INSERT INTO ExecutionStepHistory (ExecutionStepHistoryId, ExecutionStepId, ExecutionId, WorkflowStepId, CommandId, StepOrder, StepName, Status, StartedUtc, InputJson, MetadataJson)
VALUES ($ExecutionStepId, $ExecutionStepId, $ExecutionId, $WorkflowStepId, $CommandId, $StepOrder, $StepName, $Status, $StartedUtc, $InputJson, $MetadataJson);',
                         '2026-05-23 21:13:49',
                         NULL,
                         'Execution History',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'ExecutionStepHistory_Complete',
                         'Completes an execution step history row.',
                         'UPDATE ExecutionStepHistory SET Status = $Status, CompletedUtc = $CompletedUtc, OutputJson = $OutputJson, ErrorMessage = $ErrorMessage, MetadataJson = $MetadataJson WHERE ExecutionStepId = $ExecutionStepId OR ExecutionStepHistoryId = $ExecutionStepId;',
                         '2026-05-23 21:13:49',
                         NULL,
                         'Execution History',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'ExecutionStepHistory_SelectByExecution',
                         'Lists execution steps for one execution.',
                         'SELECT ExecutionStepHistoryId, ExecutionStepId, ExecutionId, WorkflowStepId, WorkflowStepDefinitionId, CommandId, CommandDefinitionId, StepOrder, StepName, Status, StartedUtc, CompletedUtc, InputJson, OutputJson, Message, ErrorMessage, MetadataJson FROM ExecutionStepHistory WHERE ExecutionId = $ExecutionId ORDER BY StepOrder ASC;',
                         '2026-05-23 21:13:49',
                         NULL,
                         'Execution History',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'EngineContext_InsertOrReplace',
                         'Inserts or updates execution context.',
                         'INSERT INTO ExecutionContext (ContextId, ContextName, Scope, Description, ContextJson, MetadataJson, IsEnabled)
VALUES ($ContextId, $ContextName, $Scope, $Description, $ContextJson, $MetadataJson, $IsEnabled)
ON CONFLICT(ContextName) DO UPDATE SET
    Scope = excluded.Scope,
    Description = excluded.Description,
    ContextJson = excluded.ContextJson,
    MetadataJson = excluded.MetadataJson,
    IsEnabled = excluded.IsEnabled,
    UpdatedUtc = CURRENT_TIMESTAMP;',
                         '2026-05-23 21:13:49',
                         NULL,
                         'Context System Foundation',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'EngineContext_SelectByName',
                         'Selects context by name.',
                         'SELECT ContextId, ContextName, Scope, Description, ContextJson, MetadataJson, IsEnabled, CreatedUtc, UpdatedUtc FROM ExecutionContext WHERE ContextName = $ContextName;',
                         '2026-05-23 21:13:49',
                         NULL,
                         'Context System Foundation',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'EngineContext_SelectAll',
                         'Lists contexts.',
                         'SELECT ContextId, ContextName, Scope, Description, ContextJson, MetadataJson, IsEnabled, CreatedUtc, UpdatedUtc FROM ExecutionContext ORDER BY Scope ASC, ContextName ASC;',
                         '2026-05-23 21:13:49',
                         NULL,
                         'Context System Foundation',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'EngineContext_SelectEnabled',
                         'Lists enabled contexts.',
                         'SELECT ContextId, ContextName, Scope, Description, ContextJson, MetadataJson, IsEnabled, CreatedUtc, UpdatedUtc FROM ExecutionContext WHERE IsEnabled = 1 ORDER BY Scope ASC, ContextName ASC;',
                         '2026-05-23 21:13:49',
                         NULL,
                         'Context System Foundation',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'EngineContext_Deactivate',
                         'Disables a context.',
                         'UPDATE ExecutionContext SET IsEnabled = 0, UpdatedUtc = CURRENT_TIMESTAMP WHERE ContextId = $ContextId;',
                         '2026-05-23 21:13:49',
                         NULL,
                         'Context System Foundation',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'AiProvider_InsertOrReplace',
                         'Inserts or updates AI provider.',
                         'INSERT INTO AiProvider (AiProviderId, ProviderName, DisplayName, ProviderKind, Description, ConfigurationJson, MetadataJson, IsEnabled)
VALUES ($AiProviderId, $ProviderName, $DisplayName, $ProviderKind, $Description, $ConfigurationJson, $MetadataJson, $IsEnabled)
ON CONFLICT(ProviderName) DO UPDATE SET
    DisplayName = excluded.DisplayName,
    ProviderKind = excluded.ProviderKind,
    Description = excluded.Description,
    ConfigurationJson = excluded.ConfigurationJson,
    MetadataJson = excluded.MetadataJson,
    IsEnabled = excluded.IsEnabled,
    UpdatedUtc = CURRENT_TIMESTAMP;',
                         '2026-05-23 21:13:49',
                         NULL,
                         'AI Provider Foundation',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'AiProvider_SelectByName',
                         'Selects AI provider by name.',
                         'SELECT AiProviderId, ProviderName, DisplayName, ProviderKind, Description, ConfigurationJson, MetadataJson, IsEnabled, CreatedUtc, UpdatedUtc FROM AiProvider WHERE ProviderName = $ProviderName;',
                         '2026-05-23 21:13:49',
                         NULL,
                         'AI Provider Foundation',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'AiProvider_SelectAll',
                         'Lists AI providers.',
                         'SELECT AiProviderId, ProviderName, DisplayName, ProviderKind, Description, ConfigurationJson, MetadataJson, IsEnabled, CreatedUtc, UpdatedUtc FROM AiProvider ORDER BY ProviderName ASC;',
                         '2026-05-23 21:13:49',
                         NULL,
                         'AI Provider Foundation',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'AiProviderCapability_InsertOrReplace',
                         'Inserts or updates AI provider capability.',
                         'INSERT INTO AiProviderCapability (AiProviderCapabilityId, AiProviderId, CapabilityName, CapabilityKind, Description, MetadataJson, IsEnabled)
VALUES ($AiProviderCapabilityId, $AiProviderId, $CapabilityName, $CapabilityKind, $Description, $MetadataJson, $IsEnabled)
ON CONFLICT(AiProviderId, CapabilityName) DO UPDATE SET
    CapabilityKind = excluded.CapabilityKind,
    Description = excluded.Description,
    MetadataJson = excluded.MetadataJson,
    IsEnabled = excluded.IsEnabled,
    UpdatedUtc = CURRENT_TIMESTAMP;',
                         '2026-05-23 21:13:49',
                         NULL,
                         'AI Provider Foundation',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'AiProviderCapability_SelectByProvider',
                         'Lists AI provider capabilities.',
                         'SELECT AiProviderCapabilityId, AiProviderId, CapabilityName, CapabilityKind, Description, MetadataJson, IsEnabled, CreatedUtc, UpdatedUtc FROM AiProviderCapability WHERE AiProviderId = $AiProviderId ORDER BY CapabilityName ASC;',
                         '2026-05-23 21:13:49',
                         NULL,
                         'AI Provider Foundation',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'ExecutionContext_Compatibility_01',
                         'Compatibility row for legacy execution-context catalog category count.',
                         'SELECT 1;',
                         '2026-05-23 21:38:50',
                         NULL,
                         'Execution Context Foundation',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'ExecutionContext_Compatibility_02',
                         'Compatibility row for legacy execution-context catalog category count.',
                         'SELECT 1;',
                         '2026-05-23 21:38:50',
                         NULL,
                         'Execution Context Foundation',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'ExecutionContext_Compatibility_03',
                         'Compatibility row for legacy execution-context catalog category count.',
                         'SELECT 1;',
                         '2026-05-23 21:38:50',
                         NULL,
                         'Execution Context Foundation',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'ExecutionContext_Compatibility_04',
                         'Compatibility row for legacy execution-context catalog category count.',
                         'SELECT 1;',
                         '2026-05-23 21:38:50',
                         NULL,
                         'Execution Context Foundation',
                         1
                     );

INSERT INTO SqlQuery (
                         QueryName,
                         Description,
                         SqlText,
                         CreatedUtc,
                         UpdatedUtc,
                         Category,
                         IsActive
                     )
                     VALUES (
                         'ExecutionContext_Compatibility_05',
                         'Compatibility row for legacy execution-context catalog category count.',
                         'SELECT 1;',
                         '2026-05-23 21:38:50',
                         NULL,
                         'Execution Context Foundation',
                         1
                     );


-- Table: WorkflowDefinition
DROP TABLE IF EXISTS WorkflowDefinition;

CREATE TABLE IF NOT EXISTS WorkflowDefinition (
    WorkflowId           TEXT    NOT NULL
                                 DEFAULT '',
    WorkflowDefinitionId TEXT    NOT NULL
                                 DEFAULT '',
    WorkflowName         TEXT    NOT NULL,
    DisplayName          TEXT    NOT NULL
                                 DEFAULT '',
    Description          TEXT    NOT NULL
                                 DEFAULT '',
    Version              INTEGER NOT NULL
                                 DEFAULT 1,
    MetadataJson         TEXT    NOT NULL
                                 DEFAULT '{}',
    IsEnabled            INTEGER NOT NULL
                                 DEFAULT 1,
    IsActive             INTEGER NOT NULL
                                 DEFAULT 1,
    CreatedUtc           TEXT    NOT NULL
                                 DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc           TEXT    NULL,
    PRIMARY KEY (
        WorkflowId
    ),
    UNIQUE (
        WorkflowName
    ),
    UNIQUE (
        WorkflowName,
        Version
    )
);


-- Table: WorkflowDefinitionStep
DROP TABLE IF EXISTS WorkflowDefinitionStep;

CREATE TABLE IF NOT EXISTS WorkflowDefinitionStep (
    WorkflowDefinitionStepId TEXT    NOT NULL
                                     DEFAULT '',
    WorkflowDefinitionId     TEXT    NOT NULL,
    StepName                 TEXT    NOT NULL,
    StepOrder                INTEGER NOT NULL,
    CommandName              TEXT    NOT NULL,
    ParametersJson           TEXT    NOT NULL
                                     DEFAULT '{}',
    IsEnabled                INTEGER NOT NULL
                                     DEFAULT 1,
    CommandDefinitionId      TEXT    NOT NULL
                                     DEFAULT '',
    InputMapJson             TEXT    NOT NULL
                                     DEFAULT '{}',
    CreatedUtc               TEXT    NOT NULL
                                     DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc               TEXT    NULL,
    PRIMARY KEY (
        WorkflowDefinitionStepId
    ),
    UNIQUE (
        WorkflowDefinitionId,
        StepOrder
    )
);


-- Table: WorkflowExecution
DROP TABLE IF EXISTS WorkflowExecution;

CREATE TABLE IF NOT EXISTS WorkflowExecution (
    WorkflowExecutionId    TEXT    PRIMARY KEY,
    WorkflowName           TEXT    NOT NULL,
    CorrelationId          TEXT    NOT NULL
                                   DEFAULT '',
    Status                 TEXT    NOT NULL,
    StartedUtc             TEXT    NOT NULL
                                   DEFAULT CURRENT_TIMESTAMP,
    CompletedUtc           TEXT    NULL,
    CurrentStepOrder       INTEGER NOT NULL
                                   DEFAULT 0,
    LastCompletedStepOrder INTEGER NOT NULL
                                   DEFAULT 0,
    IsResumable            INTEGER NOT NULL
                                   DEFAULT 1,
    ResumeToken            TEXT    NOT NULL
                                   DEFAULT '',
    LastHeartbeatUtc       TEXT    NULL,
    RuntimeStateJson       TEXT    NOT NULL
                                   DEFAULT '{}',
    ContextJson            TEXT    NOT NULL
                                   DEFAULT '{}',
    Message                TEXT    NOT NULL
                                   DEFAULT '',
    ErrorMessage           TEXT    NULL,
    MetadataJson           TEXT    NOT NULL
                                   DEFAULT '{}'
);


-- Table: WorkflowStep
DROP TABLE IF EXISTS WorkflowStep;

CREATE TABLE IF NOT EXISTS WorkflowStep (
    WorkflowStepId TEXT    PRIMARY KEY,
    WorkflowId     TEXT    NOT NULL,
    StepOrder      INTEGER NOT NULL,
    StepName       TEXT    NOT NULL
                           DEFAULT '',
    CommandId      TEXT    NOT NULL,
    CommandName    TEXT    NOT NULL
                           DEFAULT '',
    InputMapJson   TEXT    NOT NULL
                           DEFAULT '{}',
    MetadataJson   TEXT    NOT NULL
                           DEFAULT '{}',
    IsEnabled      INTEGER NOT NULL
                           DEFAULT 1,
    FOREIGN KEY (
        WorkflowId
    )
    REFERENCES WorkflowDefinition (WorkflowId) ON DELETE CASCADE,
    FOREIGN KEY (
        CommandId
    )
    REFERENCES CommandDefinition (CommandId) ON DELETE RESTRICT,
    UNIQUE (
        WorkflowId,
        StepOrder
    )
);


-- Table: WorkflowStepExecution
DROP TABLE IF EXISTS WorkflowStepExecution;

CREATE TABLE IF NOT EXISTS WorkflowStepExecution (
    WorkflowStepExecutionId TEXT    PRIMARY KEY,
    WorkflowExecutionId     TEXT    NOT NULL,
    StepName                TEXT    NOT NULL,
    StepOrder               INTEGER NOT NULL,
    CommandName             TEXT    NOT NULL,
    Status                  TEXT    NOT NULL,
    StartedUtc              TEXT    NOT NULL
                                    DEFAULT CURRENT_TIMESTAMP,
    CompletedUtc            TEXT    NULL,
    Message                 TEXT    NOT NULL
                                    DEFAULT '',
    CommandExecutionId      TEXT    NULL,
    FOREIGN KEY (
        WorkflowExecutionId
    )
    REFERENCES WorkflowExecution (WorkflowExecutionId) ON DELETE CASCADE
);


COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
