using Codex.CommandEngine.Data;
using Microsoft.Data.Sqlite;
using Xunit;

namespace Codex.CommandEngine.IntegrationTests;

public sealed class RepositorySqlQueryNameConstantTests
{
    [Fact]
    public void RepositorySqlQueryNames_AllRequiredRepositoryQueries_Should_NotContainDuplicates()
    {
        IReadOnlyList<string> queryNames = RepositorySqlQueryNames.AllRequiredRepositoryQueries;

        int distinctCount = queryNames
            .Distinct(StringComparer.Ordinal)
            .Count();

        Assert.Equal(queryNames.Count, distinctCount);
    }

    [Theory]
    [MemberData(nameof(RequiredRepositoryQueryNames))]
    public void TemplateDatabase_ContainsRequiredRepositoryQuery_FromConstants(string queryName)
    {
        string databasePath = CreateTestDatabase();

        try
        {
            using SqliteConnection connection = TestDatabasePaths.OpenConnection(databasePath);

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = """
SELECT COUNT(*)
FROM SqlQuery
WHERE QueryName = $QueryName
  AND IsActive = 1
  AND trim(SqlText) <> '';
""";
            command.Parameters.AddWithValue("$QueryName", queryName);

            long count = Convert.ToInt64(
                command.ExecuteScalar(),
                System.Globalization.CultureInfo.InvariantCulture);

            Assert.Equal(1L, count);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    public static TheoryData<string> RequiredRepositoryQueryNames()
    {
        TheoryData<string> data = [];

        foreach (string queryName in RepositorySqlQueryNames.AllRequiredRepositoryQueries)
        {
            data.Add(queryName);
        }

        return data;
    }

    private static string CreateTestDatabase()
    {
        return TestDatabasePaths.ResetDatabaseForTestClass(nameof(RepositorySqlQueryNameConstantTests));
    }
}
