using Codex.CommandEngine.Core;
using WebsitesAddin;
using Xunit;

namespace WebsitesAddin.Tests;

public sealed class WebsiteLoadRuntimeCommandHandlerTests
{
    [Fact]
    public async Task ExecuteAsync_WhenCommandMatches_ReturnsSucceededResult()
    {
        WebsiteLoadRuntimeCommandHandler handler = new();

        TestCommandExecutionContext context = new();

        CommandExecutionResult result =
            await handler.ExecuteAsync(
                new CommandExecutionRequest
                {
                    CommandName = "websites.load",
                    Parameters = new Dictionary<string, object?>
                    {
                        ["searchText"] = "bank"
                    }
                },
                context);

        Assert.Equal(CommandExecutionStatus.Succeeded, result.Status);
        Assert.Equal("websites.load", result.CommandName);
        Assert.Contains("Banking", result.OutputJson);
        Assert.Contains("Loaded", string.Join(Environment.NewLine, context.LogMessages));
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandDoesNotMatch_ReturnsFailedResult()
    {
        WebsiteLoadRuntimeCommandHandler handler = new();

        CommandExecutionResult result =
            await handler.ExecuteAsync(
                new CommandExecutionRequest
                {
                    CommandName = "wrong.command"
                },
                new TestCommandExecutionContext());

        Assert.Equal(CommandExecutionStatus.Failed, result.Status);
    }

    private sealed class TestCommandExecutionContext : ICommandExecutionContext
    {
        public string CorrelationId { get; } =
            Guid.NewGuid().ToString("N");

        public string ContextJson { get; } =
            "{}";

        public CancellationToken CancellationToken =>
            CancellationToken.None;

        public List<string> LogMessages { get; } = [];

        public void WriteLog(
            string message)
        {
            LogMessages.Add(message);
        }
    }
}
