using Microsoft.Data.Sqlite;

namespace Codex.CommandEngine.Data;

public sealed class WorkflowDefinitionRepository
{
    private readonly CommandEngineConnectionFactory _connectionFactory;

    public WorkflowDefinitionRepository(CommandEngineConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _connectionFactory = connectionFactory;
    }

    public void Upsert(WorkflowDefinitionUpsert definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentException.ThrowIfNullOrWhiteSpace(definition.WorkflowDefinitionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(definition.WorkflowName);
        ArgumentException.ThrowIfNullOrWhiteSpace(definition.DisplayName);

        if (definition.Version <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(definition), "Workflow version must be greater than zero.");
        }

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = GetRequiredSqlText(RepositorySqlQueryNames.WorkflowDefinition.InsertOrReplace);

        command.Parameters.AddWithValue("$WorkflowId", definition.WorkflowDefinitionId);
        command.Parameters.AddWithValue("$WorkflowDefinitionId", definition.WorkflowDefinitionId);
        command.Parameters.AddWithValue("$WorkflowName", definition.WorkflowName);
        command.Parameters.AddWithValue("$DisplayName", definition.DisplayName);
        command.Parameters.AddWithValue("$Description", definition.Description);
        command.Parameters.AddWithValue("$Version", definition.Version);
        command.Parameters.AddWithValue("$MetadataJson", definition.MetadataJson);
        command.Parameters.AddWithValue("$IsEnabled", definition.IsEnabled ? 1 : 0);

        command.ExecuteNonQuery();
    }

    public void UpsertStep(WorkflowStepDefinitionUpsert step)
    {
        ArgumentNullException.ThrowIfNull(step);
        ArgumentException.ThrowIfNullOrWhiteSpace(step.WorkflowStepDefinitionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(step.WorkflowDefinitionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(step.StepName);
        ArgumentException.ThrowIfNullOrWhiteSpace(step.CommandDefinitionId);

        if (step.StepOrder <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(step), "Workflow step order must be greater than zero.");
        }

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = GetRequiredSqlText(RepositorySqlQueryNames.WorkflowStep.InsertOrReplace);

        command.Parameters.AddWithValue("$WorkflowStepId", step.WorkflowStepDefinitionId);
        command.Parameters.AddWithValue("$WorkflowStepDefinitionId", step.WorkflowStepDefinitionId);
        command.Parameters.AddWithValue("$WorkflowId", step.WorkflowDefinitionId);
        command.Parameters.AddWithValue("$WorkflowDefinitionId", step.WorkflowDefinitionId);
        command.Parameters.AddWithValue("$StepOrder", step.StepOrder);
        command.Parameters.AddWithValue("$StepName", step.StepName);
        command.Parameters.AddWithValue("$CommandId", step.CommandDefinitionId);
        command.Parameters.AddWithValue("$CommandDefinitionId", step.CommandDefinitionId);
        command.Parameters.AddWithValue("$InputMapJson", step.InputMapJson);
        command.Parameters.AddWithValue("$MetadataJson", step.MetadataJson);
        command.Parameters.AddWithValue("$IsEnabled", step.IsEnabled ? 1 : 0);

        command.ExecuteNonQuery();
    }

    public WorkflowDefinitionRecord? FindByName(string workflowName, int version)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowName);

        if (version <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(version), "Workflow version must be greater than zero.");
        }

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = GetRequiredSqlText(RepositorySqlQueryNames.WorkflowDefinition.SelectByNameAndVersion);
        command.Parameters.AddWithValue("$WorkflowName", workflowName);
        command.Parameters.AddWithValue("$Version", version);

        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? ReadWorkflowDefinition(reader) : null;
    }

    public IReadOnlyList<WorkflowDefinitionRecord> ListAll()
    {
        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = GetRequiredSqlText(RepositorySqlQueryNames.WorkflowDefinition.SelectAll);

        List<WorkflowDefinitionRecord> records = [];
        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            records.Add(ReadWorkflowDefinition(reader));
        }

        return records;
    }

    public IReadOnlyList<WorkflowStepDefinitionRecord> ListSteps(string workflowDefinitionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowDefinitionId);

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = GetRequiredSqlText(RepositorySqlQueryNames.WorkflowStep.SelectByWorkflow);
        command.Parameters.AddWithValue("$WorkflowId", workflowDefinitionId);
        command.Parameters.AddWithValue("$WorkflowDefinitionId", workflowDefinitionId);

        List<WorkflowStepDefinitionRecord> records = [];
        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            records.Add(ReadWorkflowStepDefinition(reader));
        }

        return records;
    }

    private string GetRequiredSqlText(string queryName)
    {
        SqlQueryCatalog catalog = new(_connectionFactory);
        return catalog.GetRequiredSqlText(queryName);
    }

    private static WorkflowDefinitionRecord ReadWorkflowDefinition(SqliteDataReader reader)
    {
        return new WorkflowDefinitionRecord(
            reader.GetRequiredString("WorkflowId"),
            reader.GetRequiredString("WorkflowName"),
            reader.GetStringOrDefault("DisplayName"),
            reader.GetStringOrDefault("Description"),
            reader.GetInt32OrDefault("Version", 1),
            reader.GetBooleanOrDefault("IsEnabled"),
            reader.GetStringOrDefault("MetadataJson", "{}"),
            reader.GetStringOrDefault("CreatedUtc"),
            reader.GetNullableString("UpdatedUtc"));
    }

    private static WorkflowStepDefinitionRecord ReadWorkflowStepDefinition(SqliteDataReader reader)
    {
        return new WorkflowStepDefinitionRecord(
            reader.GetRequiredString("WorkflowStepId"),
            reader.GetRequiredString("WorkflowId"),
            reader.GetInt32OrDefault("StepOrder"),
            reader.GetStringOrDefault("StepName"),
            reader.GetRequiredString("CommandId"),
            reader.GetStringOrDefault("InputMapJson", "{}"),
            reader.GetStringOrDefault("MetadataJson", "{}"),
            reader.GetBooleanOrDefault("IsEnabled"));
    }
}

