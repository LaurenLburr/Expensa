using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class WorkflowHeartbeatEvaluatorTests
{
    [Fact]
    public void Evaluate_WhenHeartbeatIsRecent_ReturnsHealthy()
    {
        WorkflowHeartbeatEvaluator evaluator = new();

        DateTimeOffset now =
            new(2031, 1, 1, 12, 0, 0, TimeSpan.Zero);

        WorkflowHeartbeatEvaluation result =
            evaluator.Evaluate(
                CreateSummary(now.AddMinutes(-5).ToString("O")),
                now,
                TimeSpan.FromMinutes(10));

        Assert.Equal(WorkflowHeartbeatStatus.Healthy, result.Status);
        Assert.Equal(TimeSpan.FromMinutes(5), result.Age);
    }

    [Fact]
    public void Evaluate_WhenHeartbeatIsOld_ReturnsStale()
    {
        WorkflowHeartbeatEvaluator evaluator = new();

        DateTimeOffset now =
            new(2031, 1, 1, 12, 0, 0, TimeSpan.Zero);

        WorkflowHeartbeatEvaluation result =
            evaluator.Evaluate(
                CreateSummary(now.AddMinutes(-30).ToString("O")),
                now,
                TimeSpan.FromMinutes(10));

        Assert.Equal(WorkflowHeartbeatStatus.Stale, result.Status);
        Assert.Equal(TimeSpan.FromMinutes(30), result.Age);
    }

    [Fact]
    public void Evaluate_WhenHeartbeatMissing_ReturnsMissing()
    {
        WorkflowHeartbeatEvaluator evaluator = new();

        WorkflowHeartbeatEvaluation result =
            evaluator.Evaluate(
                CreateSummary(null),
                DateTimeOffset.UtcNow,
                TimeSpan.FromMinutes(10));

        Assert.Equal(WorkflowHeartbeatStatus.Missing, result.Status);
    }

    [Fact]
    public void Evaluate_WhenHeartbeatInvalid_ReturnsUnknown()
    {
        WorkflowHeartbeatEvaluator evaluator = new();

        WorkflowHeartbeatEvaluation result =
            evaluator.Evaluate(
                CreateSummary("not-a-date"),
                DateTimeOffset.UtcNow,
                TimeSpan.FromMinutes(10));

        Assert.Equal(WorkflowHeartbeatStatus.Unknown, result.Status);
    }

    private static WorkflowRuntimeSummary CreateSummary(string? heartbeatUtc)
    {
        return new WorkflowRuntimeSummary
        {
            WorkflowExecutionId = "workflow-001",
            WorkflowName = "heartbeat.workflow",
            CorrelationId = "correlation-001",
            Status = "Started",
            OperationStatus = WorkflowRuntimeOperationStatus.Resumable,
            CurrentStepOrder = 1,
            LastCompletedStepOrder = 0,
            IsResumable = true,
            LastHeartbeatUtc = heartbeatUtc,
            RuntimeStateJson = "{}"
        };
    }
}
