using CodexExpensa.Core.Domain.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodexExpensa.Core.Infrastructure;

public sealed class DevTransactionRepository : ITransactionRepository
{
    private readonly List<Transaction> _transactions = new();

    private int _nextId = 1;

    public IReadOnlyList<Transaction> GetByAccountId(string accountId)
    {
        return _transactions
            .Where(t => string.Equals(t.AccountId, accountId, StringComparison.Ordinal))
            .ToList();
    }

    public void Add(Transaction txn)
    {
        if (txn is null)
            throw new ArgumentNullException(nameof(txn));

        txn.SetTransactionId(_nextId++);
        _transactions.Add(txn);
    }

    public void ChangeStatus(
        int transactionId,
        TransactionStatus newStatus,
        TransactionChangeReason reasonCode,
        string? reasonText,
        string source)
    {
        if (transactionId <= 0)
            throw new ArgumentException("transactionId must be > 0.", nameof(transactionId));

        if (string.IsNullOrWhiteSpace(source))
            throw new ArgumentException("source is required.", nameof(source));

        Transaction txn = _transactions
            .FirstOrDefault(t => t.TransactionId == transactionId)
            ?? throw new InvalidOperationException($"Transaction {transactionId} was not found.");

        txn.Status = newStatus;
    }

    public void Update(Transaction txn)
    {
        if (txn is null)
            throw new ArgumentNullException(nameof(txn));

        int index = _transactions.FindIndex(t => t.TransactionId == txn.TransactionId);

        if (index < 0)
            throw new InvalidOperationException("Transaction not found.");

        _transactions[index] = txn;
    }

    public void Delete(int transactionId)
    {
        _transactions.RemoveAll(t => t.TransactionId == transactionId);
    }
}
