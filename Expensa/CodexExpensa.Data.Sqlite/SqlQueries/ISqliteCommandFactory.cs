using Microsoft.Data.Sqlite;

namespace CodexExpensa.Data.Sqlite.SqlQueries;

public interface ISqliteCommandFactory
{
    SqliteCommand Create(string queryName, SqliteTransaction? transaction = null);
}