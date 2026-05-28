using Microsoft.Data.Sqlite;

namespace Codex.CommandEngine.Data;

public sealed class ExecutionHistoryRepository
{
    private readonly CommandEngineConnectionFactory _connectionFactory;

    public ExecutionHistoryRepository(CommandEngineConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _connectionFactory = connectionFactory;
    }

    public void StartExecution(ExecutionHistoryStart execution)
    {
        ArgumentNullException.ThrowIfNull(execution);
        ArgumentException.ThrowIfNullOrWhiteSpace(execution.ExecutionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(execution.ExecutionKind);
        ArgumentException.ThrowIfNullOrWhiteSpace(execution.TargetId);
        ArgumentException.ThrowIfNullOrWhiteSpace(execution.Status);
        ArgumentException.ThrowIfNullOrWhiteSpace(execution.StartedUtc);

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = GetRequiredSqlText(RepositorySqlQueryNames.ExecutionHistory.Insert);

        command.Parameters.AddWithValue("$ExecutionId", execution.ExecutionId);
        command.Parameters.AddWithValue("$ExecutionKind", execution.ExecutionKind);
        command.Parameters.AddWithValue("$TargetId", execution.TargetId);
        command.Parameters.AddWithValue("$TargetName", execution.TargetName);
        command.Parameters.AddWithValue("$Status", execution.Status);
        command.Parameters.AddWithValue("$StartedUtc", execution.StartedUtc);
        command.Parameters.AddWithValue("$CorrelationId", execution.CorrelationId);
        command.Parameters.AddWithValue("$InputJson", execution.InputJson);
        command.Parameters.AddWithValue("$OutputJson", execution.OutputJson);
        command.Parameters.AddWithValue("$ErrorMessage", execution.ErrorMessage);
        command.Parameters.AddWithValue("$MetadataJson", execution.MetadataJson);

        command.ExecuteNonQuery();
    }

    public void CompleteExecution(ExecutionHistoryCompletion completion)
    {
        ArgumentNullException.ThrowIfNull(completion);
        ArgumentException.ThrowIfNullOrWhiteSpace(completion.ExecutionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(completion.Status);
        ArgumentException.ThrowIfNullOrWhiteSpace(completion.CompletedUtc);

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = GetRequiredSqlText(RepositorySqlQueryNames.ExecutionHistory.Complete);

        command.Parameters.AddWithValue("$ExecutionId", completion.ExecutionId);
        command.Parameters.AddWithValue("$Status", completion.Status);
        command.Parameters.AddWithValue("$CompletedUtc", completion.CompletedUtc);
        command.Parameters.AddWithValue("$OutputJson", completion.OutputJson);
        command.Parameters.AddWithValue("$ErrorMessage", completion.ErrorMessage);
        command.Parameters.AddWithValue("$MetadataJson", completion.MetadataJson);

        int affectedRows = command.ExecuteNonQuery();

        if (affectedRows == 0)
        {
            throw new InvalidOperationException($"Execution history record was not found: {completion.ExecutionId}");
        }
    }

    public ExecutionHistoryRecord? FindExecution(string executionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(executionId);

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = GetRequiredSqlText(RepositorySqlQueryNames.ExecutionHistory.SelectById);
        command.Parameters.AddWithValue("$ExecutionId", executionId);

        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? ReadExecution(reader) : null;
    }

    public IReadOnlyList<ExecutionHistoryRecord> ListRecent(int limit)
    {
        if (limit <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(limit), limit, "Limit must be greater than zero.");
        }

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = GetRequiredSqlText(RepositorySqlQueryNames.ExecutionHistory.SelectRecent);
        command.Parameters.AddWithValue("$Limit", limit);

        List<ExecutionHistoryRecord> records = [];
        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            records.Add(ReadExecution(reader));
        }

        return records;
    }

