using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Globalization;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Payees;

public sealed class HostPayeeTagAssignmentService
{
    public HostPayeeTagAssignmentResult AddOrAssignTagToPayee(string databasePath, string payeeId, string tagName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(payeeId);
        ArgumentException.ThrowIfNullOrWhiteSpace(tagName);

        using SqliteConnection connection =
            SafeSqliteConnection.OpenFromFile(databasePath);

        EnsureRequiredTablesExist(connection);

        using SqliteTransaction transaction = connection.BeginTransaction();

        string trimmedTagName = tagName.Trim();
        string tagId = GetOrCreateTagId(connection, transaction, trimmedTagName);
        bool assignmentCreated = EnsurePayeeTagAssignment(connection, transaction, payeeId, tagId);

        transaction.Commit();

        SafeSqliteConnection.SaveToFile(
            connection,
            databasePath);

        return new HostPayeeTagAssignmentResult
        {
            TagId = tagId,
            TagName = trimmedTagName,
            PayeeId = payeeId,
            AssignmentCreated = assignmentCreated
        };
    }

    public IReadOnlyList<string> GetAssignedTagNames(string databasePath, string payeeId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(payeeId);

        if (!File.Exists(databasePath))
        {
            return [];
        }

        using SqliteConnection connection =
            SafeSqliteConnection.OpenFromFile(databasePath);

        if (!TableExists(connection, "Tag") || !TableExists(connection, "TagAssignment"))
        {
            return [];
        }

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            SELECT DISTINCT t.[TagName]
            FROM [TagAssignment] ta
            INNER JOIN [Tag] t
                ON t.[TagId] = ta.[TagId]
            WHERE ta.[EntityType] = 'Payee'
              AND ta.[EntityId] = @PayeeId
              AND ta.[IsActive] = 1
              AND t.[IsActive] = 1
            ORDER BY t.[TagName];
            """;

        command.Parameters.AddWithValue("@PayeeId", payeeId);

        DataTable table = LoadDataTable(command);

        return table.Rows
            .Cast<DataRow>()
            .Select(static row => Convert.ToString(row["TagName"], CultureInfo.InvariantCulture) ?? string.Empty)
            .Where(static tagName => !string.IsNullOrWhiteSpace(tagName))
            .ToList();
    }

    private static void EnsureRequiredTablesExist(SqliteConnection connection)
    {
        if (!TableExists(connection, "Tag"))
        {
            throw new InvalidOperationException("Tag table was not found in the active Payees add-in database.");
        }

        if (!TableExists(connection, "TagAssignment"))
        {
            throw new InvalidOperationException("TagAssignment table was not found in the active Payees add-in database.");
        }
    }

    private static string GetOrCreateTagId(SqliteConnection connection, SqliteTransaction transaction, string tagName)
    {
        string? existingTagId = FindTagId(connection, transaction, tagName);

        if (!string.IsNullOrWhiteSpace(existingTagId))
        {
            return existingTagId;
        }

        string tagId = $"tag-{Guid.NewGuid():N}";

        using SqliteCommand insertCommand = connection.CreateCommand();
        insertCommand.Transaction = transaction;
        insertCommand.CommandText =
            """
            INSERT INTO [Tag] (
                [TagId], [TagName], [SortIndex], [IsActive],
                [ParentTagId], [TagKey], [TagPath], [NodeType]
            )
            VALUES (
                @TagId, @TagName, 0, 1,
                NULL, @TagKey, @TagPath, 'Normal'
            );
            """;

        insertCommand.Parameters.AddWithValue("@TagId", tagId);
        insertCommand.Parameters.AddWithValue("@TagName", tagName);
        insertCommand.Parameters.AddWithValue("@TagKey", CreateTagKey(tagName));
        insertCommand.Parameters.AddWithValue("@TagPath", tagName);
        insertCommand.ExecuteNonQuery();

        return tagId;
    }

    private static string? FindTagId(SqliteConnection connection, SqliteTransaction transaction, string tagName)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
            """
            SELECT [TagId]
            FROM [Tag]
            WHERE [IsActive] = 1
              AND lower([TagName]) = lower(@TagName)
            LIMIT 1;
            """;

        command.Parameters.AddWithValue("@TagName", tagName);

        object? result = command.ExecuteScalar();

        return result is null || result == DBNull.Value
            ? null
            : Convert.ToString(result, CultureInfo.InvariantCulture);
    }

    private static bool EnsurePayeeTagAssignment(SqliteConnection connection, SqliteTransaction transaction, string payeeId, string tagId)
    {
        using SqliteCommand existsCommand = connection.CreateCommand();
        existsCommand.Transaction = transaction;
        existsCommand.CommandText =
            """
            SELECT 1
            FROM [TagAssignment]
            WHERE [EntityType] = 'Payee'
              AND [EntityId] = @PayeeId
              AND [TagId] = @TagId
              AND [IsActive] = 1
            LIMIT 1;
            """;

        existsCommand.Parameters.AddWithValue("@PayeeId", payeeId);
        existsCommand.Parameters.AddWithValue("@TagId", tagId);

        object? existingAssignment = existsCommand.ExecuteScalar();

        if (existingAssignment is not null && existingAssignment != DBNull.Value)
        {
            return false;
        }

        using SqliteCommand insertCommand = connection.CreateCommand();
        insertCommand.Transaction = transaction;
        insertCommand.CommandText =
            """
            INSERT INTO [TagAssignment] (
                [TagAssignmentId], [TagId], [EntityType], [EntityId], [SortIndex], [IsActive]
            )
            VALUES (
                @TagAssignmentId, @TagId, 'Payee', @PayeeId, 0, 1
            );
            """;

        insertCommand.Parameters.AddWithValue("@TagAssignmentId", $"tagassignment-{Guid.NewGuid():N}");
        insertCommand.Parameters.AddWithValue("@TagId", tagId);
        insertCommand.Parameters.AddWithValue("@PayeeId", payeeId);
        insertCommand.ExecuteNonQuery();

        return true;
    }

    private static bool TableExists(SqliteConnection connection, string tableName)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT 1
            FROM [sqlite_master]
            WHERE [type] = 'table'
              AND [name] = @TableName
            LIMIT 1;
            """;
        command.Parameters.AddWithValue("@TableName", tableName);

        object? result = command.ExecuteScalar();
        return result is not null && result is not DBNull;
    }

    private static DataTable LoadDataTable(SqliteCommand command)
    {
        using SqliteDataReader reader = command.ExecuteReader();
        DataTable table = new();
        table.Load(reader);
        return table;
    }

    private static string CreateTagKey(string tagName)
    {
        return tagName.Trim().ToLowerInvariant().Replace(' ', '-');
    }
}
