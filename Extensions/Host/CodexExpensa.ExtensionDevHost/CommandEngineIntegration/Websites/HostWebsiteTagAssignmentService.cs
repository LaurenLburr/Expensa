using Microsoft.Data.Sqlite;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class HostWebsiteTagAssignmentService
{
    public HostWebsiteTagAssignmentResult AddOrAssignTagToWebsite(string databasePath, string websiteId, string tagName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(websiteId);
        ArgumentException.ThrowIfNullOrWhiteSpace(tagName);

        using SqliteConnection connection =
            SafeSqliteConnection.OpenFromFile(databasePath);

        using SqliteTransaction transaction = connection.BeginTransaction();

        string tagId = GetOrCreateTagId(connection, transaction, tagName.Trim());
        bool assignmentCreated = EnsureWebsiteTagAssignment(connection, transaction, websiteId, tagId);

        transaction.Commit();

        SafeSqliteConnection.SaveToFile(
            connection,
            databasePath);

        return new HostWebsiteTagAssignmentResult
        {
            TagId = tagId,
            TagName = tagName.Trim(),
            WebsiteId = websiteId,
            AssignmentCreated = assignmentCreated
        };
    }

    public IReadOnlyList<string> GetAssignedTagNames(string databasePath, string websiteId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(websiteId);

        if (!File.Exists(databasePath))
        {
            return [];
        }

        using SqliteConnection connection =
            SafeSqliteConnection.OpenFromFile(databasePath);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            SELECT DISTINCT t.[TagName]
            FROM [TagAssignment] ta
            INNER JOIN [Tag] t
                ON t.[TagId] = ta.[TagId]
            WHERE ta.[EntityType] = 'Website'
              AND ta.[EntityId] = @WebsiteId
              AND ta.[IsActive] = 1
              AND t.[IsActive] = 1
            ORDER BY t.[TagName];
            """;

        command.Parameters.AddWithValue("@WebsiteId", websiteId);

        List<string> tags = [];

        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            if (!reader.IsDBNull(0))
            {
                string tagName = reader.GetString(0);

                if (!string.IsNullOrWhiteSpace(tagName))
                {
                    tags.Add(tagName);
                }
            }
        }

        return tags;
    }

    public int RemoveWebsiteTagAssignments(string databasePath, string websiteId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(websiteId);

        using SqliteConnection connection =
            SafeSqliteConnection.OpenFromFile(databasePath);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            UPDATE [TagAssignment]
            SET [IsActive] = 0
            WHERE [EntityType] = 'Website'
              AND [EntityId] = @WebsiteId
              AND [IsActive] = 1;
            """;

        command.Parameters.AddWithValue("@WebsiteId", websiteId);

        int removedCount = command.ExecuteNonQuery();

        SafeSqliteConnection.SaveToFile(
            connection,
            databasePath);

        return removedCount;
    }

    public int DeleteWebsiteNode(string databasePath, string websiteId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(websiteId);

        using SqliteConnection connection =
            SafeSqliteConnection.OpenFromFile(databasePath);

        using SqliteTransaction transaction = connection.BeginTransaction();

        using SqliteCommand websiteCommand = connection.CreateCommand();
        websiteCommand.Transaction = transaction;
        websiteCommand.CommandText =
            """
            UPDATE [Website]
            SET [IsEnabled] = 0
            WHERE [WebsiteId] = @WebsiteId
              AND [IsEnabled] = 1;
            """;

        websiteCommand.Parameters.AddWithValue("@WebsiteId", websiteId);

        int deletedCount = websiteCommand.ExecuteNonQuery();

        using SqliteCommand tagAssignmentCommand = connection.CreateCommand();
        tagAssignmentCommand.Transaction = transaction;
        tagAssignmentCommand.CommandText =
            """
            UPDATE [TagAssignment]
            SET [IsActive] = 0
            WHERE [EntityType] = 'Website'
              AND [EntityId] = @WebsiteId
              AND [IsActive] = 1;
            """;

        tagAssignmentCommand.Parameters.AddWithValue("@WebsiteId", websiteId);
        tagAssignmentCommand.ExecuteNonQuery();

        transaction.Commit();

        SafeSqliteConnection.SaveToFile(
            connection,
            databasePath);

        return deletedCount;
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
            : Convert.ToString(result);
    }

    private static bool EnsureWebsiteTagAssignment(SqliteConnection connection, SqliteTransaction transaction, string websiteId, string tagId)
    {
        using SqliteCommand existsCommand = connection.CreateCommand();
        existsCommand.Transaction = transaction;
        existsCommand.CommandText =
            """
            SELECT 1
            FROM [TagAssignment]
            WHERE [EntityType] = 'Website'
              AND [EntityId] = @WebsiteId
              AND [TagId] = @TagId
              AND [IsActive] = 1
            LIMIT 1;
            """;

        existsCommand.Parameters.AddWithValue("@WebsiteId", websiteId);
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
                @TagAssignmentId, @TagId, 'Website', @WebsiteId, 0, 1
            );
            """;

        insertCommand.Parameters.AddWithValue("@TagAssignmentId", $"tagassignment-{Guid.NewGuid():N}");
        insertCommand.Parameters.AddWithValue("@TagId", tagId);
        insertCommand.Parameters.AddWithValue("@WebsiteId", websiteId);
        insertCommand.ExecuteNonQuery();

        return true;
    }

    private static string CreateTagKey(string tagName)
    {
        return tagName.Trim().ToLowerInvariant().Replace(' ', '-');
    }
}
