using Codex.CommandEngine.Data;
using Xunit;

namespace Codex.CommandEngine.IntegrationTests;

public sealed class SqlQueryCatalogTests
{
    [Fact]
    public void Upsert_Should_Insert_Query_Into_Catalog()
    {
        string databasePath = CreateTestDatabase();

        try
        {
            SqlQueryCatalog catalog = CreateCatalog(databasePath);

            catalog.Upsert(new SqlQueryDefinition(
                "CommandDefinition.SelectAll",
                "Selects all command definitions.",
                "SELECT * FROM CommandDefinition ORDER BY CommandName;",
                "CommandDefinition"));

            SqlQueryRecord? record = catalog.FindByName("CommandDefinition.SelectAll");

            Assert.NotNull(record);
            Assert.Equal("CommandDefinition.SelectAll", record.QueryName);
            Assert.Equal("CommandDefinition", record.Category);
            Assert.True(record.IsActive);
            Assert.Contains("CommandDefinition", record.SqlText);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void Upsert_Should_Update_Existing_Query_And_Set_UpdatedUtc()
    {
        string databasePath = CreateTestDatabase();

        try
        {
            SqlQueryCatalog catalog = CreateCatalog(databasePath);

            catalog.Upsert(new SqlQueryDefinition(
                "ExecutionHistory.SelectRecent",
                "Original description.",
                "SELECT * FROM ExecutionHistory;",
                "ExecutionHistory"));

            catalog.Upsert(new SqlQueryDefinition(
                "ExecutionHistory.SelectRecent",
                "Updated description.",
                "SELECT ExecutionId FROM ExecutionHistory ORDER BY StartedUtc DESC;",
                "ExecutionHistory"));

            SqlQueryRecord? record = catalog.FindByName("ExecutionHistory.SelectRecent");

            Assert.NotNull(record);
            Assert.Equal("Updated description.", record.Description);
            Assert.Contains("ORDER BY StartedUtc DESC", record.SqlText);
            Assert.False(string.IsNullOrWhiteSpace(record.UpdatedUtc));
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void GetRequiredSqlText_Should_Throw_When_Query_Is_Missing()
    {
        string databasePath = CreateTestDatabase();

        try
        {
            SqlQueryCatalog catalog = CreateCatalog(databasePath);

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
                () => catalog.GetRequiredSqlText("Missing.Query"));

            Assert.Contains("SQL query was not found", exception.Message);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void GetRequiredSqlText_Should_Throw_When_Query_Is_Inactive()
    {
        string databasePath = CreateTestDatabase();

        try
        {
            SqlQueryCatalog catalog = CreateCatalog(databasePath);

            catalog.Upsert(new SqlQueryDefinition(
                "Inactive.Query",
                "Inactive test query.",
                "SELECT 1;",
                "Test",
                IsActive: false));

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
                () => catalog.GetRequiredSqlText("Inactive.Query"));

            Assert.Contains("SQL query is inactive", exception.Message);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void ListActive_Should_Include_User_Active_Queries_In_Category_And_Name_Order()
    {
        string databasePath = CreateTestDatabase();

        try
        {
            SqlQueryCatalog catalog = CreateCatalog(databasePath);

            catalog.Upsert(new SqlQueryDefinition("Zeta.Query", "Zeta.", "SELECT 1;", "Zeta"));
            catalog.Upsert(new SqlQueryDefinition("Alpha.QueryB", "Alpha B.", "SELECT 2;", "Alpha"));
            catalog.Upsert(new SqlQueryDefinition("Alpha.QueryA", "Alpha A.", "SELECT 3;", "Alpha"));
            catalog.Upsert(new SqlQueryDefinition("Hidden.Query", "Hidden.", "SELECT 4;", "Alpha", IsActive: false));

            IReadOnlyList<SqlQueryRecord> records = catalog.ListActive();

            IReadOnlyList<SqlQueryRecord> testRecords = records
                .Where(static x =>
                    x.QueryName is "Alpha.QueryA" or "Alpha.QueryB" or "Zeta.Query" or "Hidden.Query")
                .ToList();

            Assert.Equal(3, testRecords.Count);
            Assert.Equal("Alpha.QueryA", testRecords[0].QueryName);
            Assert.Equal("Alpha.QueryB", testRecords[1].QueryName);
            Assert.Equal("Zeta.Query", testRecords[2].QueryName);
            Assert.DoesNotContain(testRecords, static x => x.QueryName == "Hidden.Query");
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    private static SqlQueryCatalog CreateCatalog(string databasePath)
    {
        CommandEngineConnectionFactory connectionFactory =
            TestDatabasePaths.CreateConnectionFactory(databasePath);

        return new SqlQueryCatalog(connectionFactory);
    }

    private static string CreateTestDatabase()
    {
        return TestDatabasePaths.ResetDatabaseForTestClass(nameof(SqlQueryCatalogTests));
    }
}
