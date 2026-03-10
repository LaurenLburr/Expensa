namespace CodexExpensa.Core.Domain.SqlQueries;

public sealed class SqlQueryDefinition
{
    public required int QueryIndex { get; init; }

    public required string QueryName { get; init; }

    public required string Description { get; init; }

    public required string SqlText { get; init; }

    public required string Fingerprint { get; init; }

    public bool IsActive { get; init; }
}