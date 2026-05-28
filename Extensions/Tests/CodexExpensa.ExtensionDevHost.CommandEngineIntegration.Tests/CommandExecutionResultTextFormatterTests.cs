using Codex.CommandEngine.Core;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class CommandExecutionResultTextFormatterTests
{
    [Fact]
    public void Format_IncludesCoreFields()
    {
        CommandExecutionResult result =
            CommandExecutionResult.Succeeded(
                "test.command",
                "abc-123",
                "Done.",
                "{\"ok\":true}");

        string text =
            CommandExecutionResultTextFormatter.Format(result);

        Assert.Contains("Command Execution Result", text);
        Assert.Contains("test.command", text);
        Assert.Contains("abc-123", text);
        Assert.Contains("Succeeded", text);
        Assert.Contains("{\"ok\":true}", text);
    }
}
