namespace Codex.CommandEngine.Data;

public sealed record CommandParameterDefinitionRecord(
    string CommandParameterDefinitionId,
    string CommandDefinitionId,
    string ParameterName,
    string ParameterType,
    bool IsRequired,
    string DefaultValue,
    string Description,
    int SortOrder,
    string CreatedUtc,
    string? UpdatedUtc);
