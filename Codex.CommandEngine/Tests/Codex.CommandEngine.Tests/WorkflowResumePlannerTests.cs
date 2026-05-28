using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class WorkflowResumePlannerTests
{
    [Fact]
    public void CreateResumePlan_WhenPendingStepsExist_ReturnsNextStep()
    {
        WorkflowResumePlanner planner = new();

        WorkflowResumePlan plan =
            planner.CreateResumePlan(new WorkflowResumeRequest
            {
                WorkflowExecutionId = "workflow-execution-001",
                WorkflowName = "resume.workflow",
                CorrelationId = "correlation-001",
                LastCompletedStepOrder = 1,
                Steps =
                [
                    new WorkflowStepExecutionRequest
                    {
                        StepName = "First",
                        CommandName = "first.command",
                        StepOrder = 1
                    },
                    new WorkflowStepExecutionRequest
                    {
                        StepName = "Second",
                        CommandName = "second.command",
                        StepOrder = 2
                    },
                    new WorkflowStepExecutionRequest
                    {
                        StepName = "Third",
                        CommandName = "third.command",
                        StepOrder = 3
                    }
                ]
            });

        Assert.True(plan.CanResume);
        Assert.NotNull(plan.NextStep);
        Assert.Equal("Second", plan.NextStep!.StepName);
        Assert.Equal(1, plan.LastCompletedStepOrder);
    }

    [Fact]
    public void CreateResumePlan_WhenNoPendingStepsExist_ReturnsNonResumablePlan()
    {
        WorkflowResumePlanner planner = new();

        WorkflowResumePlan plan =
            planner.CreateResumePlan(new WorkflowResumeRequest
            {
                WorkflowExecutionId = "workflow-execution-002",
                WorkflowName = "resume.workflow",
                CorrelationId = "correlation-002",
                LastCompletedStepOrder = 2,
                Steps =
                [
                    new WorkflowStepExecutionRequest
                    {
                        StepName = "First",
                        CommandName = "first.command",
                        StepOrder = 1
                    },
                    new WorkflowStepExecutionRequest
                    {
                        StepName = "Second",
                        CommandName = "second.command",
                        StepOrder = 2
                    }
                ]
            });

        Assert.False(plan.CanResume);
        Assert.Null(plan.NextStep);
    }

    [Fact]
    public void ToWorkflowExecutionRequest_WhenPlanCanResume_ReturnsRemainingStepsOnly()
    {
        WorkflowResumePlanner planner = new();

        List<WorkflowStepExecutionRequest> steps =
        [
            new WorkflowStepExecutionRequest
            {
                StepName = "First",
                CommandName = "first.command",
                StepOrder = 1
            },
            new WorkflowStepExecutionRequest
            {
                StepName = "Second",
                CommandName = "second.command",
                StepOrder = 2
            },
            new WorkflowStepExecutionRequest
            {
                StepName = "Third",
                CommandName = "third.command",
                StepOrder = 3
            }
        ];

        WorkflowResumePlan plan =
            planner.CreateResumePlan(new WorkflowResumeRequest
            {
                WorkflowExecutionId = "workflow-execution-003",
                WorkflowName = "resume.workflow",
                CorrelationId = "correlation-003",
                LastCompletedStepOrder = 1,
                Steps = steps
            });

        WorkflowExecutionRequest request =
            plan.ToWorkflowExecutionRequest(steps, "{\"resume\":true}");

        Assert.Equal("resume.workflow", request.WorkflowName);
        Assert.Equal("correlation-003", request.CorrelationId);
        Assert.Equal("{\"resume\":true}", request.ContextJson);
        Assert.Equal(2, request.Steps.Count);
        Assert.Equal("Second", request.Steps[0].StepName);
        Assert.Equal("Third", request.Steps[1].StepName);
    }
}
