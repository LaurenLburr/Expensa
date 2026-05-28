using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class CommandDispatcherTests
{
    [Fact]
    public async Task ExecuteAsync_WhenHandlerRegistered_ReturnsSuccess()
    {
        CommandDispatcher dispatcher = new();
        dispatcher.Register(new EchoCommandHandler());

        CommandExecutionResult result =
            await dispatcher.ExecuteAsync(new CommandExecutionRequest
            {
                CommandName = "echo",
                CorrelationId = "test-correlation",
                Parameters = new Dictionary<string, object?>
                {
                    ["Text"] = "hello"
                }
            });

        Assert.Equal(CommandExecutionStatus.Succeeded, result.Status);
        Assert.Equal("echo", result.CommandName);
        Assert.Equal("test-correlation", result.CorrelationId);
    }

    [Fact]
    public async Task ExecuteAsync_WhenHandlerMissing_ReturnsFailure()
    {
        CommandDispatcher dispatcher = new();

        CommandExecutionResult result =
            await dispatcher.ExecuteAsync(new CommandExecutionRequest
            {
                CommandName = "missing",
                CorrelationId = "test-correlation"
            });

        Assert.Equal(CommandExecutionStatus.Failed, result.Status);
    }

    [Fact]
    public void Register_WhenDuplicateCommandName_Throws()
    {
        CommandDispatcher dispatcher = new();
        dispatcher.Register(new EchoCommandHandler());

        Assert.Throws<InvalidOperationException>(() => dispatcher.Register(new EchoCommandHandler()));
    }

    [Fact]
    public async Task ExecuteAsync_WhenCancellationRequested_ReturnsCancelled()
    {
        CommandDispatcher dispatcher = new();
        dispatcher.Register(new EchoCommandHandler());

        using CancellationTokenSource source = new();
        source.Cancel();

        CommandExecutionResult result =
            await dispatcher.ExecuteAsync(
                new CommandExecutionRequest
                {
                    CommandName = "echo",
                    CorrelationId = "test-correlation"
                },
                source.Token);

        Assert.Equal(CommandExecutionStatus.Cancelled, result.Status);
    }

    private sealed class EchoCommandHandler : ICommandHandler
    {
        public string CommandName => "echo";

        public Task<CommandExecutionResult> ExecuteAsync(
            CommandExecutionRequest request,
            ICommandExecutionContext context)
        {
            context.WriteLog("Echo command executed.");

            return Task.FromResult(
                CommandExecutionResult.Succeeded(
                    request.CommandName,
                    request.CorrelationId,
                    "Echo completed.",
                    "{\"ok\":true}"));
        }
    }
}
