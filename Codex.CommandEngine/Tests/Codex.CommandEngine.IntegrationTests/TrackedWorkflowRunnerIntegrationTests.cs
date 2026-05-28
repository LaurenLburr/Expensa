using System.Text.Json;
using Codex.CommandEngine.Data;
using CoreIWorkflowExecutionHistorySink = Codex.CommandEngine.Core.IWorkflowExecutionHistorySink;
using CoreIWorkflowRunner = Codex.CommandEngine.Core.IWorkflowRunner;
using CoreTrackedWorkflowRunner = Codex.CommandEngine.Core.TrackedWorkflowRunner;
using CoreWorkflowExecutionRequest = Codex.CommandEngine.Core.WorkflowExecutionRequest;
using CoreWorkflowExecutionResult = Codex.CommandEngine.Core.WorkflowExecutionResult;
using CoreWorkflowExecutionStatus = Codex.CommandEngine.Core.WorkflowExecutionStatus;
using Xunit;

namespace Codex.CommandEngine.IntegrationTests;

public sealed class TrackedWorkflowRunnerIntegrationTests
{
    [Fact]
    public async Task ExecuteAsync_WhenWorkflowSucceeds_PersistsWorkflowExecutionHistory()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(TrackedWorkflowRunnerIntegrationTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            CommandEngineConnectionFactory factory =
                new(CommandEngineDatabaseOptions.ForFile(databasePath));

            ExecutionHistoryRepository repository =
                new(factory);

            CoreTrackedWorkflowRunner runner =
                new(
                    new SuccessWorkflowRunner(),
                    new TestWorkflowExecutionHistorySink(repository, factory));

            CoreWorkflowExecutionResult result =
                await runner.ExecuteAsync(new CoreWorkflowExecutionRequest
                {
                    WorkflowName = "sample.workflow",
                    CorrelationId = "workflow-correlation-001"
                });

            Assert.Equal(CoreWorkflowExecutionStatus.Succeeded, result.Status);

            ExecutionHistoryRecord record =
                Assert.Single(repository.ListRecent());

            Assert.Equal("workflow-correlation-001", record.CorrelationId);
            Assert.Equal("sample.workflow", record.CommandName);
            Assert.Equal("Succeeded", record.Status);
            Assert.Equal("Workflow", record.ExecutionKind);
            Assert.Equal("sample.workflow", record.TargetName);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public async Task ExecuteAsync_WhenWorkflowFails_PersistsFailureExecutionHistory()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(TrackedWorkflowRunnerIntegrationTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            CommandEngineConnectionFactory factory =
                new(CommandEngineDatabaseOptions.ForFile(databasePath));

            ExecutionHistoryRepository repository =
                new(factory);

            CoreTrackedWorkflowRunner runner =
                new(
                    new FailedWorkflowRunner(),
                    new TestWorkflowExecutionHistorySink(repository, factory));

            CoreWorkflowExecutionResult result =
                await runner.ExecuteAsync(new CoreWorkflowExecutionRequest
                {
                    WorkflowName = "failed.workflow",
                    CorrelationId = "workflow-correlation-002"
                });

            Assert.Equal(CoreWorkflowExecutionStatus.Failed, result.Status);

            ExecutionHistoryRecord record =
                Assert.Single(repository.ListRecent());

            Assert.Equal("failed.workflow", record.CommandName);
            Assert.Equal("Failed", record.Status);
            Assert.Equal("Workflow failed.", record.Message);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    private sealed class TestWorkflowExecutionHistorySink : CoreIWorkflowExecutionHistorySink
    {
        private readonly ExecutionHistoryRepository _repository;
        private readonly CommandEngineConnectionFactory _factory;

        public TestWorkflowExecutionHistorySink(
            ExecutionHistoryRepository repository,
            CommandEngineConnectionFactory factory)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(factory);

            _repository = repository;
            _factory = factory;
        }

        public void Started(
            CoreWorkflowExecutionRequest request,
            string workflowExecutionId,
            DateTimeOffset startedUtc)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentException.ThrowIfNullOrWhiteSpace(workflowExecutionId);

            _repository.Start(new ExecutionHistoryStart
            {
                ExecutionId = workflowExecutionId,
                CorrelationId = request.CorrelationId,
                CommandName = request.WorkflowName,
                RequestJson = JsonSerializer.Serialize(new
                {
                    request.WorkflowName,
                    request.CorrelationId,
                    request.ContextJson,
                    request.Steps
                }),
                StartedUtc = startedUtc.ToString("O")
            });

            DataTableQueryExecutor executor = new(_factory);

            executor.ExecuteNonQuery(
                """
                UPDATE ExecutionHistory
                SET ExecutionKind = 'Workflow',
                    TargetId = $TargetId,
                    TargetName = $TargetName
                WHERE ExecutionId = $ExecutionId;
                """,
                new Dictionary<string, object?>
                {
                    ["ExecutionId"] = workflowExecutionId,
                    ["TargetId"] = request.WorkflowName,
                    ["TargetName"] = request.WorkflowName
                });
        }

        public void Completed(
            CoreWorkflowExecutionRequest request,
            CoreWorkflowExecutionResult result,
            string workflowExecutionId,
            DateTimeOffset completedUtc,
            long durationMilliseconds)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(result);
            ArgumentException.ThrowIfNullOrWhiteSpace(workflowExecutionId);

            _repository.Complete(new ExecutionHistoryCompletion
            {
                ExecutionId = workflowExecutionId,
                Status = result.Status.ToString(),
                CompletedUtc = completedUtc.ToString("O"),
                DurationMilliseconds = durationMilliseconds,
                OutputJson = JsonSerializer.Serialize(new
                {
                    result.WorkflowName,
                    result.CorrelationId,
                    result.Status,
                    result.Message,
                    result.StepResults
                }),
                Message = result.Message,
                ExceptionText = result.Status == CoreWorkflowExecutionStatus.Failed
                    ? result.Message
                    : null
            });
        }
    }

    private sealed class SuccessWorkflowRunner : CoreIWorkflowRunner
    {
        public Task<CoreWorkflowExecutionResult> ExecuteAsync(
            CoreWorkflowExecutionRequest request,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new CoreWorkflowExecutionResult
            {
                WorkflowName = request.WorkflowName,
                CorrelationId = request.CorrelationId,
                Status = CoreWorkflowExecutionStatus.Succeeded,
                Message = "Workflow completed successfully."
            });
        }
    }

    private sealed class FailedWorkflowRunner : CoreIWorkflowRunner
    {
        public Task<CoreWorkflowExecutionResult> ExecuteAsync(
            CoreWorkflowExecutionRequest request,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new CoreWorkflowExecutionResult
            {
                WorkflowName = request.WorkflowName,
                CorrelationId = request.CorrelationId,
                Status = CoreWorkflowExecutionStatus.Failed,
                Message = "Workflow failed."
            });
        }
    }
}
