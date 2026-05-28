using Microsoft.Data.Sqlite;
using Codex.CommandEngine.Data;
using Xunit;

namespace Codex.CommandEngine.IntegrationTests;

public sealed class SqliteDataReaderExtensionsTests
{
    [Fact]
    public void ReaderHelpers_Should_Read_By_Name_Not_Ordinal()
    {
        string databasePath = TestDatabasePaths.ResetDatabaseForTestClass(nameof(SqliteDataReaderExtensionsTests));

        try
        {
            using SqliteConnection connection = TestDatabasePaths.OpenConnection(databasePath);

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = """
SELECT
    'Updated' AS UpdatedUtc,
    1 AS IsEnabled,
    'provider-001' AS AiProviderId,
    'OpenAI' AS ProviderKind,
    'Created' AS CreatedUtc,
    'Default Provider' AS DisplayName,
    'provider.default' AS ProviderName,
    '{}' AS MetadataJson,
    '{}' AS ConfigurationJson,
    'Description' AS Description;
""";

            using SqliteDataReader reader = command.ExecuteReader();

            Assert.True(reader.Read());
            Assert.Equal("provider-001", reader.GetRequiredString("AiProviderId"));
            Assert.Equal("provider.default", reader.GetRequiredString("ProviderName"));
            Assert.Equal("Default Provider", reader.GetStringOrDefault("DisplayName"));
            Assert.True(reader.GetBooleanOrDefault("IsEnabled"));
            Assert.Equal("Updated", reader.GetNullableString("UpdatedUtc"));
            Assert.Equal("fallback", reader.GetStringOrDefault("MissingColumn", "fallback"));
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }
}
