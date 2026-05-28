using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using System.Windows.Forms;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class CommandExecutionQueueTests
{
    [Fact]
    public void Snapshot_CountsStatuses()
    {
        CommandExecutionQueueSnapshot snapshot = new()
        {
            Items =
            [
                new CommandExecutionQueueItem
                {
                    CommandName = "one",
                    Status = CommandExecutionQueueStatus.Pending
                },
                new CommandExecutionQueueItem
                {
                    CommandName = "two",
                    Status = CommandExecutionQueueStatus.Running
                },
                new CommandExecutionQueueItem
                {
                    CommandName = "three",
                    Status = CommandExecutionQueueStatus.Completed
                },
                new CommandExecutionQueueItem
                {
                    CommandName = "four",
                    Status = CommandExecutionQueueStatus.Failed
                },
                new CommandExecutionQueueItem
                {
                    CommandName = "five",
                    Status = CommandExecutionQueueStatus.Cancelled
                }
            ]
        };

        Assert.Equal(1, snapshot.PendingCount);
        Assert.Equal(1, snapshot.RunningCount);
        Assert.Equal(1, snapshot.CompletedCount);
        Assert.Equal(1, snapshot.FailedCount);
        Assert.Equal(1, snapshot.CancelledCount);
    }

    [Fact]
    public void Formatter_IncludesCommandNameAndStatus()
    {
        CommandExecutionQueueSnapshot snapshot = new()
        {
            Items =
            [
                new CommandExecutionQueueItem
                {
                    CommandName = "test.command",
                    Status = CommandExecutionQueueStatus.Completed,
                    Message = "Done"
                }
            ]
        };

        string text =
            CommandExecutionQueueTextFormatter.Format(snapshot);

        Assert.Contains("test.command", text);
        Assert.Contains("Completed", text);
        Assert.Contains("Done", text);
    }

    [Fact]
    public void ListViewBuilder_Populate_StoresQueueItemInTag()
    {
        using ListView listView = new();

        CommandExecutionQueueItem item = new()
        {
            CommandName = "test.command"
        };

        CommandExecutionQueueListViewBuilder.ConfigureColumns(listView);
        CommandExecutionQueueListViewBuilder.Populate(
            listView,
            new CommandExecutionQueueSnapshot
            {
                Items = [item]
            });

        Assert.Single(listView.Items);
        Assert.Same(item, listView.Items[0].Tag);
    }
}
