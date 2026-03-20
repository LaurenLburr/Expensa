using System;

namespace CodexExpensa.Core.Domain.Transactions;

public sealed class TxnStatusLog
{
    public int TxnStatusLogId { get; init; }

    public int TransactionId { get; init; }

    public string? OldStatus { get; init; }

    public required string NewStatus { get; init; }

    public required string ReasonCode { get; init; }

    public string? ReasonText { get; init; }

    public required DateTime ChangedUtc { get; init; }

    public string? ChangedBy { get; init; }

    public string? Source { get; init; }
}
