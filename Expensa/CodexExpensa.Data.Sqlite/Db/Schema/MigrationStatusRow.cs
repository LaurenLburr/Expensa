namespace CodexExpensa.Data.Sqlite.Db.Schema;

public sealed class MigrationStatusRow
{
    public required string MigrationId { get; init; }
    public required string Status { get; init; }
}