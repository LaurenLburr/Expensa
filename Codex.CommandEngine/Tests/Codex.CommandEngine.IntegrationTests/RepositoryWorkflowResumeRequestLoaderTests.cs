using Codex.CommandEngine.Core;
using Codex.CommandEngine.Data;
using Xunit;

namespace Codex.CommandEngine.IntegrationTests;

public sealed class RepositoryWorkflowResumeRequestLoaderTests
{
    [Fact]
    public void Load_WhenWorkflowIsResumable_ReturnsResumeExecutionRequest()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(RepositoryWorkflowResumeRequestLoaderTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            WorkflowExecutionRepository repository =
                new(TestDatabasePaths.CreateConnectionFactory(databasePath));

            repository.StartWorkflow(new WorkflowExecutionStart
            {
                WorkflowExecutionId = "workflow-load-001",
                WorkflowName = "load.workflow",
                CorrelationId = "load-correlation-001",
                ContextJson = "{\"original\":true}"
            });

            repository.UpdateResumeState(new WorkflowResumeUpdate
            {
                WorkflowExecutionId = "workflow-load-001",
                LastCompletedStepOrder = 1,
                RuntimeStateJson = "{\"resume\":true}",
                ResumeToken = "resume-token-load-001",
                IsResumable = true
            });

            RepositoryWorkflowResumeRequestLoader loader = new(repository);

            WorkflowResumeExecutionRequest request =
                loader.Load(
                    "workflow-load-001",
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
                    ]);

            Assert.Equal("{\"resume\":true}", request.ContextJson);
            Assert.Equal("workflow-load-001", request.ResumeRequest.WorkflowExecutionId);
            Assert.Equal("load.workflow", request.ResumeRequest.WorkflowName);
            Assert.Equal("load-correlation-001", request.ResumeRequest.CorrelationId);
            Assert.Equal(1, request.ResumeRequest.LastCompletedStepOrder);
            Assert.Equal(2, request.ResumeRequest.Steps.Count);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void Load_WhenWorkflowIsCompleted_Throws()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(RepositoryWorkflowResumeRequestLoaderTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            WorkflowExecutionRepository repository =
                new(TestDatabasePaths.CreateConnectionFactory(databasePath));

            repository.StartWorkflow(new WorkflowExecutionStart
            {
                WorkflowExecutionId = "workflow-load-002",
                WorkflowName = "load.workflow",
                CorrelationId = "load-correlation-002"
            });

            repository.CompleteWorkflow(new WorkflowExecutionCompletion
            {
                WorkflowExecutionId = "workflow-load-002",
                Status = "Succeeded",
                Message = "Done"
            });

            RepositoryWorkflowResumeRequestLoader loader = new(repository);

            Assert.Throws<WorkflowResumeLoadException>(
                () => loader.Load("workflow-load-002", []));
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void Load_WhenWorkflowIsMissing_Throws()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(RepositoryWorkflowResumeRequestLoaderTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            WorkflowExecutionRepository repository =
                new(TestDatabasePaths.CreateConnectionFactory(databasePath));

            RepositoryWorkflowResumeRequestLoader loader = new(repository);

            Assert.Throws<WorkflowResumeLoadException>(
                () => loader.Load("missing-workflow", []));
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }
}
