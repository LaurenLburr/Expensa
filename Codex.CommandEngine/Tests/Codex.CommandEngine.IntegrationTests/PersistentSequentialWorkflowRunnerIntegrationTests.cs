using Codex.CommandEngine.Core;
using Codex.CommandEngine.Data;
using Xunit;

namespace Codex.CommandEngine.IntegrationTests;

public sealed class PersistentSequentialWorkflowRunnerIntegrationTests
{
    [Fact]
    public async Task ExecuteAsync_WhenWorkflowSucceeds_PersistsWorkflowAndSteps()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(PersistentSequentialWorkflowRunnerIntegrationTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            CommandEngineConnectionFactory factory =
                new(CommandEngineDatabaseOptions.ForFile(databasePath));

            WorkflowExecutionRepository repository = new(factory);
            CommandDispatcher dispatcher = new();

            dispatcher.Register(new SuccessHandler("first.command"));
            dispatcher.Register(new SuccessHandler("second.command"));

            RepositoryWorkflowExecutionStore store = new(repository);

            PersistentSequentialWorkflowRunner runner =
                new(dispatcher, store);

            WorkflowExecutionResult result =
                await runner.ExecuteAsync(new WorkflowExecutionRequest
                {
                    WorkflowName = "persistent.workflow",
                    CorrelationId = "persistent-correlation-001",
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

            Assert.Equal(WorkflowExecutionStatus.Succeeded, result.Status);

            WorkflowExecutionRecord? workflow =
                repository.FindWorkflow(store.WorkflowExecutionId);

            Assert.NotNull(workflow);
            Assert.Equal("Succeeded", workflow!.Status);

            IReadOnlyList<WorkflowStepExecutionRecord> steps =
                repository.ListSteps(store.WorkflowExecutionId);

            Assert.Equal(2, steps.Count);
            Assert.Equal("Succeeded", steps[0].Status);
            Assert.Equal("Succeeded", steps[1].Status);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public async Task ExecuteAsync_WhenWorkflowFails_PersistsFailedWorkflowAndStopsAtFailedStep()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(PersistentSequentialWorkflowRunnerIntegrationTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            CommandEngineConnectionFactory factory =
                new(CommandEngineDatabaseOptions.ForFile(databasePath));

            WorkflowExecutionRepository repository = new(factory);
            CommandDispatcher dispatcher = new();

            dispatcher.Register(new SuccessHandler("first.command"));
            dispatcher.Register(new FailureHandler("fail.command"));
            dispatcher.Register(new SuccessHandler("never.command"));

            RepositoryWorkflowExecutionStore store = new(repository);

            PersistentSequentialWorkflowRunner runner =
                new(dispatcher, store);

            WorkflowExecutionResult result =
                await runner.ExecuteAsync(new WorkflowExecutionRequest
                {
                    WorkflowName = "failed.persistent.workflow",
                    CorrelationId = "persistent-correlation-002",
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
                            StepName = "Fail",
                            CommandName = "fail.command",
                            StepOrder = 2
                        },
                        new WorkflowStepExecutionRequest
                        {
                            StepName = "Never",
                            CommandName = "never.command",
                            StepOrder = 3
                        }
                    ]
                });

            Assert.Equal(WorkflowExecutionStatus.Failed, result.Status);
            Assert.Equal(2, result.StepResults.Count);

            WorkflowExecutionRecord? workflow =
                repository.FindWorkflow(store.WorkflowExecutionId);

            Assert.NotNull(workflow);
            Assert.Equal("Failed", workflow!.Status);

            IReadOnlyList<WorkflowStepExecutionRecord> steps =
                repository.ListSteps(store.WorkflowExecutionId);

            Assert.Equal(2, steps.Count);
            Assert.Equal("Succeeded", steps[0].Status);
            Assert.Equal("Failed", steps[1].Status);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    private sealed class RepositoryWorkflowExecutionStore : IWorkflowExecutionStore
    {
        private readonly WorkflowExecutionRepository _repository;

        public string WorkflowExecutionId { get; private set; } = string.Empty;

        public RepositoryWorkflowExecutionStore(WorkflowExecutionRepository repository)
        {
            ArgumentNullException.ThrowIfNull(repository);
            _repository = repository;
        }

        public void StartWorkflow(
            string workflowExecutionId,
            WorkflowExecutionRequest request,
            DateTimeOffset startedUtc)
        {
            WorkflowExecutionId = workflowExecutionId;

            _repository.StartWorkflow(new WorkflowExecutionStart
            {
                WorkflowExecutionId = workflowExecutionId,
                WorkflowName = request.WorkflowName,
                CorrelationId = request.CorrelationId,
                StartedUtc = startedUtc.ToString("O"),
                ContextJson = request.ContextJson
            });
        }

        public void CompleteWorkflow(
            string workflowExecutionId,
            WorkflowExecutionResult result,
            DateTimeOffset completedUtc)
        {
            _repository.CompleteWorkflow(new WorkflowExecutionCompletion
            {
                WorkflowExecutionId = workflowExecutionId,
                Status = result.Status.ToString(),
                CompletedUtc = completedUtc.ToString("O"),
                Message = result.Message
            });
        }

        public void StartStep(
            string workflowStepExecutionId,
            string workflowExecutionId,
            WorkflowStepExecutionRequest step,
            DateTimeOffset startedUtc)
        {
            _repository.StartStep(new WorkflowStepExecutionStart
            {
                WorkflowStepExecutionId = workflowStepExecutionId,
                WorkflowExecutionId = workflowExecutionId,
                StepName = step.StepName,
                StepOrder = step.StepOrder,
                CommandName = step.CommandName,
                StartedUtc = startedUtc.ToString("O")
            });
        }

        public void CompleteStep(
            string workflowStepExecutionId,
            WorkflowStepExecutionResult stepResult,
            string? commandExecutionId,
            DateTimeOffset completedUtc)
        {
            _repository.CompleteStep(new WorkflowStepExecutionCompletion
            {
                WorkflowStepExecutionId = workflowStepExecutionId,
                Status = stepResult.Status.ToString(),
                CompletedUtc = completedUtc.ToString("O"),
                Message = stepResult.Message,
                CommandExecutionId = commandExecutionId
            });
        }
    }

    private sealed class SuccessHandler : ICommandHandler
    {
        public SuccessHandler(string commandName)
        {
            CommandName = commandName;
        }

        public string CommandName { get; }

        public Task<CommandExecutionResult> ExecuteAsync(
            CommandExecutionRequest request,
            ICommandExecutionContext context)
        {
            return Task.FromResult(CommandExecutionResult.Succeeded(
                request.CommandName,
                request.CorrelationId,
                "OK"));
        }
    }

    private sealed class FailureHandler : ICommandHandler
    {
        public FailureHandler(string commandName)
        {
            CommandName = commandName;
        }

        public string CommandName { get; }

        public Task<CommandExecutionResult> ExecuteAsync(
            CommandExecutionRequest request,
            ICommandExecutionContext context)
        {
            return Task.FromResult(CommandExecutionResult.Failed(
                request.CommandName,
                request.CorrelationId,
                "Failed."));
        }
    }
}
