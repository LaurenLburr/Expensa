using Microsoft.Data.Sqlite;
using Xunit;

namespace Codex.Data.SQLiteEngine.Tests;

public sealed class SqliteEngineSharedConnectionTests
{
    [Fact]
    public void SharedConnectionMode_UsesSameConnectionAcrossCommands()
    {
        using SqliteEngineInMemoryTestDatabase database = new();
        using SqliteEngine engine = new(
            database.ConnectionString,
            new SqliteEngineOptions
            {
                UseSingleSharedConnection = true
            });

        engine.ExecuteNonQuery("INSERT INTO [Sample] ([Name], [Amount]) VALUES ('one', 1.0);");
        engine.ExecuteNonQuery("INSERT INTO [Sample] ([Name], [Amount]) VALUES ('two', 2.0);");

        long? count = engine.ExecuteScalar<long>("SELECT COUNT(*) FROM [Sample];");

        Assert.Equal(2, count);
    }

    [Fact]
    public void SharedConnectionMode_Dispose_ThrowsOnFurtherUse()
    {
        using SqliteEngineInMemoryTestDatabase database = new();
        SqliteEngine engine = new(
            database.ConnectionString,
            new SqliteEngineOptions
            {
                UseSingleSharedConnection = true
            });

        engine.Dispose();

        Assert.Throws<ObjectDisposedException>(() =>
            engine.ExecuteScalar<long>("SELECT COUNT(*) FROM [Sample];"));
    }

    [Fact]
    public void SharedConnectionMode_TransactionCommit_PersistsChanges()
    {
        using SqliteEngineInMemoryTestDatabase database = new();
        using SqliteEngine engine = new(
            database.ConnectionString,
            new SqliteEngineOptions
            {
                UseSingleSharedConnection = true
            });

        using ISqliteTransactionScope tx = engine.BeginTransaction();

        tx.ExecuteNonQuery(
            "INSERT INTO [Sample] ([Name], [Amount]) VALUES (@Name, @Amount);",
            new[]
            {
                new SqliteParameter("@Name", "shared-tx"),
                new SqliteParameter("@Amount", 3.25m)
            });

        tx.Commit();

        long? count = engine.ExecuteScalar<long>("SELECT COUNT(*) FROM [Sample];");

        Assert.Equal(1, count);
    }

    [Fact]
    public void SharedConnectionMode_TransactionRollback_DiscardsChanges()
    {
        using SqliteEngineInMemoryTestDatabase database = new();
        using SqliteEngine engine = new(
            database.ConnectionString,
            new SqliteEngineOptions
            {
                UseSingleSharedConnection = true
            });

        using (ISqliteTransactionScope tx = engine.BeginTransaction())
        {
            tx.ExecuteNonQuery(
                "INSERT INTO [Sample] ([Name], [Amount]) VALUES (@Name, @Amount);",
                new[]
                {
                    new SqliteParameter("@Name", "shared-rollback"),
                    new SqliteParameter("@Amount", 4.5m)
                });
        }

        long? count = engine.ExecuteScalar<long>("SELECT COUNT(*) FROM [Sample];");

        Assert.Equal(0, count);
    }

    [Fact]
    public void SharedConnectionMode_QueryNameStillFlowsInEvents()
    {
        using SqliteEngineInMemoryTestDatabase database = new();
        using SqliteEngine engine = new(
            database.ConnectionString,
            new SqliteEngineOptions
            {
                UseSingleSharedConnection = true
            });

        SqliteCommandEventArgs? executedArgs = null;
        engine.CommandExecuted += (_, e) => executedArgs = e;

        engine.ExecuteNonQuery(
            "INSERT INTO [Sample] ([Name], [Amount]) VALUES ('named-shared', 8.0);",
            queryName: "Sample.SharedInsert");

        Assert.NotNull(executedArgs);
        Assert.Equal("Sample.SharedInsert", executedArgs!.QueryName);
        Assert.False(executedArgs.IsInTransaction);
    }
}
