namespace CodexExpensa.Core.Domain.Transactions;

public enum TransactionChangeReason
{
    ManualCorrection = 0,
    StatusReset = 1,
    MarkedInvalid = 2,
    ImportedCorrection = 3,
    ReconciledAdjustment = 4
}
