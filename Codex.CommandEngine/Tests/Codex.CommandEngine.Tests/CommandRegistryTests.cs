using AbstractionsCommandDefinition = Codex.CommandEngine.Abstractions.CommandDefinition;
using AbstractionsCommandExecutionResult = Codex.CommandEngine.Abstractions.CommandExecutionResult;
using AbstractionsCommandHandlerDescriptor = Codex.CommandEngine.Abstractions.CommandHandlerDescriptor;
using AbstractionsCommandRequest = Codex.CommandEngine.Abstractions.CommandRequest;
using AbstractionsICommandHandler = Codex.CommandEngine.Abstractions.ICommandHandler;
using CoreCommandRegistry = Codex.CommandEngine.Core.CommandRegistry;
using CoreDelegateCommandHandler = Codex.CommandEngine.Core.DelegateCommandHandler;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class CommandRegistryTests
{
    [Fact]
    public void Register_Should_Reject_Null_Handler()
    {
        CoreCommandRegistry registry = new();

        Assert.Throws<ArgumentNullException>(() => registry.Register(null!));
    }

    [Fact]
    public void Register_Should_Reject_Duplicate_Command_Name()
    {
        CoreCommandRegistry registry = new();
        AbstractionsICommandHandler handler = CreateHandler("Test.Command");

        registry.Register(handler);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(() => registry.Register(handler));

        Assert.Contains("already registered", exception.Message);
    }

    [Fact]
    public void TryResolve_Should_Return_Registered_Handler_Case_Insensitive()
    {
        CoreCommandRegistry registry = new();
        AbstractionsICommandHandler handler = CreateHandler("Test.Command");

        registry.Register(handler);

        bool resolved =
            registry.TryResolve("test.command", out AbstractionsICommandHandler resolvedHandler);

        Assert.True(resolved);
        Assert.Same(handler, resolvedHandler);
    }

    [Fact]
    public void ListHandlers_Should_Return_Handlers_Ordered_By_Category_Then_Command_Name()
    {
        CoreCommandRegistry registry = new();
        registry.Register(CreateHandler("Zeta.Command", "B"));
        registry.Register(CreateHandler("Alpha.Command", "A"));

        IReadOnlyList<AbstractionsCommandHandlerDescriptor> handlers =
            registry.ListHandlers();

        Assert.Equal("Alpha.Command", handlers[0].Definition.CommandName);
        Assert.Equal("Zeta.Command", handlers[1].Definition.CommandName);
    }

    private static AbstractionsICommandHandler CreateHandler(
        string commandName,
        string category = "Tests")
    {
        return new CoreDelegateCommandHandler(
            new AbstractionsCommandDefinition
            {
                CommandName = commandName,
                DisplayName = commandName,
                Category = category,
                HandlerType = typeof(CoreDelegateCommandHandler).FullName ?? nameof(CoreDelegateCommandHandler)
            },
            static (_, _) => Task.FromResult(AbstractionsCommandExecutionResult.Success("Executed")));
    }
}
