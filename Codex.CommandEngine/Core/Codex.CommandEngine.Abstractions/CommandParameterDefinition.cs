namespace Codex.CommandEngine.Abstractions;

public sealed class CommandParameterDefinition
{
    public required string ParameterName { get; init; }

    public required string ParameterType { get; init; }

    public bool IsRequired { get; init; }

    public string DefaultValue { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public int SortOrder { get; init; }
}
