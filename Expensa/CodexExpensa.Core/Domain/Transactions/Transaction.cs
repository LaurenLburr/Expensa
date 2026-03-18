using System;

namespace CodexExpensa.Core.Domain.Transactions;

public sealed class Transaction
{
    public int TransactionId { get; private set; }

    public required string AccountId { get; init; }

    public string? PayeeId { get; init; }

    public TransactionStatus Status { get; set; }

    public required decimal Amount { get; init; }

    public required DateTime StartDate { get; init; }

    public string? ConfirmationNumber { get; set; }

    public string? Note { get; set; }

    public void SetTransactionId(int transactionId)
    {
        if (transactionId <= 0)
            throw new ArgumentOutOfRangeException(nameof(transactionId), "TransactionId must be greater than 0.");

        if (TransactionId > 0 && TransactionId != transactionId)
            throw new InvalidOperationException("TransactionId has already been set and cannot be changed.");

        TransactionId = transactionId;
    }
}