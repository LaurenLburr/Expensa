namespace Codex.CommandEngine.Core;

public sealed class CommandExecutionRequest
{
    public required string CommandName { get; init; }

    public string CorrelationId { get; init; } = Guid.NewGuid().ToString("N");

    public string ContextJson { get; init; } = "{}";

    public IReadOnlyDictionary<string, object?> Parameters { get; init; } =
        new Dictionary<string, object?>();
}
