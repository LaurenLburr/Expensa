using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class SqliteCommandExecutionPersistenceRetentionTests
{
    [Fact]
    public void DeleteRecords_WhenStatusMatches_DeletesRecords()
    {
        SqliteCommandExecutionPersistenceStore store = CreateStore();

        store.UpsertQueueItem(new CommandExecutionQueueItem
        {
            CommandName = "failed.command",
            Status = CommandExecutionQueueStatus.Failed
        });

        int deleted =
            store.DeleteRecords(new CommandExecutionRetentionOptions
            {
                Status = "Failed"
            });

        Assert.Equal(1, deleted);
        Assert.Empty(store.QueryRecords(new CommandExecutionPersistentRecordQuery()));
    }

    [Fact]
    public void Vacuum_DoesNotThrow()
    {
        SqliteCommandExecutionPersistenceStore store = CreateStore();

        store.Vacuum();
    }

    private static SqliteCommandExecutionPersistenceStore CreateStore()
    {
        string folder = Path.Combine(
            Path.GetTempPath(),
            "CommandExecutionPersistenceRetentionTests",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(folder);

        return new SqliteCommandExecutionPersistenceStore(
            new CommandExecutionPersistenceOptions
            {
                DatabasePath = Path.Combine(folder, "command-execution.db")
            });
    }
}
