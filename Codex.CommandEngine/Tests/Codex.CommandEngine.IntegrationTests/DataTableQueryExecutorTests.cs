using System.Data;
using Codex.CommandEngine.Data;
using Xunit;

namespace Codex.CommandEngine.IntegrationTests;

public sealed class DataTableQueryExecutorTests
{
    [Fact]
    public void ExecuteQuery_ReturnsDataTable()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(DataTableQueryExecutorTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            CommandEngineConnectionFactory factory =
                new(CommandEngineDatabaseOptions.ForFile(databasePath));

            DataTableQueryExecutor executor =
                new(factory);

            DataTable table =
                executor.ExecuteQuery("SELECT 'alpha' AS Name;");

            Assert.Single(table.Rows);
            Assert.Equal("alpha", table.Rows[0].GetRequiredString("Name"));
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void ExecuteQuery_WithParameters_ReturnsFilteredData()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(DataTableQueryExecutorTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            CommandEngineConnectionFactory factory =
                new(CommandEngineDatabaseOptions.ForFile(databasePath));

            DataTableQueryExecutor executor =
                new(factory);

            DataTable table =
                executor.ExecuteQuery(
                    "SELECT $Value AS Name;",
                    new Dictionary<string, object?>
                    {
                        ["Value"] = "parameter-value"
                    });

            Assert.Single(table.Rows);
            Assert.Equal("parameter-value", table.Rows[0].GetRequiredString("Name"));
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }
}
