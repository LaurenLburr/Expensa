using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class WorkflowRuntimeActionTextFormatterTests
{
    [Fact]
    public void Format_ReturnsReadableActionSummary()
    {
        WorkflowRuntimeActionViewModel viewModel =
            new()
            {
                WorkflowExecutionId = "workflow-001",
                ActionName = "Abandon",
                Succeeded = true,
                Message = "Workflow was abandoned.",
                CompletedUtc = new DateTimeOffset(2035, 1, 2, 3, 4, 5, TimeSpan.Zero)
            };

        string text =
            WorkflowRuntimeActionTextFormatter.Format(viewModel);

        Assert.Contains("Workflow Runtime Action", text);
        Assert.Contains("Action: Abandon", text);
        Assert.Contains("Workflow Execution Id: workflow-001", text);
        Assert.Contains("Status: Succeeded", text);
        Assert.Contains("Workflow was abandoned.", text);
    }
}
