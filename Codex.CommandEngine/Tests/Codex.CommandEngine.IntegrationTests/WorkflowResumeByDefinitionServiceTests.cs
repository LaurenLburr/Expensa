using Codex.CommandEngine.Core;
using Codex.CommandEngine.Data;
using Xunit;

namespace Codex.CommandEngine.IntegrationTests;

public sealed class WorkflowResumeByDefinitionServiceTests
{
    [Fact]
    public async Task ResumeAsync_LoadsDefinitionAndRunsRemainingSteps()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(WorkflowResumeByDefinitionServiceTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            CommandEngineConnectionFactory factory =
                TestDatabasePaths.CreateConnectionFactory(databasePath);

            WorkflowExecutionRepository workflowExecutionRepository =
                new(factory);

            WorkflowDefinitionRepository definitionRepository =
                new(factory);

            workflowExecutionRepository.StartWorkflow(new WorkflowExecutionStart
            {
                WorkflowExecutionId = "workflow-execution-001",
                WorkflowName = "resume.definition.workflow",
                CorrelationId = "correlation-001"
            });

            workflowExecutionRepository.UpdateResumeState(new WorkflowResumeUpdate
            {
                WorkflowExecutionId = "workflow-execution-001",
                LastCompletedStepOrder = 1,
                RuntimeStateJson = "{\"resume\":true}",
                ResumeToken = "resume-token-001"
            });

            definitionRepository.UpsertWorkflow(new WorkflowDefinitionUpsert
            {
                WorkflowDefinitionId = "workflow-definition-001",
                WorkflowName = "resume.definition.workflow",
                IsActive = true
            });

            definitionRepository.UpsertStep(new WorkflowDefinitionStepUpsert
            {
                WorkflowDefinitionStepId = "workflow-step-001",
                WorkflowDefinitionId = "workflow-definition-001",
                StepName = "First",
                StepOrder = 1,
                CommandName = "first.command"
            });

            definitionRepository.UpsertStep(new WorkflowDefinitionStepUpsert
            {
                WorkflowDefinitionStepId = "workflow-step-002",
                WorkflowDefinitionId = "workflow-definition-001",
                StepName = "Second",
                StepOrder = 2,
                CommandName = "second.command"
            });

            RecordingResumeService resumeService = new();

            WorkflowResumeByDefinitionService service =
                new(
                    new RepositoryWorkflowRuntimeOperations(workflowExecutionRepository),
                    new CoreWorkflowDefinitionStoreAdapter(definitionRepository),
                    resumeService);

            WorkflowExecutionResult result =
                await service.ResumeAsync(new WorkflowResumeByDefinitionRequest
                {
                    WorkflowExecutionId = "workflow-execution-001"
                });

            Assert.Equal(WorkflowExecutionStatus.Succeeded, result.Status);
            Assert.NotNull(resumeService.LastRequest);
            Assert.Equal("workflow-execution-001", resumeService.LastRequest!.WorkflowExecutionId);
            Assert.Equal(2, resumeService.LastRequest.WorkflowDefinitionSteps.Count);
            Assert.Equal("Second", resumeService.LastRequest.WorkflowDefinitionSteps[1].StepName);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    private sealed class RecordingResumeService : IWorkflowResumeService
    {
        public WorkflowResumeServiceRequest? LastRequest { get; private set; }

        public Task<WorkflowExecutionResult> ResumeAsync(
            WorkflowResumeServiceRequest request,
            CancellationToken cancellationToken = default)
        {
            LastRequest = request;

            return Task.FromResult(new WorkflowExecutionResult
            {
                WorkflowName = "resume.definition.workflow",
                CorrelationId = "correlation-001",
                Status = WorkflowExecutionStatus.Succeeded,
                Message = "Resumed."
            });
        }
    }
}
