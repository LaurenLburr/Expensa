using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class WorkflowRuntimeActionPresenterTests
{
    [Fact]
    public void Present_ReturnsFormattedActionText()
    {
        WorkflowRuntimeActionPresenter presenter =
            new(new WorkflowRuntimeActionViewModelFactory());

        string text =
            presenter.Present(
                "Abandon Workflow",
                WorkflowRuntimeActionResult.Success(
                    "workflow-001",
                    "Workflow was abandoned."),
                new DateTimeOffset(2035, 1, 2, 3, 4, 5, TimeSpan.Zero));

        Assert.Contains("Action: Abandon Workflow", text);
        Assert.Contains("Workflow Execution Id: workflow-001", text);
        Assert.Contains("Status: Succeeded", text);
        Assert.Contains("Workflow was abandoned.", text);
    }
}
