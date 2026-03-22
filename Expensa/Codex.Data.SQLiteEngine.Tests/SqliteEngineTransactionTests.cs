using Microsoft.Data.Sqlite;
using Xunit;

namespace Codex.Data.SQLiteEngine.Tests;

public sealed class SqliteEngineTransactionTests
{
    [Fact]
    public void BeginTransaction_Commit_PersistsChanges()
    {
        using SqliteEngineInMemoryTestDatabase database = new();
        SqliteEngine engine = new(database.ConnectionString);

        Guid? beganId = null;
        Guid? committedId = null;

        engine.TransactionBegan += (_, e) => beganId = e.TransactionId;
        engine.TransactionCommitted += (_, e) => committedId = e.TransactionId;

        using ISqliteTransactionScope tx = engine.BeginTransaction();

        tx.ExecuteNonQuery(
            "INSERT INTO [Sample] ([Name], [Amount]) VALUES (@Name, @Amount);",
            new[]
            {
                new SqliteParameter("@Name", "commit"),
                new SqliteParameter("@Amount", 1.25m)
            });

        tx.Commit();

        long? count = engine.ExecuteScalar<long>("SELECT COUNT(*) FROM [Sample];");

        Assert.Equal(1, count);
        Assert.True(beganId.HasValue);
        Assert.Equal(beganId, committedId);
    }

    [Fact]
    public void DisposeWithoutCommit_RollsBackChanges()
    {
        using SqliteEngineInMemoryTestDatabase database = new();
        SqliteEngine engine = new(database.ConnectionString);

        int rolledBackCount = 0;
        engine.TransactionRolledBack += (_, _) => rolledBackCount++;

        using (ISqliteTransactionScope tx = engine.BeginTransaction())
        {
            tx.ExecuteNonQuery(
                "INSERT INTO [Sample] ([Name], [Amount]) VALUES (@Name, @Amount);",
                new[]
                {
                    new SqliteParameter("@Name", "rollback"),
                    new SqliteParameter("@Amount", 2.50m)
                });
        }

        long? count = engine.ExecuteScalar<long>("SELECT COUNT(*) FROM [Sample];");

        Assert.Equal(0, count);
        Assert.Equal(1, rolledBackCount);
    }

    [Fact]
    public void Rollback_DiscardsChanges()
    {
        using SqliteEngineInMemoryTestDatabase database = new();
        SqliteEngine engine = new(database.ConnectionString);

        using ISqliteTransactionScope tx = engine.BeginTransaction();

        tx.ExecuteNonQuery(
            "INSERT INTO [Sample] ([Name], [Amount]) VALUES (@Name, @Amount);",
            new[]
            {
                new SqliteParameter("@Name", "manual-rollback"),
                new SqliteParameter("@Amount", 3.75m)
            });

        tx.Rollback();

        long? count = engine.ExecuteScalar<long>("SELECT COUNT(*) FROM [Sample];");

        Assert.Equal(0, count);
    }

    [Fact]
    public void TransactionCommands_RaiseCommandEventsWithTransactionMetadata()
    {
        using SqliteEngineInMemoryTestDatabase database = new();
        SqliteEngine engine = new(database.ConnectionString);

        SqliteCommandEventArgs? executedArgs = null;

        using ISqliteTransactionScope tx = engine.BeginTransaction();

        engine.CommandExecuted += (_, e) => executedArgs = e;

        tx.ExecuteNonQuery(
            "INSERT INTO [Sample] ([Name], [Amount]) VALUES (@Name, @Amount);",
            new[]
            {
                new SqliteParameter("@Name", "tx"),
                new SqliteParameter("@Amount", 10m)
            });

        tx.Commit();

        Assert.NotNull(executedArgs);
        Assert.True(executedArgs!.IsInTransaction);
        Assert.Equal(tx.TransactionId, executedArgs.TransactionId);
        Assert.Null(executedArgs.QueryName);
    }

    [Fact]
    public void TransactionCommands_WithQueryName_RaiseCommandEventsWithQueryName()
    {
        using SqliteEngineInMemoryTestDatabase database = new();
        SqliteEngine engine = new(database.ConnectionString);

        SqliteCommandEventArgs? executedArgs = null;

        using ISqliteTransactionScope tx = engine.BeginTransaction();

        engine.CommandExecuted += (_, e) => executedArgs = e;

        tx.ExecuteNonQuery(
            "INSERT INTO [Sample] ([Name], [Amount]) VALUES (@Name, @Amount);",
            new[]
            {
                new SqliteParameter("@Name", "tx-name"),
                new SqliteParameter("@Amount", 11m)
            },
            queryName: "Sample.TxInsert");

        tx.Commit();

        Assert.NotNull(executedArgs);
        Assert.True(executedArgs!.IsInTransaction);
        Assert.Equal(tx.TransactionId, executedArgs.TransactionId);
        Assert.Equal("Sample.TxInsert", executedArgs.QueryName);
    }

