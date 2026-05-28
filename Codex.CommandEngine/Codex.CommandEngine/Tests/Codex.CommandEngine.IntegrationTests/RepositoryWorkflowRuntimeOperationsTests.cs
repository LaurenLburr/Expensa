using Codex.CommandEngine.Core;
using Codex.CommandEngine.Data;
using Xunit;

namespace Codex.CommandEngine.IntegrationTests;

public sealed class RepositoryWorkflowRuntimeOperationsTests
{
    [Fact]
    public void ListResumable_ReturnsWorkflowRuntimeSummaries()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(RepositoryWorkflowRuntimeOperationsTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            WorkflowExecutionRepository repository =
                new(TestDatabasePaths.CreateConnectionFactory(databasePath));

            repository.StartWorkflow(new WorkflowExecutionStart
            {
                WorkflowExecutionId = "runtime-workflow-001",
                WorkflowName = "runtime.workflow",
                CorrelationId = "runtime-correlation-001"
            });

            RepositoryWorkflowRuntimeOperations operations = new(repository);

            WorkflowRuntimeSummary summary =
                Assert.Single(operations.ListResumable());

            Assert.Equal("runtime-workflow-001", summary.WorkflowExecutionId);
            Assert.Equal("runtime.workflow", summary.WorkflowName);
            Assert.Equal(WorkflowRuntimeOperationStatus.Resumable, summary.OperationStatus);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void GetDetail_ReturnsSummaryAndSteps()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(RepositoryWorkflowRuntimeOperationsTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            WorkflowExecutionRepository repository =
                new(TestDatabasePaths.CreateConnectionFactory(databasePath));

            repository.StartWorkflow(new WorkflowExecutionStart
            {
                WorkflowExecutionId = "runtime-workflow-002",
                WorkflowName = "runtime.workflow",
                CorrelationId = "runtime-correlation-002"
            });

            repository.StartStep(new WorkflowStepExecutionStart
            {
                WorkflowStepExecutionId = "runtime-step-001",
                WorkflowExecutionId = "runtime-workflow-002",
                StepName = "First",
                StepOrder = 1,
                CommandName = "first.command"
            });

            repository.CompleteStep(new WorkflowStepExecutionCompletion
            {
                WorkflowStepExecutionId = "runtime-step-001",
                Status = "Succeeded",
                Message = "Done",
                CommandExecutionId = "command-execution-001"
            });

            RepositoryWorkflowRuntimeOperations operations = new(repository);

            WorkflowRuntimeDetail detail =
                operations.GetDetail("runtime-workflow-002");

            Assert.Equal("runtime-workflow-002", detail.Summary.WorkflowExecutionId);
            Assert.Single(detail.Steps);
            Assert.Equal("First", detail.Steps[0].StepName);
            Assert.Equal("command-execution-001", detail.Steps[0].CommandExecutionId);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void MarkAbandoned_RemovesWorkflowFromResumableOperations()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(RepositoryWorkflowRuntimeOperationsTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            WorkflowExecutionRepository repository =
                new(TestDatabasePaths.CreateConnectionFactory(databasePath));

            repository.StartWorkflow(new WorkflowExecutionStart
            {
                WorkflowExecutionId = "runtime-workflow-003",
                WorkflowName = "runtime.workflow",
                CorrelationId = "runtime-correlation-003"
            });

            RepositoryWorkflowRuntimeOperations operations = new(repository);

            operations.MarkAbandoned("runtime-workflow-003", "Operator abandoned workflow.");

            Assert.Empty(operations.ListResumable());

            WorkflowRuntimeDetail detail =
                operations.GetDetail("runtime-workflow-003");

            Assert.Equal("Abandoned", detail.Summary.Status);
            Assert.Equal(WorkflowRuntimeOperationStatus.Abandoned, detail.Summary.OperationStatus);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void Heartbeat_UpdatesRuntimeSummaryHeartbeat()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(RepositoryWorkflowRuntimeOperationsTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            WorkflowExecutionRepository repository =
                new(TestDatabasePaths.CreateConnectionFactory(databasePath));

            repository.StartWorkflow(new WorkflowExecutionStart
            {
                WorkflowExecutionId = "runtime-workflow-004",
                WorkflowName = "runtime.workflow",
                CorrelationId = "runtime-correlation-004"
            });

            RepositoryWorkflowRuntimeOperations operations = new(repository);

            DateTimeOffset heartbeat =
                new(2031, 2, 3, 4, 5, 6, TimeSpan.Zero);

            operations.Heartbeat("runtime-workflow-004", heartbeat);

            WorkflowRuntimeDetail detail =
                operations.GetDetail("runtime-workflow-004");

            Assert.Equal(heartbeat.ToString("O"), detail.Summary.LastHeartbeatUtc);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }
}
