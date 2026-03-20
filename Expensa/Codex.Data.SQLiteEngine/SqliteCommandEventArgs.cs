namespace Codex.Data.SQLiteEngine;

public sealed class SqliteCommandEventArgs : EventArgs
{
    public string? QueryName { get; init; }

    public required string Sql { get; init; }

    public required IReadOnlyList<SqliteParameterSnapshot> Parameters { get; init; }

    public required DateTime StartedUtc { get; init; }

    public TimeSpan Duration { get; init; }

    public int? RowsAffected { get; init; }

    public object? ScalarResult { get; init; }

    public Exception? Exception { get; init; }

    public bool IsInTransaction { get; init; }

    public Guid? TransactionId { get; init; }
}