    public void StartStep(ExecutionStepHistoryStart step)
    {
        ArgumentNullException.ThrowIfNull(step);
        ArgumentException.ThrowIfNullOrWhiteSpace(step.ExecutionStepId);
        ArgumentException.ThrowIfNullOrWhiteSpace(step.ExecutionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(step.Status);
        ArgumentException.ThrowIfNullOrWhiteSpace(step.StartedUtc);

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = GetRequiredSqlText(RepositorySqlQueryNames.ExecutionStepHistory.Insert);

        command.Parameters.AddWithValue("$ExecutionStepId", step.ExecutionStepId);
        command.Parameters.AddWithValue("$ExecutionId", step.ExecutionId);
        command.Parameters.AddWithValue("$WorkflowStepId", (object?)step.WorkflowStepDefinitionId ?? DBNull.Value);
        command.Parameters.AddWithValue("$WorkflowStepDefinitionId", (object?)step.WorkflowStepDefinitionId ?? DBNull.Value);
        command.Parameters.AddWithValue("$CommandId", (object?)step.CommandDefinitionId ?? DBNull.Value);
        command.Parameters.AddWithValue("$CommandDefinitionId", (object?)step.CommandDefinitionId ?? DBNull.Value);
        command.Parameters.AddWithValue("$StepOrder", step.StepOrder);
        command.Parameters.AddWithValue("$Status", step.Status);
        command.Parameters.AddWithValue("$StartedUtc", step.StartedUtc);
        command.Parameters.AddWithValue("$InputJson", step.InputJson);
        command.Parameters.AddWithValue("$OutputJson", step.OutputJson);
        command.Parameters.AddWithValue("$ErrorMessage", step.ErrorMessage);
        command.Parameters.AddWithValue("$MetadataJson", step.MetadataJson);

        command.ExecuteNonQuery();
    }

    public void CompleteStep(ExecutionStepHistoryCompletion completion)
    {
        ArgumentNullException.ThrowIfNull(completion);
        ArgumentException.ThrowIfNullOrWhiteSpace(completion.ExecutionStepId);
        ArgumentException.ThrowIfNullOrWhiteSpace(completion.Status);
        ArgumentException.ThrowIfNullOrWhiteSpace(completion.CompletedUtc);

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = GetRequiredSqlText(RepositorySqlQueryNames.ExecutionStepHistory.Complete);

        command.Parameters.AddWithValue("$ExecutionStepId", completion.ExecutionStepId);
        command.Parameters.AddWithValue("$Status", completion.Status);
        command.Parameters.AddWithValue("$CompletedUtc", completion.CompletedUtc);
        command.Parameters.AddWithValue("$OutputJson", completion.OutputJson);
        command.Parameters.AddWithValue("$ErrorMessage", completion.ErrorMessage);
        command.Parameters.AddWithValue("$MetadataJson", completion.MetadataJson);

        int affectedRows = command.ExecuteNonQuery();

        if (affectedRows == 0)
        {
            throw new InvalidOperationException($"Execution step history record was not found: {completion.ExecutionStepId}");
        }
    }

    public IReadOnlyList<ExecutionStepHistoryRecord> ListSteps(string executionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(executionId);

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = GetRequiredSqlText(RepositorySqlQueryNames.ExecutionStepHistory.SelectByExecution);
        command.Parameters.AddWithValue("$ExecutionId", executionId);

        List<ExecutionStepHistoryRecord> records = [];
        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            records.Add(ReadStep(reader));
        }

        return records;
    }

    private string GetRequiredSqlText(string queryName)
    {
        SqlQueryCatalog catalog = new(_connectionFactory);
        return catalog.GetRequiredSqlText(queryName);
    }

    private static ExecutionHistoryRecord ReadExecution(SqliteDataReader reader)
    {
        return new ExecutionHistoryRecord(
            reader.GetRequiredString("ExecutionId"),
            reader.GetRequiredString("ExecutionKind"),
            reader.GetRequiredString("TargetId"),
            reader.GetStringOrDefault("TargetName"),
            reader.GetStringOrDefault("Status"),
            reader.GetStringOrDefault("StartedUtc"),
            reader.GetNullableString("CompletedUtc"),
            reader.GetStringOrDefault("CorrelationId"),
            reader.GetStringOrDefault("InputJson", "{}"),
            reader.GetStringOrDefault("OutputJson", "{}"),
            reader.GetStringOrDefault("ErrorMessage"),
            reader.GetStringOrDefault("MetadataJson", "{}"));
    }

    private static ExecutionStepHistoryRecord ReadStep(SqliteDataReader reader)
    {
        return new ExecutionStepHistoryRecord(
            reader.GetRequiredString("ExecutionStepId"),
            reader.GetRequiredString("ExecutionId"),
            reader.GetNullableString("WorkflowStepId"),
            reader.GetNullableString("CommandId"),
            reader.GetInt32OrDefault("StepOrder"),
            reader.GetStringOrDefault("Status"),
            reader.GetStringOrDefault("StartedUtc"),
            reader.GetNullableString("CompletedUtc"),
            reader.GetStringOrDefault("InputJson", "{}"),
            reader.GetStringOrDefault("OutputJson", "{}"),
            reader.GetStringOrDefault("ErrorMessage"),
            reader.GetStringOrDefault("MetadataJson", "{}"));
    }
}

