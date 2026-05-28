using Microsoft.Data.Sqlite;
using System.Data;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class SqliteCommandExecutionPersistenceStore :
    ICommandExecutionPersistenceFileStore
{
    private readonly string _connectionString;

    public SqliteCommandExecutionPersistenceStore(
        CommandExecutionPersistenceOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.DatabasePath);

        DatabasePath = options.DatabasePath;

        string? folder =
            Path.GetDirectoryName(DatabasePath);

        if (!string.IsNullOrWhiteSpace(folder))
        {
            Directory.CreateDirectory(folder);
        }

        SqliteConnectionStringBuilder builder = new()
        {
            DataSource = DatabasePath
        };

        _connectionString = builder.ToString();
    }

    public string DatabasePath { get; }

    public void EnsureCreated()
    {
        using SqliteConnection connection =
            OpenConnection();

        ExecuteNonQuery(
            connection,
            CommandExecutionPersistenceSchema.CreateCommandExecutionTable);

        ExecuteNonQuery(
            connection,
            CommandExecutionPersistenceSchema.CreateCommandExecutionIndexes);
    }

    public void UpsertQueueItem(
        CommandExecutionQueueItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        EnsureCreated();

        Upsert(new CommandExecutionPersistentRecord
        {
            ExecutionId = item.QueueItemId.ToString("N"),
            SourceKind = "Queue",
            CommandName = item.CommandName,
            ParameterJson = item.ParameterJson,
            Status = item.Status.ToString(),
            Message = item.Message,
            CorrelationId = item.CorrelationId,
            OutputJson = item.OutputJson,
            CreatedUtc = item.CreatedUtc,
            StartedUtc = item.StartedUtc,
            CompletedUtc = item.CompletedUtc,
            IsHistory = 0,
            IsQueueItem = 1
        });
    }

    public void UpsertHistoryRecord(
        CommandExecutionHistoryRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);

        EnsureCreated();

        Upsert(new CommandExecutionPersistentRecord
        {
            ExecutionId = Guid.NewGuid().ToString("N"),
            SourceKind = "History",
            CommandName = record.CommandName,
            ParameterJson = record.ParameterJson,
            Status = record.Status,
            Message = record.Message,
            CorrelationId = record.CorrelationId,
            OutputJson = record.OutputJson,
            CreatedUtc = record.StartedUtc,
            StartedUtc = record.StartedUtc,
            CompletedUtc = record.CompletedUtc,
            IsHistory = 1,
            IsQueueItem = 0
        });
    }

    public IReadOnlyList<CommandExecutionPersistentRecord> ListQueueItems()
    {
        EnsureCreated();

        using SqliteConnection connection =
            OpenConnection();

        using SqliteCommand command =
            connection.CreateCommand();

        command.CommandText =
            CommandExecutionPersistenceSchema.SelectQueueItems;

        DataTable table =
            ExecuteDataTable(command);

        return MapRecords(table);
    }

    public IReadOnlyList<CommandExecutionPersistentRecord> ListHistoryRecords()
    {
        EnsureCreated();

        using SqliteConnection connection =
            OpenConnection();

        using SqliteCommand command =
            connection.CreateCommand();

        command.CommandText =
            CommandExecutionPersistenceSchema.SelectHistoryRecords;

        DataTable table =
            ExecuteDataTable(command);

        return MapRecords(table);
    }

    public void DeleteCompletedQueueItems()
    {
        EnsureCreated();

        using SqliteConnection connection =
            OpenConnection();

        ExecuteNonQuery(
            connection,
            CommandExecutionPersistenceSchema.DeleteCompletedQueueItems);
    }

    private void Upsert(
        CommandExecutionPersistentRecord record)
    {
        using SqliteConnection connection =
            OpenConnection();

        using SqliteCommand command =
            connection.CreateCommand();

        command.CommandText =
            CommandExecutionPersistenceSchema.UpsertCommandExecution;

        command.Parameters.AddWithValue("@ExecutionId", record.ExecutionId);
        command.Parameters.AddWithValue("@SourceKind", record.SourceKind);
        command.Parameters.AddWithValue("@CommandName", record.CommandName);
        command.Parameters.AddWithValue("@ParameterJson", record.ParameterJson);
        command.Parameters.AddWithValue("@Status", record.Status);
        command.Parameters.AddWithValue("@Message", record.Message);
        command.Parameters.AddWithValue("@CorrelationId", record.CorrelationId);
        command.Parameters.AddWithValue("@OutputJson", record.OutputJson);
        command.Parameters.AddWithValue("@CreatedUtc", record.CreatedUtc.ToString("O"));
        command.Parameters.AddWithValue("@StartedUtc", record.StartedUtc?.ToString("O") ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@CompletedUtc", record.CompletedUtc?.ToString("O") ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@IsHistory", record.IsHistory);
        command.Parameters.AddWithValue("@IsQueueItem", record.IsQueueItem);

        command.ExecuteNonQuery();
    }

    private SqliteConnection OpenConnection()
    {
        SqliteConnection connection =
            new(_connectionString);

        connection.Open();

        return connection;
    }

    private static void ExecuteNonQuery(
        SqliteConnection connection,
        string commandText)
    {
        using SqliteCommand command =
            connection.CreateCommand();

        command.CommandText =
            commandText;

        command.ExecuteNonQuery();
    }

    private static DataTable ExecuteDataTable(
        SqliteCommand command)
    {
        DataTable table =
            new();

        using SqliteDataReader reader =
            command.ExecuteReader();

        table.Load(reader);

        return table;
    }

    private static IReadOnlyList<CommandExecutionPersistentRecord> MapRecords(
        DataTable table)
    {
        List<CommandExecutionPersistentRecord> records = [];

        foreach (DataRow row in table.Rows)
        {
            records.Add(new CommandExecutionPersistentRecord
            {
                ExecutionId = GetString(row, "ExecutionId"),
                SourceKind = GetString(row, "SourceKind"),
                CommandName = GetString(row, "CommandName"),
                ParameterJson = GetString(row, "ParameterJson"),
                Status = GetString(row, "Status"),
                Message = GetString(row, "Message"),
                CorrelationId = GetString(row, "CorrelationId"),
                OutputJson = GetString(row, "OutputJson"),
                CreatedUtc = DateTimeOffset.Parse(GetString(row, "CreatedUtc")),
                StartedUtc = GetNullableDateTimeOffset(row, "StartedUtc"),
                CompletedUtc = GetNullableDateTimeOffset(row, "CompletedUtc"),
                IsHistory = GetInt32(row, "IsHistory"),
                IsQueueItem = GetInt32(row, "IsQueueItem")
            });
        }

        return records;
    }

    private static string GetString(
        DataRow row,
        string columnName)
    {
        object value =
            row[columnName];

        return value == DBNull.Value
            ? string.Empty
            : Convert.ToString(value) ?? string.Empty;
    }

    private static int GetInt32(
        DataRow row,
        string columnName)
    {
        object value =
            row[columnName];

        return value == DBNull.Value
            ? 0
            : Convert.ToInt32(value);
    }

    private static DateTimeOffset? GetNullableDateTimeOffset(
        DataRow row,
        string columnName)
    {
        string value =
            GetString(row, columnName);

        return string.IsNullOrWhiteSpace(value)
            ? null
            : DateTimeOffset.Parse(value);
    }
}
