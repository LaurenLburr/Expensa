--
-- Codex.CommandEngine repository SQL catalog patch
-- Apply to:
-- D:\Git\CodexExpensa\Codex.CommandEngine\DevDatabase\CommandEngine.dev.db
--
-- Goal:
-- Repositories fetch every operational SQL statement from SqlQuery.
--

BEGIN TRANSACTION;

INSERT OR REPLACE INTO SqlQuery
(
    QueryName,
    Description,
    SqlText,
    Category,
    IsActive,
    UpdatedUtc
)
VALUES
(
    'AiProvider_InsertOrReplace',
    'Inserts or updates an AI provider by provider name.',
    'INSERT INTO AiProvider (
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
    UpdatedUtc = CURRENT_TIMESTAMP;',
    'AI Provider Foundation',
    1,
    CURRENT_TIMESTAMP
),
(
    'AiProvider_SelectByName',
    'Selects an AI provider by provider name.',
    'SELECT AiProviderId,
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
WHERE ProviderName = $ProviderName;',
    'AI Provider Foundation',
    1,
    CURRENT_TIMESTAMP
),
(
    'AiProvider_SelectAll',
    'Lists AI providers ordered by provider kind and provider name.',
    'SELECT AiProviderId,
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
         ProviderName ASC;',
    'AI Provider Foundation',
    1,
    CURRENT_TIMESTAMP
),
(
    'AiProviderCapability_InsertOrReplace',
    'Inserts or updates an AI provider capability.',
    'INSERT INTO AiProviderCapability (
    AiProviderCapabilityId,
    AiProviderId,
    CapabilityName,
    Description,
    MetadataJson,
    IsEnabled,
    CreatedUtc
)
VALUES (
    $AiProviderCapabilityId,
    $AiProviderId,
    $CapabilityName,
    $Description,
    $MetadataJson,
    $IsEnabled,
    CURRENT_TIMESTAMP
)
ON CONFLICT(AiProviderId, CapabilityName) DO UPDATE SET
    Description = excluded.Description,
    MetadataJson = excluded.MetadataJson,
    IsEnabled = excluded.IsEnabled,
    UpdatedUtc = CURRENT_TIMESTAMP;',
    'AI Provider Foundation',
    1,
    CURRENT_TIMESTAMP
),
(
    'AiProviderCapability_SelectByProvider',
    'Lists AI provider capabilities for one provider.',
    'SELECT AiProviderCapabilityId,
       AiProviderId,
       CapabilityName,
       Description,
       MetadataJson,
       IsEnabled,
       CreatedUtc,
       UpdatedUtc
FROM AiProviderCapability
WHERE AiProviderId = $AiProviderId
ORDER BY CapabilityName ASC;',
    'AI Provider Foundation',
    1,
    CURRENT_TIMESTAMP
),
(
    'CommandDefinition_InsertOrReplace',
    'Inserts or updates a command definition by command name.',
    'INSERT INTO CommandDefinition (
    CommandId,
    CommandName,
    DisplayName,
    Description,
    Category,
    Version,
    HandlerType,
    IsEnabled
)
VALUES (
    $CommandId,
    $CommandName,
    $DisplayName,
    $Description,
    $Category,
    $Version,
    $HandlerType,
    $IsEnabled
)
ON CONFLICT(CommandName) DO UPDATE SET
    DisplayName = excluded.DisplayName,
    Description = excluded.Description,
    Category = excluded.Category,
    Version = excluded.Version,
    HandlerType = excluded.HandlerType,
    IsEnabled = excluded.IsEnabled,
    UpdatedUtc = CURRENT_TIMESTAMP;',
    'Command Registration',
    1,
    CURRENT_TIMESTAMP
),
(
    'CommandDefinition_SelectByName',
    'Selects a command definition by command name.',
    'SELECT CommandId,
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
WHERE CommandName = $CommandName;',
    'Command Registration',
    1,
    CURRENT_TIMESTAMP
),
(
    'CommandDefinition_SelectAll',
    'Lists all command definitions ordered by category and command name.',
    'SELECT CommandId,
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
         CommandName ASC;',
    'Command Registration',
    1,
    CURRENT_TIMESTAMP
),
(
    'CommandParameterDefinition_InsertOrReplace',
    'Inserts or updates a command parameter definition.',
    'INSERT INTO CommandParameterDefinition (
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
    UpdatedUtc = CURRENT_TIMESTAMP;',
    'Command Registration',
    1,
    CURRENT_TIMESTAMP
),
(
    'CommandParameterDefinition_SelectByCommand',
    'Lists parameter definitions for a command definition.',
    'SELECT CommandParameterDefinitionId,
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
         ParameterName ASC;',
    'Command Registration',
    1,
    CURRENT_TIMESTAMP
),
(
    'EngineContext_InsertOrReplace',
    'Inserts or updates an execution context by context name.',
    'INSERT INTO ExecutionContext (
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
    UpdatedUtc = CURRENT_TIMESTAMP;',
    'Context System Foundation',
    1,
    CURRENT_TIMESTAMP
),
(
    'EngineContext_SelectByName',
    'Selects an execution context by context name.',
    'SELECT ContextId,
       ContextName,
       Scope,
       Description,
       ContextJson,
       MetadataJson,
       IsEnabled,
       CreatedUtc,
       UpdatedUtc
FROM ExecutionContext
WHERE ContextName = $ContextName;',
    'Context System Foundation',
    1,
    CURRENT_TIMESTAMP
),
(
    'EngineContext_SelectAll',
    'Lists execution contexts ordered by scope and context name.',
    'SELECT ContextId,
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
         ContextName ASC;',
    'Context System Foundation',
    1,
    CURRENT_TIMESTAMP
),
(
    'WorkflowDefinition_InsertOrReplace',
    'Inserts or updates a workflow definition by workflow name and version.',
    'INSERT INTO WorkflowDefinition (
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
    UpdatedUtc = CURRENT_TIMESTAMP;',
    'Workflow Foundation',
    1,
    CURRENT_TIMESTAMP
),
(
    'WorkflowDefinition_SelectByNameAndVersion',
    'Selects a workflow definition by workflow name and version.',
    'SELECT WorkflowId,
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
  AND Version = $Version;',
    'Workflow Foundation',
    1,
    CURRENT_TIMESTAMP
),
(
    'WorkflowDefinition_SelectAll',
    'Lists workflow definitions ordered by workflow name and version.',
    'SELECT WorkflowId,
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
         Version DESC;',
    'Workflow Foundation',
    1,
    CURRENT_TIMESTAMP
),
(
    'WorkflowStep_InsertOrReplace',
    'Inserts or updates a workflow step by workflow and step order.',
    'INSERT INTO WorkflowStep (
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
    IsEnabled = excluded.IsEnabled;',
    'Workflow Foundation',
    1,
    CURRENT_TIMESTAMP
),
(
    'WorkflowStep_SelectByWorkflow',
    'Lists workflow steps for one workflow definition.',
    'SELECT WorkflowStepId,
       WorkflowId,
       StepOrder,
       StepName,
       CommandId,
       InputMapJson,
       MetadataJson,
       IsEnabled
FROM WorkflowStep
WHERE WorkflowId = $WorkflowId
ORDER BY StepOrder ASC;',
    'Workflow Foundation',
    1,
    CURRENT_TIMESTAMP
),
(
    'ExecutionHistory_Insert',
    'Inserts an execution history row.',
    'INSERT INTO ExecutionHistory (
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
);',
    'Execution History',
    1,
    CURRENT_TIMESTAMP
),
(
    'ExecutionHistory_Complete',
    'Completes an execution history row.',
    'UPDATE ExecutionHistory
SET Status = $Status,
    CompletedUtc = $CompletedUtc,
    OutputJson = $OutputJson,
    ErrorMessage = $ErrorMessage,
    MetadataJson = $MetadataJson
WHERE ExecutionId = $ExecutionId;',
    'Execution History',
    1,
    CURRENT_TIMESTAMP
),
(
    'ExecutionHistory_SelectById',
    'Selects one execution history row by id.',
    'SELECT ExecutionId,
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
WHERE ExecutionId = $ExecutionId;',
    'Execution History',
    1,
    CURRENT_TIMESTAMP
),
(
    'ExecutionHistory_SelectRecent',
    'Lists recent execution history rows.',
    'SELECT ExecutionId,
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
LIMIT $Limit;',
    'Execution History',
    1,
    CURRENT_TIMESTAMP
),
(
    'ExecutionStepHistory_Insert',
    'Inserts an execution step history row.',
    'INSERT INTO ExecutionStepHistory (
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
);',
    'Execution History',
    1,
    CURRENT_TIMESTAMP
),
(
    'ExecutionStepHistory_Complete',
    'Completes one execution step history row.',
    'UPDATE ExecutionStepHistory
SET Status = $Status,
    CompletedUtc = $CompletedUtc,
    OutputJson = $OutputJson,
    ErrorMessage = $ErrorMessage,
    MetadataJson = $MetadataJson
WHERE ExecutionStepId = $ExecutionStepId;',
    'Execution History',
    1,
    CURRENT_TIMESTAMP
),
(
    'ExecutionStepHistory_SelectByExecution',
    'Lists step history rows for one execution.',
    'SELECT ExecutionStepId,
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
ORDER BY StepOrder ASC;',
    'Execution History',
    1,
    CURRENT_TIMESTAMP
);

COMMIT;
