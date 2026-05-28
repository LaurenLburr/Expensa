using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class CommandExecutionPersistentRecordReplayMapperTests
{
    [Fact]
    public void CanReplay_WhenCommandNameExists_ReturnsTrue()
    {
        CommandExecutionPersistentRecord record = new()
        {
            ExecutionId = "abc",
            CommandName = "test.command",
            CreatedUtc = DateTimeOffset.Parse("2026-01-01T00:00:00Z")
        };

        Assert.True(CommandExecutionPersistentRecordReplayMapper.CanReplay(record));
    }

    [Fact]
    public void GetParameterJson_WhenBlank_ReturnsEmptyObject()
    {
        CommandExecutionPersistentRecord record = new()
        {
            ExecutionId = "abc",
            CommandName = "test.command",
            ParameterJson = "",
            CreatedUtc = DateTimeOffset.Parse("2026-01-01T00:00:00Z")
        };

        Assert.Equal("{}", CommandExecutionPersistentRecordReplayMapper.GetParameterJson(record));
    }
}
