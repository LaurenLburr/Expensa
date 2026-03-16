namespace CodexExpensa.Core.Domain.Transactions;

public enum TransactionStatus
{
    Projected = 0,
    Outstanding = 1,
    Cleared = 2,
    Invalid = 3
}