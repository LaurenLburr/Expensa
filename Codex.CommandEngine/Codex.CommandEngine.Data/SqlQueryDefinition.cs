namespace Codex.CommandEngine.Data;

public sealed record SqlQueryDefinition(
    string QueryName,
    string Description,
    string SqlText,
    string Category,
    bool IsActive = true);