    [Fact]
    public void Commit_AfterCompletion_Throws()
    {
        using SqliteEngineInMemoryTestDatabase database = new();
        SqliteEngine engine = new(database.ConnectionString);

        using ISqliteTransactionScope tx = engine.BeginTransaction();
        tx.Commit();

        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => tx.Commit());
        Assert.Contains("already been completed", ex.Message);
    }

    [Fact]
    public void Rollback_AfterCommit_Throws()
    {
        using SqliteEngineInMemoryTestDatabase database = new();
        SqliteEngine engine = new(database.ConnectionString);

        using ISqliteTransactionScope tx = engine.BeginTransaction();
        tx.Commit();

        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => tx.Rollback());
        Assert.Contains("already been completed", ex.Message);
    }

    [Fact]
    public void TransactionCommandFailure_RaisesCommandFailed_WithTransactionMetadata()
    {
        using SqliteEngineInMemoryTestDatabase database = new();
        SqliteEngine engine = new(database.ConnectionString);

        SqliteCommandEventArgs? failedArgs = null;
        engine.CommandFailed += (_, e) => failedArgs = e;

        using ISqliteTransactionScope tx = engine.BeginTransaction();

        Assert.Throws<SqliteException>(() =>
            tx.ExecuteNonQuery("INSERT INTO [DoesNotExist] ([Name]) VALUES ('oops');"));

        Assert.NotNull(failedArgs);
        Assert.True(failedArgs!.IsInTransaction);
        Assert.Equal(tx.TransactionId, failedArgs.TransactionId);
        Assert.Null(failedArgs.QueryName);
    }

    [Fact]
    public void TransactionCommandFailure_WithQueryName_RaisesCommandFailed_WithQueryName()
    {
        using SqliteEngineInMemoryTestDatabase database = new();
        SqliteEngine engine = new(database.ConnectionString);

        SqliteCommandEventArgs? failedArgs = null;
        engine.CommandFailed += (_, e) => failedArgs = e;

        using ISqliteTransactionScope tx = engine.BeginTransaction();

        Assert.Throws<SqliteException>(() =>
            tx.ExecuteNonQuery(
                "INSERT INTO [DoesNotExist] ([Name]) VALUES ('oops');",
                queryName: "Sample.TxBadInsert"));

        Assert.NotNull(failedArgs);
        Assert.True(failedArgs!.IsInTransaction);
        Assert.Equal(tx.TransactionId, failedArgs.TransactionId);
        Assert.Equal("Sample.TxBadInsert", failedArgs.QueryName);
    }

    [Fact]
    public void ExecuteInTransaction_Action_CommitsWhenActionSucceeds()
    {
        using SqliteEngineInMemoryTestDatabase database = new();
        SqliteEngine engine = new(database.ConnectionString);

        engine.ExecuteInTransaction(tx =>
        {
            tx.ExecuteNonQuery(
                "INSERT INTO [Sample] ([Name], [Amount]) VALUES (@Name, @Amount);",
                new[]
                {
                    new SqliteParameter("@Name", "wrapper-action"),
                    new SqliteParameter("@Amount", 12.5m)
                });
        });

        long? count = engine.ExecuteScalar<long>("SELECT COUNT(*) FROM [Sample] WHERE [Name] = 'wrapper-action';");

        Assert.Equal(1, count);
    }

    [Fact]
    public void ExecuteInTransaction_Action_RollsBackWhenActionThrows()
    {
        using SqliteEngineInMemoryTestDatabase database = new();
        SqliteEngine engine = new(database.ConnectionString);

        int rolledBackCount = 0;
        engine.TransactionRolledBack += (_, _) => rolledBackCount++;

        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() =>
            engine.ExecuteInTransaction(tx =>
            {
                tx.ExecuteNonQuery(
                    "INSERT INTO [Sample] ([Name], [Amount]) VALUES (@Name, @Amount);",
                    new[]
                    {
                        new SqliteParameter("@Name", "wrapper-rollback"),
                        new SqliteParameter("@Amount", 13.5m)
                    });

                throw new InvalidOperationException("boom");
            }));

        long? count = engine.ExecuteScalar<long>("SELECT COUNT(*) FROM [Sample] WHERE [Name] = 'wrapper-rollback';");

        Assert.Equal("boom", ex.Message);
        Assert.Equal(0, count);
        Assert.Equal(1, rolledBackCount);
    }

    [Fact]
    public void ExecuteInTransaction_Func_CommitsAndReturnsValue()
    {
        using SqliteEngineInMemoryTestDatabase database = new();
        SqliteEngine engine = new(database.ConnectionString);

        string result = engine.ExecuteInTransaction(tx =>
        {
            tx.ExecuteNonQuery(
                "INSERT INTO [Sample] ([Name], [Amount]) VALUES (@Name, @Amount);",
                new[]
                {
                    new SqliteParameter("@Name", "wrapper-func"),
                    new SqliteParameter("@Amount", 14.5m)
                });

            long? count = tx.ExecuteScalar<long>("SELECT COUNT(*) FROM [Sample] WHERE [Name] = 'wrapper-func';");
            return $"count={count}";
        });

        Assert.Equal("count=1", result);
    }

}
