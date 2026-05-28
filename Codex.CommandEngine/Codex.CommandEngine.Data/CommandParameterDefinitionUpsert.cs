namespace Codex.CommandEngine.Data;

public sealed class CommandParameterDefinitionUpsert
{
    public string CommandParameterDefinitionId { get; init; } = Guid.NewGuid().ToString("N");

    public required string CommandDefinitionId { get; init; }

    public required string ParameterName { get; init; }

    public required string ParameterType { get; init; }

    public bool IsRequired { get; init; }

    public string DefaultValue { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public int SortOrder { get; init; }
}
