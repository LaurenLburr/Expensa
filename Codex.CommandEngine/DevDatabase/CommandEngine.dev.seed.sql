BEGIN TRANSACTION;
CREATE TABLE AiProvider (
    AiProviderId      TEXT    PRIMARY KEY,
    ProviderName      TEXT    NOT NULL UNIQUE,
    ProviderKind      TEXT    NOT NULL DEFAULT '',
    ConfigurationJson TEXT    NOT NULL DEFAULT '{}',
    IsEnabled         INTEGER NOT NULL DEFAULT 1,
    CreatedUtc        TEXT    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc        TEXT    NULL
);
INSERT INTO "AiProvider" VALUES('ai-provider-openai','OpenAI','OpenAI','{"apiKeyEnvironmentVariable":"OPENAI_API_KEY","baseUrl":"https://api.openai.com/v1"}',1,'2026-05-17 16:01:31','2026-05-17 16:01:31');
CREATE TABLE Command (
    CommandId   TEXT    PRIMARY KEY,
    CommandName TEXT    NOT NULL UNIQUE,
    Description TEXT    NOT NULL DEFAULT '',
    IsActive    INTEGER NOT NULL DEFAULT 1
);
CREATE TABLE CommandDefinition (
    CommandId    TEXT    PRIMARY KEY,
    CommandName  TEXT    NOT NULL UNIQUE,
    Description  TEXT    NOT NULL DEFAULT '',
    Category     TEXT    NOT NULL DEFAULT '',
    Version      INTEGER NOT NULL DEFAULT 1,
    HandlerKey   TEXT    NOT NULL DEFAULT '',
    InputJson    TEXT    NOT NULL DEFAULT '{}',
    OutputJson   TEXT    NOT NULL DEFAULT '{}',
    MetadataJson TEXT    NOT NULL DEFAULT '{}',
    IsEnabled    INTEGER NOT NULL DEFAULT 1,
    CreatedUtc   TEXT    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc   TEXT    NULL,
    DisplayName  TEXT    NOT NULL DEFAULT '',
    HandlerType  TEXT    NOT NULL DEFAULT ''
);
CREATE TABLE CommandHistory (
    CommandHistoryId TEXT    PRIMARY KEY,
    CommandName      TEXT    NOT NULL,
    StartedUtc       TEXT    NOT NULL,
    CompletedUtc     TEXT    NULL,
    Succeeded        INTEGER NOT NULL DEFAULT 0,
    Message          TEXT    NOT NULL DEFAULT ''
);
CREATE TABLE CommandParameterDefinition (
    CommandParameterDefinitionId TEXT    PRIMARY KEY,
    CommandId                    TEXT    NOT NULL,
    ParameterName                TEXT    NOT NULL,
    ParameterType                TEXT    NOT NULL,
    IsRequired                   INTEGER NOT NULL DEFAULT 0,
    DefaultValue                 TEXT    NOT NULL DEFAULT '',
    Description                  TEXT    NOT NULL DEFAULT '',
    SortOrder                    INTEGER NOT NULL DEFAULT 0,
    CreatedUtc                   TEXT    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc                   TEXT    NULL,
    FOREIGN KEY (CommandId) REFERENCES CommandDefinition (CommandId) ON DELETE CASCADE,
    UNIQUE (CommandId, ParameterName)
);
CREATE TABLE DatabaseSchemaSnapshot (
    MigrationId   TEXT    PRIMARY KEY,
    SchemaVersion INTEGER NOT NULL,
    SnapshotSql   TEXT    NOT NULL,
    SnapshotHash  TEXT    NOT NULL,
    CreatedUtc    TEXT    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description   TEXT    NOT NULL DEFAULT '',
    FOREIGN KEY (MigrationId) REFERENCES MigrationHistory (MigrationId) ON DELETE CASCADE
);
CREATE TABLE ExecutionContext (
    ContextId   TEXT PRIMARY KEY,
    ContextName TEXT NOT NULL UNIQUE,
    ContextJson TEXT NOT NULL DEFAULT '{}',
    CreatedUtc  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc  TEXT NULL
);
INSERT INTO "ExecutionContext" VALUES('execution-context-default','Default','{}','2026-05-17 16:01:31','2026-05-17 16:01:31');
CREATE TABLE ExecutionHistory (
    ExecutionId   TEXT PRIMARY KEY,
    ExecutionKind TEXT NOT NULL,
    TargetId      TEXT NOT NULL,
    TargetName    TEXT NOT NULL DEFAULT '',
    Status        TEXT NOT NULL,
    StartedUtc    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CompletedUtc  TEXT NULL,
    InputJson     TEXT NOT NULL DEFAULT '{}',
    OutputJson    TEXT NOT NULL DEFAULT '{}',
    ErrorMessage  TEXT NOT NULL DEFAULT '',
    MetadataJson  TEXT NOT NULL DEFAULT '{}',
    CorrelationId TEXT NOT NULL DEFAULT ''
);
CREATE TABLE ExecutionStepHistory (
    ExecutionStepId TEXT    PRIMARY KEY,
    ExecutionId     TEXT    NOT NULL,
    WorkflowStepId  TEXT    NULL,
    CommandId       TEXT    NULL,
    StepOrder       INTEGER NOT NULL,
    Status          TEXT    NOT NULL,
    StartedUtc      TEXT    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CompletedUtc    TEXT    NULL,
    InputJson       TEXT    NOT NULL DEFAULT '{}',
    OutputJson      TEXT    NOT NULL DEFAULT '{}',
    ErrorMessage    TEXT    NOT NULL DEFAULT '',
    MetadataJson    TEXT    NOT NULL DEFAULT '{}',
    FOREIGN KEY (ExecutionId) REFERENCES ExecutionHistory (ExecutionId) ON DELETE CASCADE,
    FOREIGN KEY (WorkflowStepId) REFERENCES WorkflowStep (WorkflowStepId) ON DELETE SET NULL,
    FOREIGN KEY (CommandId) REFERENCES CommandDefinition (CommandId) ON DELETE SET NULL
);
CREATE TABLE Log (
    LogId     INTEGER PRIMARY KEY AUTOINCREMENT,
    Timestamp TEXT    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Action    TEXT    NOT NULL,
    Details   TEXT    NULL
);
CREATE TABLE MigrationHistory (
    MigrationId TEXT PRIMARY KEY,
    AppliedUtc  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description TEXT NOT NULL DEFAULT ''
);
CREATE TABLE Setting (
    SettingName  TEXT PRIMARY KEY,
    SettingValue TEXT NOT NULL
);
INSERT INTO "Setting" VALUES('DatabaseMode','DevObjectCopy');
INSERT INTO "Setting" VALUES('DevDatabaseVersion','2026-05-17.1');
CREATE TABLE SqlQuery (
    QueryName   TEXT    PRIMARY KEY,
    Description TEXT    NOT NULL DEFAULT '',
    SqlText     TEXT    NOT NULL DEFAULT '',
    CreatedUtc  TEXT    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Category    TEXT    NOT NULL DEFAULT '',
    IsActive    INTEGER NOT NULL DEFAULT 1,
    UpdatedUtc  TEXT    NULL
);
INSERT INTO "SqlQuery" VALUES('SqlQuery_SelectByName','Selects one active SQL catalog entry by query name.','SELECT QueryName,
       Description,
       SqlText,
       CreatedUtc,
       Category,
       IsActive,
       UpdatedUtc
FROM SqlQuery
WHERE QueryName = $QueryName
  AND IsActive = 1;','2026-05-17 16:01:31','SQL Catalog',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('SqlQuery_GetByName','Compatibility alias for SqlQuery_SelectByName.','SELECT QueryName,
       Description,
       SqlText,
       CreatedUtc,
       Category,
       IsActive,
       UpdatedUtc
FROM SqlQuery
WHERE QueryName = $QueryName
  AND IsActive = 1;','2026-05-17 16:01:31','SQL Catalog',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('SqlQuery_FindByName','Compatibility alias for SqlQuery_SelectByName.','SELECT QueryName,
       Description,
       SqlText,
       CreatedUtc,
       Category,
       IsActive,
       UpdatedUtc
FROM SqlQuery
WHERE QueryName = $QueryName
  AND IsActive = 1;','2026-05-17 16:01:31','SQL Catalog',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('SqlQuery_SelectAll','Lists all SQL catalog entries ordered by category and query name.','SELECT QueryName,
       Description,
       SqlText,
       CreatedUtc,
       Category,
       IsActive,
       UpdatedUtc
FROM SqlQuery
ORDER BY Category ASC,
         QueryName ASC;','2026-05-17 16:01:31','SQL Catalog',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('SqlQuery_SelectActive','Lists active SQL catalog entries ordered by category and query name.','SELECT QueryName,
       Description,
       SqlText,
       CreatedUtc,
       Category,
       IsActive,
       UpdatedUtc
FROM SqlQuery
WHERE IsActive = 1
ORDER BY Category ASC,
         QueryName ASC;','2026-05-17 16:01:31','SQL Catalog',1,'2026-05-17 16:01:31');
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
    UpdatedUtc = CURRENT_TIMESTAMP;','2026-05-17 16:01:31','SQL Catalog',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('SqlQuery_Upsert','Compatibility alias for SqlQuery_InsertOrReplace.','INSERT INTO SqlQuery (
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
    UpdatedUtc = CURRENT_TIMESTAMP;','2026-05-17 16:01:31','SQL Catalog',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('AiProvider_InsertOrReplace','Inserts or updates an AI provider by provider name.','INSERT INTO AiProvider (
    AiProviderId,
    ProviderName,
    ProviderKind,
    ConfigurationJson,
    IsEnabled
)
VALUES (
    $AiProviderId,
    $ProviderName,
    $ProviderKind,
    $ConfigurationJson,
    $IsEnabled
)
ON CONFLICT(ProviderName) DO UPDATE SET
    ProviderKind = excluded.ProviderKind,
    ConfigurationJson = excluded.ConfigurationJson,
    IsEnabled = excluded.IsEnabled,
    UpdatedUtc = CURRENT_TIMESTAMP;','2026-05-17 16:01:31','AI Provider Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('AiProvider_Upsert','Compatibility alias for AiProvider_InsertOrReplace.','INSERT INTO AiProvider (
    AiProviderId,
    ProviderName,
    ProviderKind,
    ConfigurationJson,
    IsEnabled
)
VALUES (
    $AiProviderId,
    $ProviderName,
    $ProviderKind,
    $ConfigurationJson,
    $IsEnabled
)
ON CONFLICT(ProviderName) DO UPDATE SET
    ProviderKind = excluded.ProviderKind,
    ConfigurationJson = excluded.ConfigurationJson,
    IsEnabled = excluded.IsEnabled,
    UpdatedUtc = CURRENT_TIMESTAMP;','2026-05-17 16:01:31','AI Provider Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('AiProvider_SelectByName','Selects an AI provider by provider name.','SELECT AiProviderId,
       ProviderName,
       ProviderKind,
       ConfigurationJson,
       IsEnabled,
       CreatedUtc,
       UpdatedUtc
FROM AiProvider
WHERE ProviderName = $ProviderName;','2026-05-17 16:01:31','AI Provider Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('AiProvider_FindByName','Compatibility alias for AiProvider_SelectByName.','SELECT AiProviderId,
       ProviderName,
       ProviderKind,
       ConfigurationJson,
       IsEnabled,
       CreatedUtc,
       UpdatedUtc
FROM AiProvider
WHERE ProviderName = $ProviderName;','2026-05-17 16:01:31','AI Provider Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('AiProvider_GetByName','Compatibility alias for AiProvider_SelectByName.','SELECT AiProviderId,
       ProviderName,
       ProviderKind,
       ConfigurationJson,
       IsEnabled,
       CreatedUtc,
       UpdatedUtc
FROM AiProvider
WHERE ProviderName = $ProviderName;','2026-05-17 16:01:31','AI Provider Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('AiProvider_SelectById','Selects an AI provider by provider id.','SELECT AiProviderId,
       ProviderName,
       ProviderKind,
       ConfigurationJson,
       IsEnabled,
       CreatedUtc,
       UpdatedUtc
FROM AiProvider
WHERE AiProviderId = $AiProviderId;','2026-05-17 16:01:31','AI Provider Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('AiProvider_SelectAll','Lists all AI providers ordered by provider name.','SELECT AiProviderId,
       ProviderName,
       ProviderKind,
       ConfigurationJson,
       IsEnabled,
       CreatedUtc,
       UpdatedUtc
FROM AiProvider
ORDER BY ProviderName ASC;','2026-05-17 16:01:31','AI Provider Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('AiProvider_List','Compatibility alias for AiProvider_SelectAll.','SELECT AiProviderId,
       ProviderName,
       ProviderKind,
       ConfigurationJson,
       IsEnabled,
       CreatedUtc,
       UpdatedUtc
FROM AiProvider
ORDER BY ProviderName ASC;','2026-05-17 16:01:31','AI Provider Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('AiProvider_SelectEnabled','Lists enabled AI providers ordered by provider name.','SELECT AiProviderId,
       ProviderName,
       ProviderKind,
       ConfigurationJson,
       IsEnabled,
       CreatedUtc,
       UpdatedUtc
FROM AiProvider
WHERE IsEnabled = 1
ORDER BY ProviderName ASC;','2026-05-17 16:01:31','AI Provider Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('AiProvider_DeleteByName','Deletes an AI provider by provider name.','DELETE FROM AiProvider
WHERE ProviderName = $ProviderName;','2026-05-17 16:01:31','AI Provider Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('AiProvider_Delete','Compatibility alias for AiProvider_DeleteByName.','DELETE FROM AiProvider
WHERE ProviderName = $ProviderName;','2026-05-17 16:01:31','AI Provider Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('AiProvider_DeleteById','Deletes an AI provider by provider id.','DELETE FROM AiProvider
WHERE AiProviderId = $AiProviderId;','2026-05-17 16:01:31','AI Provider Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('ExecutionContext_InsertOrReplace','ExecutionContext compatibility/upsert query.','INSERT INTO ExecutionContext (
    ContextId,
    ContextName,
    ContextJson
)
VALUES (
    $ContextId,
    $ContextName,
    $ContextJson
)
ON CONFLICT(ContextName) DO UPDATE SET
    ContextJson = excluded.ContextJson,
    UpdatedUtc = CURRENT_TIMESTAMP;','2026-05-17 16:01:31','Execution Context Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('ExecutionContext_Upsert','ExecutionContext compatibility/upsert query.','INSERT INTO ExecutionContext (
    ContextId,
    ContextName,
    ContextJson
)
VALUES (
    $ContextId,
    $ContextName,
    $ContextJson
)
ON CONFLICT(ContextName) DO UPDATE SET
    ContextJson = excluded.ContextJson,
    UpdatedUtc = CURRENT_TIMESTAMP;','2026-05-17 16:01:31','Execution Context Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('EngineContext_InsertOrReplace','EngineContext compatibility/upsert query.','INSERT INTO ExecutionContext (
    ContextId,
    ContextName,
    ContextJson
)
VALUES (
    $ContextId,
    $ContextName,
    $ContextJson
)
ON CONFLICT(ContextName) DO UPDATE SET
    ContextJson = excluded.ContextJson,
    UpdatedUtc = CURRENT_TIMESTAMP;','2026-05-17 16:01:31','Execution Context Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('EngineContext_Upsert','EngineContext compatibility/upsert query.','INSERT INTO ExecutionContext (
    ContextId,
    ContextName,
    ContextJson
)
VALUES (
    $ContextId,
    $ContextName,
    $ContextJson
)
ON CONFLICT(ContextName) DO UPDATE SET
    ContextJson = excluded.ContextJson,
    UpdatedUtc = CURRENT_TIMESTAMP;','2026-05-17 16:01:31','Execution Context Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('ExecutionContext_SelectByName','ExecutionContext select-by-name compatibility query.','SELECT ContextId,
       ContextName,
       ContextJson,
       CreatedUtc,
       UpdatedUtc
FROM ExecutionContext
WHERE ContextName = $ContextName;','2026-05-17 16:01:31','Execution Context Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('ExecutionContext_FindByName','ExecutionContext select-by-name compatibility query.','SELECT ContextId,
       ContextName,
       ContextJson,
       CreatedUtc,
       UpdatedUtc
FROM ExecutionContext
WHERE ContextName = $ContextName;','2026-05-17 16:01:31','Execution Context Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('ExecutionContext_GetByName','ExecutionContext select-by-name compatibility query.','SELECT ContextId,
       ContextName,
       ContextJson,
       CreatedUtc,
       UpdatedUtc
FROM ExecutionContext
WHERE ContextName = $ContextName;','2026-05-17 16:01:31','Execution Context Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('EngineContext_SelectByName','EngineContext select-by-name compatibility query.','SELECT ContextId,
       ContextName,
       ContextJson,
       CreatedUtc,
       UpdatedUtc
FROM ExecutionContext
WHERE ContextName = $ContextName;','2026-05-17 16:01:31','Execution Context Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('EngineContext_FindByName','EngineContext select-by-name compatibility query.','SELECT ContextId,
       ContextName,
       ContextJson,
       CreatedUtc,
       UpdatedUtc
FROM ExecutionContext
WHERE ContextName = $ContextName;','2026-05-17 16:01:31','Execution Context Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('EngineContext_GetByName','EngineContext select-by-name compatibility query.','SELECT ContextId,
       ContextName,
       ContextJson,
       CreatedUtc,
       UpdatedUtc
FROM ExecutionContext
WHERE ContextName = $ContextName;','2026-05-17 16:01:31','Execution Context Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('ExecutionContext_SelectById','ExecutionContext select-by-id compatibility query.','SELECT ContextId,
       ContextName,
       ContextJson,
       CreatedUtc,
       UpdatedUtc
FROM ExecutionContext
WHERE ContextId = $ContextId;','2026-05-17 16:01:31','Execution Context Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('EngineContext_SelectById','EngineContext select-by-id compatibility query.','SELECT ContextId,
       ContextName,
       ContextJson,
       CreatedUtc,
       UpdatedUtc
FROM ExecutionContext
WHERE ContextId = $ContextId;','2026-05-17 16:01:31','Execution Context Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('ExecutionContext_SelectAll','ExecutionContext list compatibility query.','SELECT ContextId,
       ContextName,
       ContextJson,
       CreatedUtc,
       UpdatedUtc
FROM ExecutionContext
ORDER BY ContextName ASC;','2026-05-17 16:01:31','Execution Context Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('ExecutionContext_List','ExecutionContext list compatibility query.','SELECT ContextId,
       ContextName,
       ContextJson,
       CreatedUtc,
       UpdatedUtc
FROM ExecutionContext
ORDER BY ContextName ASC;','2026-05-17 16:01:31','Execution Context Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('EngineContext_SelectAll','EngineContext list compatibility query.','SELECT ContextId,
       ContextName,
       ContextJson,
       CreatedUtc,
       UpdatedUtc
FROM ExecutionContext
ORDER BY ContextName ASC;','2026-05-17 16:01:31','Execution Context Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('EngineContext_List','EngineContext list compatibility query.','SELECT ContextId,
       ContextName,
       ContextJson,
       CreatedUtc,
       UpdatedUtc
FROM ExecutionContext
ORDER BY ContextName ASC;','2026-05-17 16:01:31','Execution Context Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('ExecutionContext_DeleteByName','ExecutionContext delete-by-name compatibility query.','DELETE FROM ExecutionContext
WHERE ContextName = $ContextName;','2026-05-17 16:01:31','Execution Context Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('ExecutionContext_Delete','ExecutionContext delete-by-name compatibility query.','DELETE FROM ExecutionContext
WHERE ContextName = $ContextName;','2026-05-17 16:01:31','Execution Context Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('ExecutionContext_DeleteById','ExecutionContext delete-by-id compatibility query.','DELETE FROM ExecutionContext
WHERE ContextId = $ContextId;','2026-05-17 16:01:31','Execution Context Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('EngineContext_DeleteByName','EngineContext delete-by-name compatibility query.','DELETE FROM ExecutionContext
WHERE ContextName = $ContextName;','2026-05-17 16:01:31','Execution Context Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('EngineContext_Delete','EngineContext delete-by-name compatibility query.','DELETE FROM ExecutionContext
WHERE ContextName = $ContextName;','2026-05-17 16:01:31','Execution Context Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('EngineContext_DeleteById','EngineContext delete-by-id compatibility query.','DELETE FROM ExecutionContext
WHERE ContextId = $ContextId;','2026-05-17 16:01:31','Execution Context Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('CommandDefinition_InsertOrReplace','Inserts or updates a command definition by command name.','INSERT INTO CommandDefinition (
    CommandId,
    CommandName,
    DisplayName,
    Description,
    Category,
    Version,
    HandlerKey,
    HandlerType,
    InputJson,
    OutputJson,
    MetadataJson,
    IsEnabled
)
VALUES (
    $CommandId,
    $CommandName,
    $DisplayName,
    $Description,
    $Category,
    $Version,
    $HandlerKey,
    $HandlerType,
    $InputJson,
    $OutputJson,
    $MetadataJson,
    $IsEnabled
)
ON CONFLICT(CommandName) DO UPDATE SET
    DisplayName = excluded.DisplayName,
    Description = excluded.Description,
    Category = excluded.Category,
    Version = excluded.Version,
    HandlerKey = excluded.HandlerKey,
    HandlerType = excluded.HandlerType,
    InputJson = excluded.InputJson,
    OutputJson = excluded.OutputJson,
    MetadataJson = excluded.MetadataJson,
    IsEnabled = excluded.IsEnabled,
    UpdatedUtc = CURRENT_TIMESTAMP;','2026-05-17 16:01:31','Command Registration',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('CommandDefinition_Upsert','Compatibility alias for CommandDefinition_InsertOrReplace.','INSERT INTO CommandDefinition (
    CommandId,
    CommandName,
    DisplayName,
    Description,
    Category,
    Version,
    HandlerKey,
    HandlerType,
    InputJson,
    OutputJson,
    MetadataJson,
    IsEnabled
)
VALUES (
    $CommandId,
    $CommandName,
    $DisplayName,
    $Description,
    $Category,
    $Version,
    $HandlerKey,
    $HandlerType,
    $InputJson,
    $OutputJson,
    $MetadataJson,
    $IsEnabled
)
ON CONFLICT(CommandName) DO UPDATE SET
    DisplayName = excluded.DisplayName,
    Description = excluded.Description,
    Category = excluded.Category,
    Version = excluded.Version,
    HandlerKey = excluded.HandlerKey,
    HandlerType = excluded.HandlerType,
    InputJson = excluded.InputJson,
    OutputJson = excluded.OutputJson,
    MetadataJson = excluded.MetadataJson,
    IsEnabled = excluded.IsEnabled,
    UpdatedUtc = CURRENT_TIMESTAMP;','2026-05-17 16:01:31','Command Registration',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('CommandDefinition_SelectByName','Selects a command definition by command name.','SELECT CommandId,
       CommandName,
       DisplayName,
       Description,
       Category,
       Version,
       HandlerKey,
       HandlerType,
       InputJson,
       OutputJson,
       MetadataJson,
       IsEnabled,
       CreatedUtc,
       UpdatedUtc
FROM CommandDefinition
WHERE CommandName = $CommandName;','2026-05-17 16:01:31','Command Registration',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('CommandDefinition_FindByName','Compatibility alias for CommandDefinition_SelectByName.','SELECT CommandId,
       CommandName,
       DisplayName,
       Description,
       Category,
       Version,
       HandlerKey,
       HandlerType,
       InputJson,
       OutputJson,
       MetadataJson,
       IsEnabled,
       CreatedUtc,
       UpdatedUtc
FROM CommandDefinition
WHERE CommandName = $CommandName;','2026-05-17 16:01:31','Command Registration',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('CommandDefinition_SelectAll','Lists all command definitions ordered for UI display.','SELECT CommandId,
       CommandName,
       DisplayName,
       Description,
       Category,
       Version,
       HandlerKey,
       HandlerType,
       InputJson,
       OutputJson,
       MetadataJson,
       IsEnabled,
       CreatedUtc,
       UpdatedUtc
FROM CommandDefinition
ORDER BY Category ASC,
         CommandName ASC;','2026-05-17 16:01:31','Command Registration',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('CommandDefinition_List','Compatibility alias for CommandDefinition_SelectAll.','SELECT CommandId,
       CommandName,
       DisplayName,
       Description,
       Category,
       Version,
       HandlerKey,
       HandlerType,
       InputJson,
       OutputJson,
       MetadataJson,
       IsEnabled,
       CreatedUtc,
       UpdatedUtc
FROM CommandDefinition
ORDER BY Category ASC,
         CommandName ASC;','2026-05-17 16:01:31','Command Registration',1,'2026-05-17 16:01:31');
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
    UpdatedUtc = CURRENT_TIMESTAMP;','2026-05-17 16:01:31','Command Registration',1,'2026-05-17 16:01:31');
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
         ParameterName ASC;','2026-05-17 16:01:31','Command Registration',1,'2026-05-17 16:01:31');
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
    UpdatedUtc = CURRENT_TIMESTAMP;','2026-05-17 16:01:31','Workflow Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('WorkflowDefinition_Upsert','Compatibility alias for WorkflowDefinition_InsertOrReplace.','INSERT INTO WorkflowDefinition (
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
    UpdatedUtc = CURRENT_TIMESTAMP;','2026-05-17 16:01:31','Workflow Foundation',1,'2026-05-17 16:01:31');
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
         Version DESC;','2026-05-17 16:01:31','Workflow Foundation',1,'2026-05-17 16:01:31');
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
  AND Version = $Version;','2026-05-17 16:01:31','Workflow Foundation',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('WorkflowDefinition_FindByNameAndVersion','Compatibility alias for WorkflowDefinition_SelectByNameAndVersion.','SELECT WorkflowId,
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
  AND Version = $Version;','2026-05-17 16:01:31','Workflow Foundation',1,'2026-05-17 16:01:31');
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
    IsEnabled = excluded.IsEnabled;','2026-05-17 16:01:31','Workflow Foundation',1,'2026-05-17 16:01:31');
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
ORDER BY StepOrder ASC;','2026-05-17 16:01:31','Workflow Foundation',1,'2026-05-17 16:01:31');
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
);','2026-05-17 16:01:31','Execution History',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('ExecutionHistory_Complete','Completes an execution history record by execution id.','UPDATE ExecutionHistory
SET Status = $Status,
    CompletedUtc = $CompletedUtc,
    OutputJson = $OutputJson,
    ErrorMessage = $ErrorMessage,
    MetadataJson = $MetadataJson
WHERE ExecutionId = $ExecutionId;','2026-05-17 16:01:31','Execution History',1,'2026-05-17 16:01:31');
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
LIMIT $Limit;','2026-05-17 16:01:31','Execution History',1,'2026-05-17 16:01:31');
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
);','2026-05-17 16:01:31','Execution History',1,'2026-05-17 16:01:31');
INSERT INTO "SqlQuery" VALUES('ExecutionStepHistory_Complete','Completes one execution step history record.','UPDATE ExecutionStepHistory
SET Status = $Status,
    CompletedUtc = $CompletedUtc,
    OutputJson = $OutputJson,
    ErrorMessage = $ErrorMessage,
    MetadataJson = $MetadataJson
WHERE ExecutionStepId = $ExecutionStepId;','2026-05-17 16:01:31','Execution History',1,'2026-05-17 16:01:31');
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
ORDER BY StepOrder ASC;','2026-05-17 16:01:31','Execution History',1,'2026-05-17 16:01:31');
CREATE TABLE WorkflowDefinition (
    WorkflowId   TEXT    PRIMARY KEY,
    WorkflowName TEXT    NOT NULL,
    Description  TEXT    NOT NULL DEFAULT '',
    Version      INTEGER NOT NULL DEFAULT 1,
    MetadataJson TEXT    NOT NULL DEFAULT '{}',
    IsEnabled    INTEGER NOT NULL DEFAULT 1,
    CreatedUtc   TEXT    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedUtc   TEXT    NULL,
    DisplayName  TEXT    NOT NULL DEFAULT '',
    UNIQUE (WorkflowName, Version)
);
CREATE TABLE WorkflowStep (
    WorkflowStepId TEXT    PRIMARY KEY,
    WorkflowId     TEXT    NOT NULL,
    StepOrder      INTEGER NOT NULL,
    CommandId      TEXT    NOT NULL,
    InputMapJson   TEXT    NOT NULL DEFAULT '{}',
    MetadataJson   TEXT    NOT NULL DEFAULT '{}',
    IsEnabled      INTEGER NOT NULL DEFAULT 1,
    StepName       TEXT    NOT NULL DEFAULT '',
    FOREIGN KEY (WorkflowId) REFERENCES WorkflowDefinition (WorkflowId) ON DELETE CASCADE,
    FOREIGN KEY (CommandId) REFERENCES CommandDefinition (CommandId) ON DELETE RESTRICT,
    UNIQUE (WorkflowId, StepOrder)
);
DELETE FROM "sqlite_sequence";
COMMIT;