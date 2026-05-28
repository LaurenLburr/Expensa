using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class CommandExecutionHistoryTests
{
    [Fact]
    public void AddThenListAll_ReturnsMostRecentFirst()
    {
        CommandExecutionHistoryStore store = new();

        store.Add(new CommandExecutionHistoryRecord
        {
            StartedUtc = DateTimeOffset.Parse("2026-01-01T00:00:00Z"),
            CompletedUtc = DateTimeOffset.Parse("2026-01-01T00:00:01Z"),
            CommandName = "older"
        });

        store.Add(new CommandExecutionHistoryRecord
        {
            StartedUtc = DateTimeOffset.Parse("2026-01-02T00:00:00Z"),
            CompletedUtc = DateTimeOffset.Parse("2026-01-02T00:00:01Z"),
            CommandName = "newer"
        });

        IReadOnlyList<CommandExecutionHistoryRecord> records = store.ListAll();

        Assert.Equal("newer", records[0].CommandName);
        Assert.Equal("older", records[1].CommandName);
    }

    [Fact]
    public void Format_WhenEmpty_ReturnsNoRecordsMessage()
    {
        string text = CommandExecutionHistoryTextFormatter.Format([]);

        Assert.Contains("No command executions", text);
    }

    [Fact]
    public void Format_WhenRecordsExist_IncludesCommandName()
    {
        string text = CommandExecutionHistoryTextFormatter.Format(
        [
            new CommandExecutionHistoryRecord
            {
                StartedUtc = DateTimeOffset.Parse("2026-01-01T00:00:00Z"),
                CompletedUtc = DateTimeOffset.Parse("2026-01-01T00:00:01Z"),
                CommandName = "test.command",
                Status = "Succeeded"
            }
        ]);

        Assert.Contains("test.command", text);
        Assert.Contains("Succeeded", text);
    }
}
