using System.Collections.Generic;

namespace CodexExpensa.Core.Domain.Transactions;

public interface ITransactionRepository
{
    IReadOnlyList<Transaction> GetByAccountId(string accountId);

    void Add(Transaction txn);

    void Update(Transaction txn);

    void Delete(int transactionId);
}