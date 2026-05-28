using AbstractionsCommandDefinition = Codex.CommandEngine.Abstractions.CommandDefinition;
using AbstractionsCommandExecutionResult = Codex.CommandEngine.Abstractions.CommandExecutionResult;
using AbstractionsCommandRequest = Codex.CommandEngine.Abstractions.CommandRequest;
using CoreCommandEngine = Codex.CommandEngine.Core.CommandEngine;
using CoreDelegateCommandHandler = Codex.CommandEngine.Core.DelegateCommandHandler;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class CommandEngineTests
{
    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_Command_Is_Not_Registered()
    {
        CoreCommandEngine engine = new();

        AbstractionsCommandExecutionResult result =
            await engine.ExecuteAsync(new AbstractionsCommandRequest
            {
                CommandName = "Missing.Command"
            });

        Assert.False(result.Succeeded);
        Assert.Contains("Command not found", result.Message);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Run_Registered_Command()
    {
        CoreCommandEngine engine = new();

        engine.Register(
            "Test.Command",
            static (_, _) => Task.FromResult(AbstractionsCommandExecutionResult.Success("Executed")));

        AbstractionsCommandExecutionResult result =
            await engine.ExecuteAsync(new AbstractionsCommandRequest
            {
                CommandName = "Test.Command"
            });

        Assert.True(result.Succeeded);
        Assert.Equal("Executed", result.Message);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Validation_Failure_From_Handler()
    {
        CoreCommandEngine engine = new();

        engine.Register(new CoreDelegateCommandHandler(
            new AbstractionsCommandDefinition
            {
                CommandName = "Validation.Command",
                DisplayName = "Validation Command",
                HandlerType = typeof(CoreDelegateCommandHandler).FullName ?? nameof(CoreDelegateCommandHandler)
            },
            static (_, _) => Task.FromResult(AbstractionsCommandExecutionResult.Success("Executed")),
            static (_, _) => Task.FromResult(AbstractionsCommandExecutionResult.Failure("Invalid request."))));

        AbstractionsCommandExecutionResult result =
            await engine.ExecuteAsync(new AbstractionsCommandRequest
            {
                CommandName = "Validation.Command"
            });

        Assert.False(result.Succeeded);
        Assert.Equal("Invalid request.", result.Message);
    }
}
