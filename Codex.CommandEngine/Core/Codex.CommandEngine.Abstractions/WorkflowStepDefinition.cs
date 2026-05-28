namespace Codex.CommandEngine.Abstractions;

public sealed record WorkflowStepDefinition(
    string WorkflowStepId,
    string WorkflowId,
    int StepOrder,
    string StepName,
    string CommandId,
    string InputMapJson,
    string MetadataJson,
    bool IsEnabled);
