using System.Text.Json;
using Codex.CommandEngine.Data;
using CoreCommandDispatcher = Codex.CommandEngine.Core.CommandDispatcher;
using CoreCommandExecutionRequest = Codex.CommandEngine.Core.CommandExecutionRequest;
using CoreCommandExecutionResult = Codex.CommandEngine.Core.CommandExecutionResult;
using CoreCommandExecutionStatus = Codex.CommandEngine.Core.CommandExecutionStatus;
using CoreICommandExecutionContext = Codex.CommandEngine.Core.ICommandExecutionContext;
using CoreICommandExecutionHistorySink = Codex.CommandEngine.Core.ICommandExecutionHistorySink;
using CoreICommandHandler = Codex.CommandEngine.Core.ICommandHandler;
using CoreTrackedCommandDispatcher = Codex.CommandEngine.Core.TrackedCommandDispatcher;
using Xunit;

namespace Codex.CommandEngine.IntegrationTests;

public sealed class TrackedCommandDispatcherIntegrationTests
{
    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_PersistsExecutionHistory()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(TrackedCommandDispatcherIntegrationTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            CommandEngineConnectionFactory factory =
                new(CommandEngineDatabaseOptions.ForFile(databasePath));

            ExecutionHistoryRepository repository =
                new(factory);

            CoreTrackedCommandDispatcher dispatcher =
                new(
                    new CoreCommandDispatcher(),
                    new TestExecutionHistoryCommandSink(repository));

            dispatcher.Register(new SuccessHandler());

            CoreCommandExecutionResult result =
                await dispatcher.ExecuteAsync(new CoreCommandExecutionRequest
                {
                    CommandName = "success.command",
                    CorrelationId = "correlation-success"
                });

            Assert.Equal(CoreCommandExecutionStatus.Succeeded, result.Status);

            ExecutionHistoryRecord record =
                Assert.Single(repository.ListRecent());

            Assert.Equal("correlation-success", record.CorrelationId);
            Assert.Equal("success.command", record.CommandName);
            Assert.Equal("Succeeded", record.Status);
            Assert.NotNull(record.DurationMilliseconds);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_PersistsFailureHistory()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(TrackedCommandDispatcherIntegrationTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            CommandEngineConnectionFactory factory =
                new(CommandEngineDatabaseOptions.ForFile(databasePath));

            ExecutionHistoryRepository repository =
                new(factory);

            CoreTrackedCommandDispatcher dispatcher =
                new(
                    new CoreCommandDispatcher(),
                    new TestExecutionHistoryCommandSink(repository));

            dispatcher.Register(new ThrowingHandler());

            CoreCommandExecutionResult result =
                await dispatcher.ExecuteAsync(new CoreCommandExecutionRequest
                {
                    CommandName = "throw.command",
                    CorrelationId = "correlation-failure"
                });

            Assert.Equal(CoreCommandExecutionStatus.Failed, result.Status);

            ExecutionHistoryRecord record =
                Assert.Single(repository.ListRecent());

            Assert.Equal("correlation-failure", record.CorrelationId);
            Assert.Equal("throw.command", record.CommandName);
            Assert.Equal("Failed", record.Status);
            Assert.Contains("Boom", record.ErrorMessage);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    [Fact]
    public async Task ExecuteAsync_WhenCancellationRequested_PersistsCancelledHistory()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(TrackedCommandDispatcherIntegrationTests));

        try
        {
            CommandEngineSchema.EnsureCreated(databasePath);

            CommandEngineConnectionFactory factory =
                new(CommandEngineDatabaseOptions.ForFile(databasePath));

            ExecutionHistoryRepository repository =
                new(factory);

            CoreTrackedCommandDispatcher dispatcher =
                new(
                    new CoreCommandDispatcher(),
                    new TestExecutionHistoryCommandSink(repository));

            dispatcher.Register(new SuccessHandler());

            using CancellationTokenSource source = new();
            source.Cancel();

            CoreCommandExecutionResult result =
                await dispatcher.ExecuteAsync(
                    new CoreCommandExecutionRequest
                    {
                        CommandName = "success.command",
                        CorrelationId = "correlation-cancelled"
                    },
                    source.Token);

            Assert.Equal(CoreCommandExecutionStatus.Cancelled, result.Status);

            ExecutionHistoryRecord record =
                Assert.Single(repository.ListRecent());

            Assert.Equal("Cancelled", record.Status);
            Assert.Equal("correlation-cancelled", record.CorrelationId);
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }

    private sealed class TestExecutionHistoryCommandSink : CoreICommandExecutionHistorySink
    {
        private readonly ExecutionHistoryRepository _repository;

        public TestExecutionHistoryCommandSink(ExecutionHistoryRepository repository)
        {
            ArgumentNullException.ThrowIfNull(repository);
            _repository = repository;
        }

        public void Started(
            CoreCommandExecutionRequest request,
            string executionId,
            DateTimeOffset startedUtc)
        {
            _repository.Start(new ExecutionHistoryStart
            {
                ExecutionId = executionId,
                CorrelationId = request.CorrelationId,
                CommandName = request.CommandName,
                RequestJson = JsonSerializer.Serialize(new
                {
                    request.CommandName,
                    request.CorrelationId,
                    request.ContextJson,
                    request.Parameters
                }),
                StartedUtc = startedUtc.ToString("O")
            });
        }

        public void Completed(
            CoreCommandExecutionRequest request,
            CoreCommandExecutionResult result,
            string executionId,
            DateTimeOffset completedUtc,
            long durationMilliseconds)
        {
            _repository.Complete(new ExecutionHistoryCompletion
            {
                ExecutionId = executionId,
                Status = result.Status.ToString(),
                CompletedUtc = completedUtc.ToString("O"),
                DurationMilliseconds = durationMilliseconds,
                OutputJson = result.OutputJson,
                Message = result.Message,
                ExceptionText = result.Exception?.ToString()
            });
        }
    }

    private sealed class SuccessHandler : CoreICommandHandler
    {
        public string CommandName => "success.command";

        public Task<CoreCommandExecutionResult> ExecuteAsync(
            CoreCommandExecutionRequest request,
            CoreICommandExecutionContext context)
        {
            return Task.FromResult(CoreCommandExecutionResult.Succeeded(
                request.CommandName,
                request.CorrelationId,
                "Succeeded.",
                "{\"ok\":true}"));
        }
    }

    private sealed class ThrowingHandler : CoreICommandHandler
    {
        public string CommandName => "throw.command";

        public Task<CoreCommandExecutionResult> ExecuteAsync(
            CoreCommandExecutionRequest request,
            CoreICommandExecutionContext context)
        {
            throw new InvalidOperationException("Boom");
        }
    }
}
