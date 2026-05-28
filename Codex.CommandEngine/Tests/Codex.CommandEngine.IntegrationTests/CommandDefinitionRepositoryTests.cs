using Codex.CommandEngine.Data;
using Microsoft.Data.Sqlite;
using Xunit;

namespace Codex.CommandEngine.IntegrationTests;

public sealed class CommandDefinitionRepositoryTests
{
    [Fact]
    public void Upsert_Should_Insert_And_Update_Command_Definition()
    {
        string databasePath = CreateTestDatabase();

        try
        {
            CommandDefinitionRepository repository = CreateRepository(databasePath);

            repository.Upsert(new CommandDefinitionUpsert
            {
                CommandDefinitionId = "command-001",
                CommandName = "Test.Command",
                DisplayName = "Test Command",
                Description = "Initial description",
                Category = "Tests",
                Version = 1,
                IsEnabled = true,
                HandlerType = "TestHandler"
            });

            repository.Upsert(new CommandDefinitionUpsert
            {
                CommandDefinitionId = "command-001",
                CommandName = "Test.Command",
                DisplayName = "Test Command Updated",
                Description = "Updated description",
                Category = "Tests",
                Version = 2,
                IsEnabled = false,
                HandlerType = "UpdatedHandler"
            });

            CommandDefinitionRecord? record = repository.FindByName("Test.Command");

            Assert.NotNull(record);
            Assert.Equal("command-001", record.CommandDefinitionId);
            Assert.Equal("Test Command Updated", record.DisplayName);
            Assert.Equal("Updated description", record.Description);
            Assert.Equal(2, record.Version);
            Assert.False(record.IsEnabled);
            Assert.Equal("UpdatedHandler", record.HandlerType);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void UpsertParameter_Should_Insert_And_List_Command_Parameters()
    {
        string databasePath = CreateTestDatabase();

        try
        {
            CommandDefinitionRepository repository = CreateRepository(databasePath);

            repository.Upsert(new CommandDefinitionUpsert
            {
                CommandDefinitionId = "command-001",
                CommandName = "Test.Command",
                DisplayName = "Test Command",
                Description = "Test command.",
                Category = "Tests",
                Version = 1,
                IsEnabled = true,
                HandlerType = "TestHandler"
            });

            repository.UpsertParameter(new CommandParameterDefinitionUpsert
            {
                CommandParameterDefinitionId = "parameter-001",
                CommandDefinitionId = "command-001",
                ParameterName = "inputPath",
                ParameterType = "string",
                IsRequired = true,
                DefaultValue = string.Empty,
                Description = "Input file path.",
                SortOrder = 1
            });

            IReadOnlyList<CommandParameterDefinitionRecord> parameters =
                repository.ListParameters("command-001");

            Assert.Single(parameters);
            Assert.Equal("inputPath", parameters[0].ParameterName);
            Assert.Equal("string", parameters[0].ParameterType);
            Assert.True(parameters[0].IsRequired);
            Assert.Equal(1, parameters[0].SortOrder);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void TemplateDatabase_ContainsCommandRegistrationCatalogEntries()
    {
        string databasePath = CreateTestDatabase();

        try
        {
            using SqliteConnection connection = TestDatabasePaths.OpenConnection(databasePath);
            long count = CountRows(
                connection,
                "SELECT COUNT(*) FROM SqlQuery WHERE Category = 'Command Registration';");

            Assert.True(count >= 5);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    private static CommandDefinitionRepository CreateRepository(string databasePath)
    {
        CommandEngineConnectionFactory factory = TestDatabasePaths.CreateConnectionFactory(databasePath);
        return new CommandDefinitionRepository(factory);
    }

    private static string CreateTestDatabase()
    {
        return TestDatabasePaths.ResetDatabaseForTestClass(nameof(CommandDefinitionRepositoryTests));
    }

    private static long CountRows(SqliteConnection connection, string sql)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;
        return Convert.ToInt64(command.ExecuteScalar(), System.Globalization.CultureInfo.InvariantCulture);
    }
}
