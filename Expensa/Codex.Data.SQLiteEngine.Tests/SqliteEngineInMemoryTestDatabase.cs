using Microsoft.Data.Sqlite;

namespace Codex.Data.SQLiteEngine.Tests;

internal sealed class SqliteEngineInMemoryTestDatabase : IDisposable
{
    private readonly SqliteConnection _keeperConnection;

    public string ConnectionString { get; }

    public SqliteEngineInMemoryTestDatabase()
    {
        string databaseName = $"sqlite-engine-tests-{Guid.NewGuid():N}";
        ConnectionString = $"Data Source={databaseName};Mode=Memory;Cache=Shared";

        _keeperConnection = new SqliteConnection(ConnectionString);
        _keeperConnection.Open();

        using SqliteCommand command = _keeperConnection.CreateCommand();
        command.CommandText =
            """
            CREATE TABLE [Sample]
            (
                [Id] INTEGER PRIMARY KEY AUTOINCREMENT,
                [Name] TEXT NOT NULL,
                [Amount] REAL NOT NULL
            );
            """;
        command.ExecuteNonQuery();
    }

    public void Dispose()
    {
        _keeperConnection.Dispose();
    }
}
