using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class ExtensionCommandRuntimeTests
{
    [Fact]
    public void Create_WithRegistrations_ReturnsListableCommands()
    {
        IExtensionCommandRuntime runtime =
            ExtensionCommandRuntimeFactory.CreateRequired(
            [
                new RuntimeCommandRegistration
                {
                    Handler = new TestCommandHandler("extension.open"),
                    DisplayName = "Open Extension",
                    Category = "Extension"
                }
            ]);

        IReadOnlyList<RuntimeCommandDescriptor> commands =
            runtime.ListCommands();

        Assert.Single(commands);
        Assert.Equal("extension.open", commands[0].CommandName);
        Assert.Equal("Open Extension", commands[0].DisplayName);
    }

    [Fact]
    public void TryGetCommand_WhenCommandExists_ReturnsDescriptor()
    {
        IExtensionCommandRuntime runtime =
            ExtensionCommandRuntimeFactory.CreateRequired(
            [
                new RuntimeCommandRegistration
                {
                    Handler = new TestCommandHandler("extension.inspect"),
                    DisplayName = "Inspect Extension",
                    Category = "Extension"
                }
            ]);

        bool found =
            runtime.TryGetCommand(
                "extension.inspect",
                out RuntimeCommandDescriptor descriptor);

        Assert.True(found);
        Assert.Equal("Inspect Extension", descriptor.DisplayName);
    }

    [Fact]
    public async Task ExecuteCommandAsync_WhenCommandExists_ReturnsSuccess()
    {
        IExtensionCommandRuntime runtime =
            ExtensionCommandRuntimeFactory.CreateRequired(
            [
                new RuntimeCommandRegistration
                {
                    Handler = new TestCommandHandler("extension.run"),
                    DisplayName = "Run Extension",
                    Category = "Extension"
                }
            ]);

        CommandExecutionResult result =
            await runtime.ExecuteCommandAsync(new ExtensionRuntimeCommandRequest
            {
                CommandName = "extension.run",
                CorrelationId = "correlation-001",
                Parameters = new Dictionary<string, object?>
                {
                    ["source"] = "ExtensionMgr"
                }
            });

        Assert.Equal(CommandExecutionStatus.Succeeded, result.Status);
        Assert.Equal("correlation-001", result.CorrelationId);
    }

    [Fact]
    public async Task ExecuteWorkflowAsync_WhenWorkflowStepsAreRegistered_ReturnsSuccess()
    {
        IExtensionCommandRuntime runtime =
            ExtensionCommandRuntimeFactory.CreateRequired(
            [
                new RuntimeCommandRegistration
                {
                    Handler = new TestCommandHandler("extension.step.one"),
                    DisplayName = "Step One",
                    Category = "Extension"
                },
                new RuntimeCommandRegistration
                {
                    Handler = new TestCommandHandler("extension.step.two"),
                    DisplayName = "Step Two",
                    Category = "Extension"
                }
            ]);

        WorkflowExecutionResult result =
            await runtime.ExecuteWorkflowAsync(new ExtensionRuntimeWorkflowRequest
            {
                WorkflowName = "extension.workflow",
                CorrelationId = "correlation-001",
                Steps =
                [
                    new WorkflowStepExecutionRequest
                    {
                        StepName = "One",
                        StepOrder = 1,
                        CommandName = "extension.step.one"
                    },
                    new WorkflowStepExecutionRequest
                    {
                        StepName = "Two",
                        StepOrder = 2,
                        CommandName = "extension.step.two"
                    }
                ]
            });

        Assert.Equal(WorkflowExecutionStatus.Succeeded, result.Status);
        Assert.Equal("extension.workflow", result.WorkflowName);
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
