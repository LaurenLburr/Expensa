using Codex.CommandEngine.Data;
using Xunit;

namespace Codex.CommandEngine.IntegrationTests;

public sealed class AiProviderRepositoryDataTableTests
{
    [Fact]
    public void ListAll_AfterUpsert_ReturnsProviderThroughDataTablePath()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(AiProviderRepositoryDataTableTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            CommandEngineConnectionFactory factory =
                new(CommandEngineDatabaseOptions.ForFile(databasePath));

            AiProviderRepository repository =
                new(factory);

            repository.Upsert(new AiProviderUpsert
            {
                AiProviderId = "provider-datatable-001",
                ProviderName = "provider.datatable",
                DisplayName = "DataTable Provider",
                ProviderKind = "Test",
                Description = "Provider read through DataTable migration path.",
                ConfigurationJson = "{}",
                MetadataJson = "{}",
                IsEnabled = true
            });

            IReadOnlyList<AiProviderRecord> providers =
                repository.ListAll();

            Assert.Contains(providers, provider => provider.ProviderName == "provider.datatable");
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }
}
