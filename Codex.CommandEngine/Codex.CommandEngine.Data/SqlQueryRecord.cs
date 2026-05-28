namespace Codex.CommandEngine.Data;

public sealed record SqlQueryRecord(
    string QueryName,
    string Description,
    string SqlText,
    string Category,
    bool IsActive,
    string CreatedUtc,
    string? UpdatedUtc);
