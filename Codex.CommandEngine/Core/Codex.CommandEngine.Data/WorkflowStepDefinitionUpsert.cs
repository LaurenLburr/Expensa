namespace Codex.CommandEngine.Data;

public sealed record WorkflowStepDefinitionUpsert(
    string WorkflowStepDefinitionId,
    string WorkflowDefinitionId,
    int StepOrder,
    string StepName,
    string CommandDefinitionId,
    string InputMapJson,
    string MetadataJson,
    bool IsEnabled);
