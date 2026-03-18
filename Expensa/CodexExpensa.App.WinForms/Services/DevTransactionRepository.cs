using CodexExpensa.Core.Domain.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodexExpensa.App.WinForms.Services;

public sealed class DevTransactionRepository : ITransactionRepository
{
    private readonly List<Transaction> _transactions = new();

    private int _nextId = 1;

    public IReadOnlyList<Transaction> GetByAccountId(string accountId)
    {
        return _transactions
            .Where(t => t.AccountId == accountId)
            .ToList();
    }

    public void Add(Transaction txn)
    {
        if (txn is null)
            throw new ArgumentNullException(nameof(txn));

        Transaction stored = new()
        {
            AccountId = txn.AccountId,
            PayeeId = txn.PayeeId,
            Status = txn.Status,
            Amount = txn.Amount,
            StartDate = txn.StartDate,
            ConfirmationNumber = txn.ConfirmationNumber,
            Note = txn.Note
        };

        stored.SetTransactionId(_nextId++);

        _transactions.Add(stored);

        txn.SetTransactionId(stored.TransactionId);
    }

    public void Update(Transaction txn)
    {
        if (txn is null)
            throw new ArgumentNullException(nameof(txn));

        int index = _transactions.FindIndex(t => t.TransactionId == txn.TransactionId);

        if (index < 0)
            return;

        Transaction stored = new()
        {
            AccountId = txn.AccountId,
            PayeeId = txn.PayeeId,
            Status = txn.Status,
            Amount = txn.Amount,
            StartDate = txn.StartDate,
            ConfirmationNumber = txn.ConfirmationNumber,
            Note = txn.Note
        };

        stored.SetTransactionId(txn.TransactionId);

        _transactions[index] = stored;
    }

    public void Delete(int transactionId)
    {
        _transactions.RemoveAll(t => t.TransactionId == transactionId);
    }
}