namespace CodexExpensa.Core.Abstractions;

/// <summary>
/// Simple status row for schema migrations.
/// </summary>
public sealed class MigrationStatusRow
{
    public required string MigrationId { get; init; }
    public required string Status { get; init; }
}