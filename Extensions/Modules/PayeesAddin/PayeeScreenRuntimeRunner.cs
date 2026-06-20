using Codex.CommandEngine.Core;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PayeesAddin;

public sealed class PayeeScreenRuntimeRunner
{
    private static readonly JsonSerializerOptions JsonOptions =
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition =
                JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };

    public Task<CommandExecutionResult> ExecuteAsync(
        PayeeScreenRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        PayeeScreenDocument document =
            LoadScreen(request);

        return Task.FromResult(
            new CommandExecutionResult
            {
                CorrelationId = Guid.NewGuid().ToString("N"),
                CommandName = "Payees.LoadScreen",
                Status = CommandExecutionStatus.Succeeded,
                Message = $"Loaded screen: {document.Title}",
                OutputJson =
                    JsonSerializer.Serialize(
                        document,
                        JsonOptions)
            });
    }

    private static PayeeScreenDocument LoadScreen(
        PayeeScreenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.EntityId))
        {
            return new PayeeScreenDocument
            {
                Title = string.IsNullOrWhiteSpace(request.DisplayText)
                    ? "Payees"
                    : request.DisplayText,
                Subtitle = "Select a payee to view its transactions.",
                Columns = ["Payee", "IsActive"],
                Rows = LoadPayeeList(request.DatabasePath)
            };
        }

        using SqliteConnection connection =
            OpenReadOnly(request.DatabasePath);

        string payeeName =
            FindPayeeName(
                connection,
                request.EntityId)
            ?? request.DisplayText;

        using SqliteCommand command =
            connection.CreateCommand();

        command.CommandText =
            """
            SELECT
                t.[StartDate],
                t.[Status],
                t.[Amount],
                a.[AccountNickname] AS [Account],
                t.[ConfirmationNumber],
                t.[Note]
            FROM [Txn] t
            LEFT JOIN [Account] a
                ON a.[AccountId] = t.[AccountId]
            WHERE t.[PayeeId] = @PayeeId
            ORDER BY t.[StartDate] DESC, t.[TransactionId] DESC;
            """;

        command.Parameters.AddWithValue(
            "@PayeeId",
            request.EntityId);

        DataTable table =
            LoadTable(command);

        return new PayeeScreenDocument
        {
            Title = payeeName,
            Subtitle =
                $"PayeeId: {request.EntityId}; {table.Rows.Count} transaction(s)",
            Columns =
            [
                "StartDate",
                "Status",
                "Amount",
                "Account",
                "ConfirmationNumber",
                "Note"
            ],
            Rows = ToRows(table)
        };
    }

    private static IReadOnlyList<IReadOnlyDictionary<string, object?>>
        LoadPayeeList(string databasePath)
    {
        using SqliteConnection connection =
            OpenReadOnly(databasePath);

        using SqliteCommand command =
            connection.CreateCommand();

        command.CommandText =
            """
            SELECT
                [PayeeName] AS [Payee],
                [IsActive]
            FROM [Payee]
            ORDER BY [PayeeName];
            """;

        return ToRows(
            LoadTable(command));
    }

    private static string? FindPayeeName(
        SqliteConnection connection,
        string payeeId)
    {
        using SqliteCommand command =
            connection.CreateCommand();

        command.CommandText =
            """
            SELECT [PayeeName]
            FROM [Payee]
            WHERE [PayeeId] = @PayeeId
            LIMIT 1;
            """;

        command.Parameters.AddWithValue(
            "@PayeeId",
            payeeId);

        return Convert.ToString(
            command.ExecuteScalar(),
            CultureInfo.InvariantCulture);
    }

    private static SqliteConnection OpenReadOnly(
        string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                "The Expensa database was not found.",
                path);
        }

        SqliteConnection connection =
            new($"Data Source={path};Mode=ReadOnly;Pooling=False");

        connection.Open();

        return connection;
    }

    private static DataTable LoadTable(
        SqliteCommand command)
    {
        using SqliteDataReader reader =
            command.ExecuteReader();

        DataTable table = new();
        table.Load(reader);

        return table;
    }

    private static IReadOnlyList<IReadOnlyDictionary<string, object?>>
        ToRows(DataTable table)
    {
        List<IReadOnlyDictionary<string, object?>> rows = [];

        foreach (DataRow sourceRow in table.Rows)
        {
            Dictionary<string, object?> row =
                new(StringComparer.OrdinalIgnoreCase);

            foreach (DataColumn column in table.Columns)
            {
                row[column.ColumnName] =
                    sourceRow[column] is DBNull
                        ? null
                        : sourceRow[column];
            }

            rows.Add(row);
        }

        return rows;
    }
}
