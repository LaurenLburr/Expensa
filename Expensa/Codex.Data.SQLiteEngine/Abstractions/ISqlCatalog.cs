namespace Codex.Data.SQLiteEngine.Abstractions;

public interface ISqlCatalog
{
    string GetSql(string name);
    string GetSql(string name, ISqliteTransactionScope tx);
}
