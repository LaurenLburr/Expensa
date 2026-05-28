namespace Codex.CommandEngine.Data;

public sealed record SqlCatalogRecord(
    string QueryName,
    string Description,
    string SqlText,
    string Category,
    bool IsActive);
