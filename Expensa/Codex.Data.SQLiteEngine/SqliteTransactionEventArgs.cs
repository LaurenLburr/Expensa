namespace Codex.Data.SQLiteEngine;

public sealed class SqliteTransactionEventArgs : EventArgs
{
    public required Guid TransactionId { get; init; }

    public required DateTime StartedUtc { get; init; }

    public TimeSpan Duration { get; init; }

    public Exception? Exception { get; init; }
}
