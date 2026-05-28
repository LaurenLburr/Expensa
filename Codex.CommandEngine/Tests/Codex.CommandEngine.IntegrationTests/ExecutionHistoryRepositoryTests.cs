using Codex.CommandEngine.Data;
using Xunit;

namespace Codex.CommandEngine.IntegrationTests;

public sealed class ExecutionHistoryRepositoryTests
{
    [Fact]
    public void Start_ThenFindByExecutionId_ReturnsStartedRecord()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(ExecutionHistoryRepositoryTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            CommandEngineConnectionFactory factory =
                new(CommandEngineDatabaseOptions.ForFile(databasePath));

            ExecutionHistoryRepository repository =
                new(factory);

            repository.Start(new ExecutionHistoryStart
            {
                ExecutionId = "execution-001",
                CorrelationId = "correlation-001",
                CommandName = "test.command",
                RequestJson = "{\"input\":true}"
            });

            ExecutionHistoryRecord? record =
                repository.FindByExecutionId("execution-001");

            Assert.NotNull(record);
            Assert.Equal("correlation-001", record!.CorrelationId);
            Assert.Equal("test.command", record.CommandName);
            Assert.Equal("Started", record.Status);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void Complete_ThenFindByExecutionId_ReturnsCompletedRecord()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(ExecutionHistoryRepositoryTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            CommandEngineConnectionFactory factory =
                new(CommandEngineDatabaseOptions.ForFile(databasePath));

            ExecutionHistoryRepository repository =
                new(factory);

            repository.Start(new ExecutionHistoryStart
            {
                ExecutionId = "execution-002",
                CorrelationId = "correlation-002",
                CommandName = "test.command",
                RequestJson = "{}"
            });

            repository.Complete(new ExecutionHistoryCompletion
            {
                ExecutionId = "execution-002",
                Status = "Succeeded",
                DurationMilliseconds = 42,
                OutputJson = "{\"ok\":true}",
                Message = "Done"
            });

            ExecutionHistoryRecord? record =
                repository.FindByExecutionId("execution-002");

            Assert.NotNull(record);
            Assert.Equal("Succeeded", record!.Status);
            Assert.Equal(42, record.DurationMilliseconds);
            Assert.Equal("Done", record.Message);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void ListRecent_ReturnsRecords()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(ExecutionHistoryRepositoryTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            CommandEngineConnectionFactory factory =
                new(CommandEngineDatabaseOptions.ForFile(databasePath));

            ExecutionHistoryRepository repository =
                new(factory);

            repository.Start(new ExecutionHistoryStart
            {
                ExecutionId = "execution-003",
                CorrelationId = "correlation-003",
                CommandName = "test.command",
                RequestJson = "{}"
            });

            IReadOnlyList<ExecutionHistoryRecord> records =
                repository.ListRecent();

            Assert.NotEmpty(records);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }
}
