using Microsoft.Data.Sqlite;
using System.Data;
using Xunit;

namespace Codex.Data.SQLiteEngine.Tests;

public sealed class SqliteEngineTests
{
    [Fact]
    public void ExecuteNonQuery_InsertsRow_AndRaisesEvents()
    {
        using SqliteEngineInMemoryTestDatabase database = new();
        SqliteEngine engine = new(database.ConnectionString);

        int executingCount = 0;
        int executedCount = 0;
        int failedCount = 0;
        SqliteCommandEventArgs? executedArgs = null;

        engine.CommandExecuting += (_, _) => executingCount++;
        engine.CommandExecuted += (_, e) =>
        {
            executedCount++;
            executedArgs = e;
        };
        engine.CommandFailed += (_, _) => failedCount++;

        int rows = engine.ExecuteNonQuery(
            "INSERT INTO [Sample] ([Name], [Amount]) VALUES (@Name, @Amount);",
            new[]
            {
                new SqliteParameter("@Name", "alpha"),
                new SqliteParameter("@Amount", 12.5m)
            });

        Assert.Equal(1, rows);
        Assert.Equal(1, executingCount);
        Assert.Equal(1, executedCount);
        Assert.Equal(0, failedCount);
        Assert.NotNull(executedArgs);
        Assert.Equal(1, executedArgs!.RowsAffected);
        Assert.True(executedArgs.Duration >= TimeSpan.Zero);
    }

    [Fact]
    public void ExecuteScalar_ReturnsTypedValue()
    {
        using SqliteEngineInMemoryTestDatabase database = new();
        SqliteEngine engine = new(database.ConnectionString);

        engine.ExecuteNonQuery(
            "INSERT INTO [Sample] ([Name], [Amount]) VALUES (@Name, @Amount);",
            new[]
            {
                new SqliteParameter("@Name", "beta"),
                new SqliteParameter("@Amount", 99.25m)
            });

        long? count = engine.ExecuteScalar<long>(
            "SELECT COUNT(*) FROM [Sample];");

        Assert.Equal(1, count);
    }

    [Fact]
    public void Query_ReturnsMappedRows()
    {
        using SqliteEngineInMemoryTestDatabase database = new();
        SqliteEngine engine = new(database.ConnectionString);

        engine.ExecuteNonQuery(
            "INSERT INTO [Sample] ([Name], [Amount]) VALUES (@Name, @Amount);",
            new[]
            {
                new SqliteParameter("@Name", "gamma"),
                new SqliteParameter("@Amount", 5.5m)
            });

        IReadOnlyList<SampleRow> rows = engine.Query(
            "SELECT [Id], [Name], [Amount] FROM [Sample] ORDER BY [Id];",
            reader => new SampleRow
            {
                Id = reader.GetInt64(0),
                Name = reader.GetString(1),
                Amount = reader.GetDecimal(2)
            });

        Assert.Single(rows);
        Assert.Equal("gamma", rows[0].Name);
        Assert.Equal(5.5m, rows[0].Amount);
    }

    [Fact]
    public void QueryDataTable_ReturnsRowsAndColumns()
    {
        using SqliteEngineInMemoryTestDatabase database = new();
        SqliteEngine engine = new(database.ConnectionString);

        engine.ExecuteNonQuery(
            "INSERT INTO [Sample] ([Name], [Amount]) VALUES ('delta', 7.25);");

        DataTable table = engine.QueryDataTable(
            "SELECT [Id], [Name], [Amount] FROM [Sample];");

        Assert.Equal(1, table.Rows.Count);
        Assert.True(table.Columns.Contains("Id"));
        Assert.True(table.Columns.Contains("Name"));
        Assert.True(table.Columns.Contains("Amount"));
    }

    [Fact]
    public void ExecuteNonQuery_WhenSqlFails_RaisesFailedEvent()
    {
        using SqliteEngineInMemoryTestDatabase database = new();
        SqliteEngine engine = new(database.ConnectionString);

        int executingCount = 0;
        int executedCount = 0;
        int failedCount = 0;
        SqliteCommandEventArgs? failedArgs = null;

        engine.CommandExecuting += (_, _) => executingCount++;
        engine.CommandExecuted += (_, _) => executedCount++;
        engine.CommandFailed += (_, e) =>
        {
            failedCount++;
            failedArgs = e;
        };

        Assert.Throws<SqliteException>(() =>
            engine.ExecuteNonQuery("INSERT INTO [DoesNotExist] ([Name]) VALUES ('oops');"));

        Assert.Equal(1, executingCount);
        Assert.Equal(0, executedCount);
        Assert.Equal(1, failedCount);
        Assert.NotNull(failedArgs);
        Assert.NotNull(failedArgs!.Exception);
        Assert.True(failedArgs.Duration >= TimeSpan.Zero);
    }

    [Fact]
    public void ExecuteScalar_WhenResultIsNull_ReturnsDefault()
    {
        using SqliteEngineInMemoryTestDatabase database = new();
        SqliteEngine engine = new(database.ConnectionString);

        string? value = engine.ExecuteScalar<string>(
            "SELECT NULL;");

        Assert.Null(value);
    }

    private sealed class SampleRow
    {
        public long Id { get; init; }

        public required string Name { get; init; }

        public decimal Amount { get; init; }
    }
}
