using Codex.CommandEngine.Data;
using Xunit;

namespace Codex.CommandEngine.IntegrationTests;

public sealed class WorkflowExecutionRepositoryTests
{
    [Fact]
    public void StartWorkflow_ThenFindWorkflow_ReturnsStartedWorkflow()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(WorkflowExecutionRepositoryTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            WorkflowExecutionRepository repository =
                new(new CommandEngineDatabaseOptionsFactory().CreateFactory(databasePath));

            repository.StartWorkflow(new WorkflowExecutionStart
            {
                WorkflowExecutionId = "workflow-execution-001",
                WorkflowName = "sample.workflow",
                CorrelationId = "correlation-001",
                ContextJson = "{\"source\":\"test\"}"
            });

            WorkflowExecutionRecord? record =
                repository.FindWorkflow("workflow-execution-001");

            Assert.NotNull(record);
            Assert.Equal("sample.workflow", record!.WorkflowName);
            Assert.Equal("Started", record.Status);
            Assert.Equal("correlation-001", record.CorrelationId);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void StartAndCompleteStep_ThenListSteps_ReturnsCompletedStep()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(WorkflowExecutionRepositoryTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            WorkflowExecutionRepository repository =
                new(new CommandEngineDatabaseOptionsFactory().CreateFactory(databasePath));

            repository.StartWorkflow(new WorkflowExecutionStart
            {
                WorkflowExecutionId = "workflow-execution-002",
                WorkflowName = "sample.workflow",
                CorrelationId = "correlation-002"
            });

            repository.StartStep(new WorkflowStepExecutionStart
            {
                WorkflowStepExecutionId = "workflow-step-execution-001",
                WorkflowExecutionId = "workflow-execution-002",
                StepName = "First Step",
                StepOrder = 1,
                CommandName = "first.command"
            });

            repository.CompleteStep(new WorkflowStepExecutionCompletion
            {
                WorkflowStepExecutionId = "workflow-step-execution-001",
                Status = "Succeeded",
                Message = "Step completed.",
                CommandExecutionId = "command-execution-001"
            });

            IReadOnlyList<WorkflowStepExecutionRecord> steps =
                repository.ListSteps("workflow-execution-002");

            Assert.Single(steps);
            Assert.Equal("Succeeded", steps[0].Status);
            Assert.Equal("command-execution-001", steps[0].CommandExecutionId);

            WorkflowExecutionRecord? workflow =
                repository.FindWorkflow("workflow-execution-002");

            Assert.NotNull(workflow);
            Assert.Equal(1, workflow!.CurrentStepOrder);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void ListIncomplete_ExcludesCompletedWorkflows()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(WorkflowExecutionRepositoryTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            WorkflowExecutionRepository repository =
                new(new CommandEngineDatabaseOptionsFactory().CreateFactory(databasePath));

            repository.StartWorkflow(new WorkflowExecutionStart
            {
                WorkflowExecutionId = "workflow-execution-003",
                WorkflowName = "incomplete.workflow",
                CorrelationId = "correlation-003"
            });

            repository.StartWorkflow(new WorkflowExecutionStart
            {
                WorkflowExecutionId = "workflow-execution-004",
                WorkflowName = "complete.workflow",
                CorrelationId = "correlation-004"
            });

            repository.CompleteWorkflow(new WorkflowExecutionCompletion
            {
                WorkflowExecutionId = "workflow-execution-004",
                Status = "Succeeded",
                Message = "Done"
            });

            IReadOnlyList<WorkflowExecutionRecord> incomplete =
                repository.ListIncomplete();

            Assert.Single(incomplete);
            Assert.Equal("workflow-execution-003", incomplete[0].WorkflowExecutionId);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    private sealed class CommandEngineDatabaseOptionsFactory
    {
        public CommandEngineConnectionFactory CreateFactory(string databasePath)
        {
            return new CommandEngineConnectionFactory(CommandEngineDatabaseOptions.ForFile(databasePath));
        }
    }
}
