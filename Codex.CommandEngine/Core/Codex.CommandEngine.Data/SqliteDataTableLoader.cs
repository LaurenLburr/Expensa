using System.Data;
using Microsoft.Data.Sqlite;

namespace Codex.CommandEngine.Data;

public static class SqliteDataTableLoader
{
    public static DataTable Load(SqliteConnection connection, string sql)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentException.ThrowIfNullOrWhiteSpace(sql);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;

        return Load(command);
    }

    public static DataTable Load(SqliteCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        using SqliteDataReader reader = command.ExecuteReader();

        DataTable table = new();
        table.Load(reader);

        return table;
    }
}
