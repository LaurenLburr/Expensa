using Codex.CommandEngine.Data;
using Xunit;

namespace Codex.CommandEngine.IntegrationTests;

public sealed class WorkflowResumeRepositoryTests
{
    [Fact]
    public void ListResumable_WhenWorkflowStarted_ReturnsWorkflow()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(WorkflowResumeRepositoryTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            WorkflowExecutionRepository repository =
                new(TestDatabasePaths.CreateConnectionFactory(databasePath));

            repository.StartWorkflow(new WorkflowExecutionStart
            {
                WorkflowExecutionId = "workflow-resume-001",
                WorkflowName = "resume.workflow",
                CorrelationId = "resume-correlation-001"
            });

            IReadOnlyList<WorkflowResumeState> resumable =
                repository.ListResumable();

            WorkflowResumeState state = Assert.Single(resumable);

            Assert.Equal("workflow-resume-001", state.WorkflowExecutionId);
            Assert.True(state.IsResumable);
            Assert.Equal("Started", state.Status);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void UpdateResumeState_ThenFindResumeState_ReturnsUpdatedState()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(WorkflowResumeRepositoryTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            WorkflowExecutionRepository repository =
                new(TestDatabasePaths.CreateConnectionFactory(databasePath));

            repository.StartWorkflow(new WorkflowExecutionStart
            {
                WorkflowExecutionId = "workflow-resume-002",
                WorkflowName = "resume.workflow",
                CorrelationId = "resume-correlation-002"
            });

            repository.UpdateResumeState(new WorkflowResumeUpdate
            {
                WorkflowExecutionId = "workflow-resume-002",
                LastCompletedStepOrder = 3,
                RuntimeStateJson = "{\"checkpoint\":3}",
                ResumeToken = "resume-token-002",
                IsResumable = true
            });

            WorkflowResumeState? state =
                repository.FindResumeState("workflow-resume-002");

            Assert.NotNull(state);
            Assert.Equal(3, state!.LastCompletedStepOrder);
            Assert.Equal("resume-token-002", state.ResumeToken);
            Assert.Equal("{\"checkpoint\":3}", state.RuntimeStateJson);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void MarkAbandoned_RemovesWorkflowFromResumableList()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(WorkflowResumeRepositoryTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            WorkflowExecutionRepository repository =
                new(TestDatabasePaths.CreateConnectionFactory(databasePath));

            repository.StartWorkflow(new WorkflowExecutionStart
            {
                WorkflowExecutionId = "workflow-resume-003",
                WorkflowName = "resume.workflow",
                CorrelationId = "resume-correlation-003"
            });

            repository.MarkAbandoned("workflow-resume-003", "Stale workflow.");

            IReadOnlyList<WorkflowResumeState> resumable =
                repository.ListResumable();

            Assert.Empty(resumable);

            WorkflowExecutionRecord? workflow =
                repository.FindWorkflow("workflow-resume-003");

            Assert.NotNull(workflow);
            Assert.Equal("Abandoned", workflow!.Status);
            Assert.Equal("Stale workflow.", workflow.Message);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void Heartbeat_UpdatesLastHeartbeatUtc()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(WorkflowResumeRepositoryTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            WorkflowExecutionRepository repository =
                new(TestDatabasePaths.CreateConnectionFactory(databasePath));

            repository.StartWorkflow(new WorkflowExecutionStart
            {
                WorkflowExecutionId = "workflow-resume-004",
                WorkflowName = "resume.workflow",
                CorrelationId = "resume-correlation-004"
            });

            repository.Heartbeat("workflow-resume-004", "2030-01-01T00:00:00.0000000+00:00");

            WorkflowResumeState? state =
                repository.FindResumeState("workflow-resume-004");

            Assert.NotNull(state);
            Assert.Equal("2030-01-01T00:00:00.0000000+00:00", state!.LastHeartbeatUtc);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }
}
