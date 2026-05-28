using Codex.CommandEngine.Core;
using Codex.CommandEngine.Data;
using Xunit;

namespace Codex.CommandEngine.IntegrationTests;

public sealed class WorkflowRuntimeActionServiceIntegrationTests
{
    [Fact]
    public void Abandon_WhenWorkflowIsPersisted_UpdatesRepositoryState()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(WorkflowRuntimeActionServiceIntegrationTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            WorkflowExecutionRepository repository =
                new(TestDatabasePaths.CreateConnectionFactory(databasePath));

            repository.StartWorkflow(new WorkflowExecutionStart
            {
                WorkflowExecutionId = "action-workflow-001",
                WorkflowName = "action.workflow",
                CorrelationId = "action-correlation-001"
            });

            RepositoryWorkflowRuntimeOperations operations =
                new(repository);

            WorkflowRuntimeActionService service =
                new(operations);

            WorkflowRuntimeActionResult result =
                service.Abandon("action-workflow-001", "Integration abandon.");

            Assert.True(result.Succeeded);

            WorkflowRuntimeDetail detail =
                operations.GetDetail("action-workflow-001");

            Assert.Equal("Abandoned", detail.Summary.Status);
            Assert.Equal(WorkflowRuntimeOperationStatus.Abandoned, detail.Summary.OperationStatus);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void Heartbeat_WhenWorkflowIsPersisted_UpdatesRepositoryHeartbeat()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(WorkflowRuntimeActionServiceIntegrationTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            WorkflowExecutionRepository repository =
                new(TestDatabasePaths.CreateConnectionFactory(databasePath));

            repository.StartWorkflow(new WorkflowExecutionStart
            {
                WorkflowExecutionId = "action-workflow-002",
                WorkflowName = "action.workflow",
                CorrelationId = "action-correlation-002"
            });

            RepositoryWorkflowRuntimeOperations operations =
                new(repository);

            WorkflowRuntimeActionService service =
                new(operations);

            DateTimeOffset heartbeat =
                new(2035, 4, 5, 6, 7, 8, TimeSpan.Zero);

            WorkflowRuntimeActionResult result =
                service.Heartbeat("action-workflow-002", heartbeat);

            Assert.True(result.Succeeded);

            WorkflowRuntimeDetail detail =
                operations.GetDetail("action-workflow-002");

            Assert.Equal(heartbeat.ToString("O"), detail.Summary.LastHeartbeatUtc);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void Abandon_WhenWorkflowAlreadyCompleted_ReturnsFailureAndDoesNotChangeStatus()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(WorkflowRuntimeActionServiceIntegrationTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            WorkflowExecutionRepository repository =
                new(TestDatabasePaths.CreateConnectionFactory(databasePath));

            repository.StartWorkflow(new WorkflowExecutionStart
            {
                WorkflowExecutionId = "action-workflow-003",
                WorkflowName = "action.workflow",
                CorrelationId = "action-correlation-003"
            });

            repository.CompleteWorkflow(new WorkflowExecutionCompletion
            {
                WorkflowExecutionId = "action-workflow-003",
                Status = "Succeeded",
                Message = "Completed."
            });

            RepositoryWorkflowRuntimeOperations operations =
                new(repository);

            WorkflowRuntimeActionService service =
                new(operations);

            WorkflowRuntimeActionResult result =
                service.Abandon("action-workflow-003", "Too late.");

            Assert.False(result.Succeeded);

            WorkflowRuntimeDetail detail =
                operations.GetDetail("action-workflow-003");

            Assert.Equal("Succeeded", detail.Summary.Status);
            Assert.Equal(WorkflowRuntimeOperationStatus.Completed, detail.Summary.OperationStatus);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }
}
