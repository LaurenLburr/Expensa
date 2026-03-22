namespace Codex.Data.SQLiteEngine.Configuration;

public enum TransactionParticipationMode
{
    UseConnectionOnly = 0,
    UseTransactionWhenAvailable = 1,
    RequireTransaction = 2
}
