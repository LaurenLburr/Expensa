using System;

namespace CodexExpensa.Core.Domain.Transactions;

public sealed class Transaction
{
    public int TransactionId { get; init; }

    public required string AccountId { get; init; }

    public string? PayeeId { get; init; }

    public required TransactionStatus Status { get; init; }

    public required decimal Amount { get; init; }

    public required DateTime StartDate { get; init; }

    public string? Confirm { get; init; }

    public string? Note { get; init; }
}