using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class WorkflowRuntimeOperationsExtensionsTests
{
    [Fact]
    public void ListStale_ReturnsOnlyStaleOrMissingWorkflows()
    {
        DateTimeOffset now =
            new(2031, 1, 1, 12, 0, 0, TimeSpan.Zero);

        FakeWorkflowRuntimeOperations operations =
            new(
            [
                CreateSummary("healthy", now.AddMinutes(-1).ToString("O")),
                CreateSummary("stale", now.AddMinutes(-30).ToString("O")),
                CreateSummary("missing", null)
            ]);

        IReadOnlyList<WorkflowRuntimeSummary> stale =
            operations.ListStale(
                new WorkflowHeartbeatEvaluator(),
                now,
                TimeSpan.FromMinutes(10));

        Assert.Equal(2, stale.Count);
        Assert.Contains(stale, item => item.WorkflowExecutionId == "stale");
        Assert.Contains(stale, item => item.WorkflowExecutionId == "missing");
    }

    private static WorkflowRuntimeSummary CreateSummary(string id, string? heartbeatUtc)
    {
        return new WorkflowRuntimeSummary
        {
            WorkflowExecutionId = id,
            WorkflowName = "runtime.workflow",
            CorrelationId = "correlation-" + id,
            Status = "Started",
            OperationStatus = WorkflowRuntimeOperationStatus.Resumable,
            CurrentStepOrder = 1,
            LastCompletedStepOrder = 0,
            IsResumable = true,
            LastHeartbeatUtc = heartbeatUtc,
            RuntimeStateJson = "{}"
        };
    }

    private sealed class FakeWorkflowRuntimeOperations : IWorkflowRuntimeOperations
    {
        private readonly IReadOnlyList<WorkflowRuntimeSummary> _summaries;

        public FakeWorkflowRuntimeOperations(IReadOnlyList<WorkflowRuntimeSummary> summaries)
        {
            _summaries = summaries;
        }

        public IReadOnlyList<WorkflowRuntimeSummary> ListResumable()
        {
            return _summaries;
        }

        public IReadOnlyList<WorkflowRuntimeSummary> ListIncomplete()
        {
            return _summaries;
        }

        public WorkflowRuntimeDetail GetDetail(string workflowExecutionId)
        {
            throw new NotImplementedException();
        }

        public void MarkAbandoned(string workflowExecutionId, string message)
        {
            throw new NotImplementedException();
        }

        public void Heartbeat(string workflowExecutionId, DateTimeOffset heartbeatUtc)
        {
            throw new NotImplementedException();
        }
    }
}
