namespace CodexExpensa.Core.Abstractions;

public interface ISqlQueryProvider
{
    string GetSql(string queryName);
}