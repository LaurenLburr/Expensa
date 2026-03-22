namespace Codex.Data.SQLiteEngine.Abstractions;

public interface ICommandLogger
{
    void Log(string action, string? details);
    void Log(string action, string? details, ISqliteTransactionScope tx);
}
