using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class SqliteCommandExecutionPersistenceStoreTests
{
    [Fact]
    public void UpsertQueueItem_ThenListQueueItems_ReturnsItem()
    {
        SqliteCommandExecutionPersistenceStore store = CreateStore();

        store.UpsertQueueItem(new CommandExecutionQueueItem
        {
            CommandName = "test.command",
            ParameterJson = "{\"value\":1}",
            Status = CommandExecutionQueueStatus.Pending,
            Message = "Waiting"
        });

        IReadOnlyList<CommandExecutionPersistentRecord> records = store.ListQueueItems();

        Assert.Single(records);
        Assert.Equal("test.command", records[0].CommandName);
        Assert.Equal("Pending", records[0].Status);
        Assert.Equal(1, records[0].IsQueueItem);
    }

    [Fact]
    public void UpsertHistoryRecord_ThenListHistoryRecords_ReturnsRecord()
    {
        SqliteCommandExecutionPersistenceStore store = CreateStore();

        store.UpsertHistoryRecord(new CommandExecutionHistoryRecord
        {
            StartedUtc = DateTimeOffset.Parse("2026-01-01T00:00:00Z"),
            CompletedUtc = DateTimeOffset.Parse("2026-01-01T00:00:01Z"),
            CommandName = "test.command",
            Status = "Completed",
            Message = "Done.",
            ParameterJson = "{}"
        });

        IReadOnlyList<CommandExecutionPersistentRecord> records = store.ListHistoryRecords();

        Assert.Single(records);
        Assert.Equal("test.command", records[0].CommandName);
        Assert.Equal("Completed", records[0].Status);
        Assert.Equal(1, records[0].IsHistory);
    }

    private static SqliteCommandExecutionPersistenceStore CreateStore()
    {
        string folder = Path.Combine(
            Path.GetTempPath(),
            "CommandExecutionPersistenceTests",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(folder);

        return new SqliteCommandExecutionPersistenceStore(
            new CommandExecutionPersistenceOptions
            {
                DatabasePath = Path.Combine(folder, "command-execution.db")
            });
    }
}
