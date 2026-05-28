using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class CommandExecutionQueueRecoveryMapperTests
{
    [Fact]
    public void ToRecoveredQueueItem_WhenPending_MarksFailed()
    {
        CommandExecutionPersistentRecord record = new()
        {
            ExecutionId = Guid.NewGuid().ToString("N"),
            SourceKind = "Queue",
            CommandName = "test.command",
            Status = "Pending",
            CreatedUtc = DateTimeOffset.Parse("2026-01-01T00:00:00Z"),
            IsQueueItem = 1
        };

        CommandExecutionQueueItem item =
            CommandExecutionQueueRecoveryMapper.ToRecoveredQueueItem(record);

        Assert.Equal(CommandExecutionQueueStatus.Failed, item.Status);
        Assert.Contains("Recovered", item.Message);
    }

    [Fact]
    public void ToRecoveredQueueItem_WhenCompleted_PreservesCompleted()
    {
        CommandExecutionPersistentRecord record = new()
        {
            ExecutionId = Guid.NewGuid().ToString("N"),
            SourceKind = "Queue",
            CommandName = "test.command",
            Status = "Completed",
            CreatedUtc = DateTimeOffset.Parse("2026-01-01T00:00:00Z"),
            CompletedUtc = DateTimeOffset.Parse("2026-01-01T00:00:01Z"),
            Message = "Done.",
            IsQueueItem = 1
        };

        CommandExecutionQueueItem item =
            CommandExecutionQueueRecoveryMapper.ToRecoveredQueueItem(record);

        Assert.Equal(CommandExecutionQueueStatus.Completed, item.Status);
        Assert.Equal("Done.", item.Message);
    }

    [Fact]
    public void ToRecoveredQueueItems_FiltersNonQueueRecords()
    {
        IReadOnlyList<CommandExecutionQueueItem> items =
            CommandExecutionQueueRecoveryMapper.ToRecoveredQueueItems(
                [
                    new CommandExecutionPersistentRecord
                    {
                        ExecutionId = Guid.NewGuid().ToString("N"),
                        CommandName = "queue.command",
                        Status = "Completed",
                        CreatedUtc = DateTimeOffset.Parse("2026-01-01T00:00:00Z"),
                        IsQueueItem = 1
                    },
                    new CommandExecutionPersistentRecord
                    {
                        ExecutionId = Guid.NewGuid().ToString("N"),
                        CommandName = "history.command",
                        Status = "Completed",
                        CreatedUtc = DateTimeOffset.Parse("2026-01-01T00:00:00Z"),
                        IsHistory = 1
                    }
                ]);

        Assert.Single(items);
        Assert.Equal("queue.command", items[0].CommandName);
    }
}
