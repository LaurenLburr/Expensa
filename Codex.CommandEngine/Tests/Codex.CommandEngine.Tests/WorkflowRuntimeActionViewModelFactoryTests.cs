using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class WorkflowRuntimeActionViewModelFactoryTests
{
    [Fact]
    public void Create_WhenResultSucceeded_ReturnsSucceededViewModel()
    {
        WorkflowRuntimeActionViewModelFactory factory = new();

        DateTimeOffset completedUtc =
            new(2035, 1, 2, 3, 4, 5, TimeSpan.Zero);

        WorkflowRuntimeActionViewModel viewModel =
            factory.Create(
                "Abandon",
                WorkflowRuntimeActionResult.Success(
                    "workflow-001",
                    "Workflow was abandoned."),
                completedUtc);

        Assert.Equal("workflow-001", viewModel.WorkflowExecutionId);
        Assert.Equal("Abandon", viewModel.ActionName);
        Assert.True(viewModel.Succeeded);
        Assert.Equal("Succeeded", viewModel.DisplayStatus);
        Assert.Equal(completedUtc, viewModel.CompletedUtc);
    }

    [Fact]
    public void Create_WhenResultFailed_ReturnsFailedViewModel()
    {
        WorkflowRuntimeActionViewModelFactory factory = new();

        WorkflowRuntimeActionViewModel viewModel =
            factory.Create(
                "Heartbeat",
                WorkflowRuntimeActionResult.Failure(
                    "workflow-002",
                    "Cannot update heartbeat."),
                DateTimeOffset.UtcNow);

        Assert.False(viewModel.Succeeded);
        Assert.Equal("Failed", viewModel.DisplayStatus);
        Assert.Equal("Cannot update heartbeat.", viewModel.Message);
    }
}
