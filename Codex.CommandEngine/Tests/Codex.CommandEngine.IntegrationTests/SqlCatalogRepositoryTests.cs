using Codex.CommandEngine.Data;
using Xunit;

namespace Codex.CommandEngine.IntegrationTests;

public sealed class SqlCatalogRepositoryTests
{
    [Fact]
    public void ListAll_ReturnsSqlCatalogEntries()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(SqlCatalogRepositoryTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            CommandEngineConnectionFactory factory =
                new(CommandEngineDatabaseOptions.ForFile(databasePath));

            SqlCatalogRepository repository =
                new(factory);

            IReadOnlyList<SqlCatalogRecord> records =
                repository.ListAll();

            Assert.NotEmpty(records);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }
}
