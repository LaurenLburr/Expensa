using Microsoft.Data.Sqlite;
using System.Text;

namespace Codex.CommandEngine.App;

public static class DbSchemaFormatter
{
    public static string BuildSchemaText(string databasePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);

        using SqliteConnection connection = new($"Data Source={databasePath}");
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            SELECT name, sql
            FROM sqlite_master
            WHERE type = 'table'
            ORDER BY name;
            """;

        using SqliteDataReader reader = command.ExecuteReader();

        StringBuilder builder = new();

        while (reader.Read())
        {
            builder.AppendLine($"TABLE: {reader.GetString(0)}");
            builder.AppendLine(reader.GetString(1));
            builder.AppendLine();
        }

        return builder.ToString();
    }
}
