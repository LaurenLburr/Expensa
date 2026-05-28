namespace Codex.CommandEngine.Data;

public sealed record WorkflowStepDefinitionRecord(
    string WorkflowStepDefinitionId,
    string WorkflowDefinitionId,
    int StepOrder,
    string StepName,
    string CommandDefinitionId,
    string InputMapJson,
    string MetadataJson,
    bool IsEnabled);
