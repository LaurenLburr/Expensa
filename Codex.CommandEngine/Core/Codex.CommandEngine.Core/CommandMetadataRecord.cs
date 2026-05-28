namespace Codex.CommandEngine.Core;

public sealed class CommandMetadataRecord
{
    public required string CommandName { get; init; }

    public string DisplayName { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string ParameterTemplateJson { get; init; } = "{}";

    public string Notes { get; init; } = string.Empty;
}
