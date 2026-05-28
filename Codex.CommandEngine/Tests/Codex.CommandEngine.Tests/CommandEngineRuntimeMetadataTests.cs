using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class CommandEngineRuntimeMetadataTests
{
    [Fact]
    public void RegisterCommand_WithMetadata_AddsDescriptor()
    {
        CommandEngineRuntime runtime = new();

        runtime.RegisterCommand(new RuntimeCommandRegistration
        {
            Handler = new TestCommandHandler("extension.open"),
            DisplayName = "Open Extension",
            Description = "Opens an extension.",
            Category = "Extension",
            Version = 2,
            IsEnabled = true
        });

        bool found =
            runtime.TryGetRegisteredCommand(
                "extension.open",
                out RuntimeCommandDescriptor descriptor);

        Assert.True(found);
        Assert.Equal("extension.open", descriptor.CommandName);
        Assert.Equal("Open Extension", descriptor.DisplayName);
        Assert.Equal("Opens an extension.", descriptor.Description);
        Assert.Equal("Extension", descriptor.Category);
        Assert.Equal(2, descriptor.Version);
        Assert.True(descriptor.IsEnabled);
    }

    [Fact]
    public void RegisterCommands_WithMultipleRegistrations_AddsDescriptorsInCategoryThenNameOrder()
    {
        CommandEngineRuntime runtime = new();

        runtime.RegisterCommands(
        [
            new RuntimeCommandRegistration
            {
                Handler = new TestCommandHandler("zeta.command"),
                DisplayName = "Zeta",
                Category = "B"
            },
            new RuntimeCommandRegistration
            {
                Handler = new TestCommandHandler("alpha.command"),
                DisplayName = "Alpha",
                Category = "A"
            }
        ]);

        IReadOnlyList<RuntimeCommandDescriptor> commands =
            runtime.ListRegisteredCommands();

        Assert.Equal("alpha.command", commands[0].CommandName);
        Assert.Equal("zeta.command", commands[1].CommandName);
    }

    [Fact]
    public async Task ExecuteCommandAsync_WhenRegisteredCommandIsDisabled_ReturnsFailureButStillListsDescriptor()
    {
        CommandEngineRuntime runtime = new();

        runtime.RegisterCommand(new RuntimeCommandRegistration
        {
            Handler = new TestCommandHandler("disabled.command"),
            DisplayName = "Disabled Command",
            Category = "Extension",
            IsEnabled = false
        });

        IReadOnlyList<RuntimeCommandDescriptor> commands =
            runtime.ListRegisteredCommands();

        Assert.Single(commands);
        Assert.False(commands[0].IsEnabled);

        CommandExecutionResult result =
            await runtime.ExecuteCommandAsync(new CommandExecutionRequest
            {
                CommandName = "disabled.command",
                CorrelationId = "correlation-001"
            });

        Assert.Equal(CommandExecutionStatus.Failed, result.Status);
        Assert.Contains("No command handler is registered", result.Message);
    }

    [Fact]
    public void RegisterCommand_WhenVersionIsInvalid_Throws()
    {
        CommandEngineRuntime runtime = new();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => runtime.RegisterCommand(new RuntimeCommandRegistration
            {
                Handler = new TestCommandHandler("bad.version"),
                Version = 0
            }));
    }

    private sealed class TestCommandHandler : ICommandHandler
    {
        public TestCommandHandler(string commandName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(commandName);

            CommandName = commandName;
        }

        public string CommandName { get; }

        public Task<CommandExecutionResult> ExecuteAsync(
            CommandExecutionRequest request,
            ICommandExecutionContext context)
        {
            return Task.FromResult(
                CommandExecutionResult.Succeeded(
                    request.CommandName,
                    request.CorrelationId,
                    "Executed.",
                    "{\"ok\":true}"));
        }
    }
}
