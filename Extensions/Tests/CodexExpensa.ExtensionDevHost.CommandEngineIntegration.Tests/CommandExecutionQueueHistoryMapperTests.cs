using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class CommandExecutionQueueHistoryMapperTests
{
    [Fact]
    public void ShouldRecordToHistory_WhenCompleted_ReturnsTrue()
    {
        CommandExecutionQueueItem item = new()
        {
            CommandName = "test.command",
            Status = CommandExecutionQueueStatus.Completed
        };

        Assert.True(CommandExecutionQueueHistoryMapper.ShouldRecordToHistory(item));
    }

    [Fact]
    public void ShouldRecordToHistory_WhenRunning_ReturnsFalse()
    {
        CommandExecutionQueueItem item = new()
        {
            CommandName = "test.command",
            Status = CommandExecutionQueueStatus.Running
        };

        Assert.False(CommandExecutionQueueHistoryMapper.ShouldRecordToHistory(item));
    }

    [Fact]
    public void ToHistoryRecord_CopiesQueueFields()
    {
        CommandExecutionQueueItem item = new()
        {
            CreatedUtc = DateTimeOffset.Parse("2026-01-01T00:00:00Z"),
            StartedUtc = DateTimeOffset.Parse("2026-01-01T00:00:01Z"),
            CompletedUtc = DateTimeOffset.Parse("2026-01-01T00:00:02Z"),
            CommandName = "test.command",
            ParameterJson = "{\"value\":1}",
            Status = CommandExecutionQueueStatus.Completed,
            Message = "Done.",
            CorrelationId = "abc-123",
            OutputJson = "{\"ok\":true}"
        };

        CommandExecutionHistoryRecord record =
            CommandExecutionQueueHistoryMapper.ToHistoryRecord(item);

        Assert.Equal(item.StartedUtc, record.StartedUtc);
        Assert.Equal(item.CompletedUtc, record.CompletedUtc);
        Assert.Equal("test.command", record.CommandName);
        Assert.Equal("{\"value\":1}", record.ParameterJson);
        Assert.Equal("Completed", record.Status);
        Assert.Equal("Done.", record.Message);
        Assert.Equal("abc-123", record.CorrelationId);
        Assert.Equal("{\"ok\":true}", record.OutputJson);
    }
}
