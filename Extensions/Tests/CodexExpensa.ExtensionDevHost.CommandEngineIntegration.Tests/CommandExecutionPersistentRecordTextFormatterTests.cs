using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class CommandExecutionPersistentRecordTextFormatterTests
{
    [Fact]
    public void Format_IncludesRecordDetails()
    {
        string text =
            CommandExecutionPersistentRecordTextFormatter.Format(
                "Records",
                [
                    new CommandExecutionPersistentRecord
                    {
                        ExecutionId = "abc",
                        SourceKind = "History",
                        CommandName = "test.command",
                        Status = "Completed",
                        CreatedUtc = DateTimeOffset.Parse("2026-01-01T00:00:00Z"),
                        ParameterJson = "{}"
                    }
                ]);

        Assert.Contains("Records", text);
        Assert.Contains("abc", text);
        Assert.Contains("test.command", text);
        Assert.Contains("Completed", text);
    }
}
