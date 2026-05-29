using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class SqliteCommandExecutionPersistenceStoreQueryTests
{
    [Fact]
    public void QueryRecords_WhenStatusFilter_ReturnsOnlyMatchingRecords()
    {
        SqliteCommandExecutionPersistenceStore store = CreateStore();

        store.UpsertQueueItem(new CommandExecutionQueueItem
        {
            CommandName = "failed.command",
            Status = CommandExecutionQueueStatus.Failed,
            Message = "Nope."
        });

        store.UpsertQueueItem(new CommandExecutionQueueItem
        {
            CommandName = "completed.command",
            Status = CommandExecutionQueueStatus.Completed,
            Message = "Done."
        });

        IReadOnlyList<CommandExecutionPersistentRecord> records =
            store.QueryRecords(new CommandExecutionPersistentRecordQuery
            {
                Status = "Failed"
            });

        Assert.Single(records);
        Assert.Equal("failed.command", records[0].CommandName);
    }

    [Fact]
    public void QueryRecords_WhenSearchText_ReturnsMatchingCommand()
    {
        SqliteCommandExecutionPersistenceStore store = CreateStore();

        store.UpsertQueueItem(new CommandExecutionQueueItem
        {
            CommandName = "websites.add",
            Status = CommandExecutionQueueStatus.Completed,
            Message = "Added website."
        });

        IReadOnlyList<CommandExecutionPersistentRecord> records =
            store.QueryRecords(new CommandExecutionPersistentRecordQuery
            {
                SearchText = "websites"
            });

        Assert.Single(records);
        Assert.Equal("websites.add", records[0].CommandName);
    }

    private static SqliteCommandExecutionPersistenceStore CreateStore()
    {
        string folder = Path.Combine(
            Path.GetTempPath(),
            "CommandExecutionPersistenceQueryTests",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(folder);

        return new SqliteCommandExecutionPersistenceStore(
            new CommandExecutionPersistenceOptions
            {
                DatabasePath = Path.Combine(folder, "command-execution.db")
            });
    }
}
