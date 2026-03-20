using System.Collections.Generic;

namespace CodexExpensa.Core.Domain.Transactions;

public interface ITransactionRepository
{
    IReadOnlyList<Transaction> GetByAccountId(string accountId);

    void Add(Transaction txn);

    void Update(Transaction txn);

    void ChangeStatus(
        int transactionId,
        TransactionStatus newStatus,
        TransactionChangeReason reasonCode,
        string? reasonText,
        string source);

    IReadOnlyList<TxnStatusLog> GetStatusHistory(int transactionId);

    void Delete(int transactionId);
}
