using Codex.CommandEngine.Data;
using Xunit;

namespace Codex.CommandEngine.IntegrationTests;

public sealed class WorkflowDefinitionRepositoryTests
{
    [Fact]
    public void UpsertWorkflowAndSteps_ThenFindByName_ReturnsDefinition()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(WorkflowDefinitionRepositoryTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            WorkflowDefinitionRepository repository =
                new(TestDatabasePaths.CreateConnectionFactory(databasePath));

            repository.UpsertWorkflow(new WorkflowDefinitionUpsert
            {
                WorkflowDefinitionId = "workflow-definition-001",
                WorkflowName = "definition.workflow",
                DisplayName = "Definition Workflow",
                Description = "Test workflow definition.",
                Version = 2,
                IsActive = true
            });

            repository.UpsertStep(new WorkflowDefinitionStepUpsert
            {
                WorkflowDefinitionStepId = "workflow-step-definition-001",
                WorkflowDefinitionId = "workflow-definition-001",
                StepName = "First",
                StepOrder = 1,
                CommandName = "first.command",
                ParametersJson = "{\"value\":42}"
            });

            repository.UpsertStep(new WorkflowDefinitionStepUpsert
            {
                WorkflowDefinitionStepId = "workflow-step-definition-002",
                WorkflowDefinitionId = "workflow-definition-001",
                StepName = "Second",
                StepOrder = 2,
                CommandName = "second.command"
            });

            var definition =
                repository.FindByName("definition.workflow");

            Assert.NotNull(definition);
            Assert.Equal("definition.workflow", definition!.WorkflowName);
            Assert.Equal(2, definition.Version);
            Assert.Equal(2, definition.Steps.Count);
            Assert.Equal("First", definition.Steps[0].StepName);
            Assert.Equal("second.command", definition.Steps[1].CommandName);
            Assert.True(definition.Steps[0].Parameters.ContainsKey("value"));
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public void ListActive_ExcludesInactiveDefinitions()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(WorkflowDefinitionRepositoryTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            WorkflowDefinitionRepository repository =
                new(TestDatabasePaths.CreateConnectionFactory(databasePath));

            repository.UpsertWorkflow(new WorkflowDefinitionUpsert
            {
                WorkflowDefinitionId = "workflow-definition-active",
                WorkflowName = "active.workflow",
                IsActive = true
            });

            repository.UpsertWorkflow(new WorkflowDefinitionUpsert
            {
                WorkflowDefinitionId = "workflow-definition-inactive",
                WorkflowName = "inactive.workflow",
                IsActive = false
            });

            var active =
                repository.ListActive();

            Assert.Single(active);
            Assert.Equal("active.workflow", active[0].WorkflowName);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }
}
