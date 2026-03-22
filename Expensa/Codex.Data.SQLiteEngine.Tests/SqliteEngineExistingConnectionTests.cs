using Microsoft.Data.Sqlite;
using System.Data;
using Xunit;

namespace Codex.Data.SQLiteEngine.Tests;

public sealed class SqliteEngineExistingConnectionTests
{
    [Fact]
    public void ExistingConnectionMode_UsesProvidedConnection()
    {
        using SqliteEngineInMemoryTestDatabase database = new();
        using SqliteConnection connection = new(database.ConnectionString);
        connection.Open();

        using SqliteEngine engine = new(connection);

        engine.ExecuteNonQuery("INSERT INTO [Sample] ([Name], [Amount]) VALUES ('existing', 9.0);");

        long? count = engine.ExecuteScalar<long>("SELECT COUNT(*) FROM [Sample];");

        Assert.Equal(1, count);
        Assert.Equal(System.Data.ConnectionState.Open, connection.State);
    }

    [Fact]
    public void ExistingConnectionMode_Dispose_DoesNotCloseExternalConnection_ByDefault()
    {
        using SqliteEngineInMemoryTestDatabase database = new();
        using SqliteConnection connection = new(database.ConnectionString);
        connection.Open();

        var engine = new SqliteEngine(connection);

        engine.Dispose();

        Assert.Equal(ConnectionState.Open, connection.State);
    }


    [Fact]
    public void ExistingConnectionMode_Dispose_CanOwnAndCloseConnection()
    {
        using SqliteEngineInMemoryTestDatabase database = new();
        using SqliteConnection connection = new(database.ConnectionString);
        connection.Open();

        var engine = new SqliteEngine(connection, ownsConnection: true);

        engine.Dispose();

        Assert.Equal(ConnectionState.Closed, connection.State);
    }


    [Fact]
    public void ExistingConnectionMode_WithQueryName_RaisesEventsWithQueryName()
    {
        using SqliteEngineInMemoryTestDatabase database = new();
        using SqliteConnection connection = new(database.ConnectionString);
        connection.Open();

        using SqliteEngine engine = new(connection);

        SqliteCommandEventArgs? executedArgs = null;
        engine.CommandExecuted += (_, e) => executedArgs = e;

        engine.ExecuteNonQuery(
            "INSERT INTO [Sample] ([Name], [Amount]) VALUES ('existing-named', 10.0);",
            queryName: "Sample.ExistingInsert");

        Assert.NotNull(executedArgs);
        Assert.Equal("Sample.ExistingInsert", executedArgs!.QueryName);
    }

    [Fact]
    public void ExistingConnectionMode_TransactionCommit_PersistsChanges()
    {
        using SqliteEngineInMemoryTestDatabase database = new();
        using SqliteConnection connection = new(database.ConnectionString);
        connection.Open();

        using SqliteEngine engine = new(connection);

        using ISqliteTransactionScope tx = engine.BeginTransaction();
        tx.ExecuteNonQuery(
            "INSERT INTO [Sample] ([Name], [Amount]) VALUES (@Name, @Amount);",
            new[]
            {
                new SqliteParameter("@Name", "existing-tx"),
                new SqliteParameter("@Amount", 11.5m)
            });
        tx.Commit();

        long? count = engine.ExecuteScalar<long>("SELECT COUNT(*) FROM [Sample];");

        Assert.Equal(1, count);
    }
}
