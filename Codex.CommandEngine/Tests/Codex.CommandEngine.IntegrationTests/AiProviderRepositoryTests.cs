using Codex.CommandEngine.Data;
using Microsoft.Data.Sqlite;
using Xunit;

namespace Codex.CommandEngine.IntegrationTests;

public sealed class AiProviderRepositoryTests
{
    [Fact]
    public void Upsert_ThenFindByName_ReturnsProvider()
    {
        string databasePath = CreateTestDatabase();

        try
        {
          
            AiProviderRepository repository = CreateRepository(databasePath);

            repository.Upsert(new AiProviderUpsert
            {
                AiProviderId = "provider-001",
                ProviderName = "openai.default",
                DisplayName = "Default OpenAI Provider",
                ProviderKind = "OpenAI",
                Description = "Primary provider definition for AI orchestration.",
                ConfigurationJson = "{\"model\":\"configured-at-runtime\"}",
                MetadataJson = "{}",
                IsEnabled = true
            });

            AiProviderRecord? record = repository.FindByName("openai.default");

            Assert.NotNull(record);
            Assert.Equal("provider-001", record.AiProviderId);
            Assert.Equal("Default OpenAI Provider", record.DisplayName);
            Assert.Equal("OpenAI", record.ProviderKind);
            Assert.True(record.IsEnabled);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void UpsertCapability_ThenListCapabilities_ReturnsProviderCapabilities()
    {
        string databasePath = CreateTestDatabase();

        try
        {
            AiProviderRepository repository = CreateRepository(databasePath);

            repository.Upsert(new AiProviderUpsert
            {
                AiProviderId = "provider-001",
                ProviderName = "openai.default",
                DisplayName = "Default OpenAI Provider",
                ProviderKind = "OpenAI",
                Description = "Primary provider definition for AI orchestration.",
                ConfigurationJson = "{}",
                MetadataJson = "{}",
                IsEnabled = true
            });

            repository.UpsertCapability(new AiProviderCapabilityUpsert
            {
                AiProviderCapabilityId = "capability-001",
                AiProviderId = "provider-001",
                CapabilityName = "chat.completion",
                Description = "Can produce conversational responses.",
                MetadataJson = "{}",
                IsEnabled = true
            });

            IReadOnlyList<AiProviderCapabilityRecord> capabilities =
                repository.ListCapabilities("provider-001");

            Assert.Single(capabilities);
            Assert.Equal("chat.completion", capabilities[0].CapabilityName);
            Assert.True(capabilities[0].IsEnabled);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void TemplateDatabase_ContainsAiProviderFoundationCatalogQueries()
    {
        string databasePath = CreateTestDatabase();

        try
        {
            using SqliteConnection connection = TestDatabasePaths.OpenConnection(databasePath);
            long count = CountRows(
                connection,
                "SELECT COUNT(*) FROM SqlQuery WHERE Category = 'AI Provider Foundation';");

            Assert.True(count >= 5);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    //[Fact]
    //public void TemplateDatabase_ContainsAiProviderFoundationSchemaSnapshot()
    //{
    //    string databasePath = CreateTestDatabase();

    //    try
    //    {
    //        using SqliteConnection connection = TestDatabasePaths.OpenConnection(databasePath);
    //        int version = Convert.ToInt32(
    //            ExecuteScalar(
    //                connection,
    //                "SELECT SchemaVersion FROM DatabaseSchemaSnapshot WHERE MigrationId = '0007_ai_provider_foundation';"),
    //            System.Globalization.CultureInfo.InvariantCulture);

    //        Assert.Equal(7, version);
    //    }
    //    finally
    //    {
    //        TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
    //    }
    //}

    private static AiProviderRepository CreateRepository(string databasePath)
    {
        CommandEngineConnectionFactory factory = TestDatabasePaths.CreateConnectionFactory(databasePath);
        return new AiProviderRepository(factory);
    }

    private static string CreateTestDatabase()
    {
        return TestDatabasePaths.ResetDatabaseForTestClass(nameof(AiProviderRepositoryTests));
    }

    private static long CountRows(SqliteConnection connection, string sql)
    {
        return Convert.ToInt64(ExecuteScalar(connection, sql), System.Globalization.CultureInfo.InvariantCulture);
    }

    private static object ExecuteScalar(SqliteConnection connection, string sql)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;
        return command.ExecuteScalar()
            ?? throw new InvalidOperationException($"Query returned no value: {sql}");
    }
}
