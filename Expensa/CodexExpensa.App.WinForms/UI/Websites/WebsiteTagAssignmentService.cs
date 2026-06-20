using CodexExpensa.App.WinForms.Composition;
using Microsoft.Data.Sqlite;

namespace CodexExpensa.App.WinForms.UI.Websites;

public sealed class WebsiteTagAssignmentService
{
    private const string EntityType = "Website";

    public IReadOnlyList<string> GetActiveTagNames()
    {
        if (!File.Exists(AppPaths.DatabaseFilePath()))
        {
            return [];
        }

        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            SELECT [TagName]
            FROM [Tag]
            WHERE [IsActive] = 1
            ORDER BY [SortIndex], [TagName];
            """;

        List<string> tags = [];

        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            string tagName = reader.GetString(0);

            if (!string.IsNullOrWhiteSpace(tagName))
            {
                tags.Add(tagName);
            }
        }

        return tags;
    }

    public IReadOnlyList<string> GetAssignedTagNames(string websiteId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(websiteId);

        if (!File.Exists(AppPaths.DatabaseFilePath()))
        {
            return [];
        }

        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            SELECT DISTINCT t.[TagName]
            FROM [TagAssignment] ta
            INNER JOIN [Tag] t
                ON t.[TagId] = ta.[TagId]
            WHERE ta.[EntityType] = @EntityType
              AND ta.[EntityId] = @WebsiteId
              AND ta.[IsActive] = 1
              AND t.[IsActive] = 1
            ORDER BY t.[TagName];
            """;

        command.Parameters.AddWithValue("@EntityType", EntityType);
        command.Parameters.AddWithValue("@WebsiteId", websiteId);

        List<string> tags = [];

        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            string tagName = reader.GetString(0);

            if (!string.IsNullOrWhiteSpace(tagName))
            {
                tags.Add(tagName);
            }
        }

        return tags;
    }

    public string AssignTagToWebsite(string websiteId, string tagName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(websiteId);
        ArgumentException.ThrowIfNullOrWhiteSpace(tagName);

        string cleanTagName = tagName.Trim();

        using SqliteConnection connection = OpenConnection();
        using SqliteTransaction transaction = connection.BeginTransaction();

        string tagId = GetOrCreateTagId(connection, transaction, cleanTagName);
        EnsureWebsiteTagAssignment(connection, transaction, websiteId, tagId);

        transaction.Commit();

        return cleanTagName;
    }

    public int RemoveWebsiteTagAssociations(string websiteId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(websiteId);

        using SqliteConnection connection = OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            UPDATE [TagAssignment]
            SET [IsActive] = 0
            WHERE [EntityType] = @EntityType
              AND [EntityId] = @WebsiteId
              AND [IsActive] = 1;
            """;

        command.Parameters.AddWithValue("@EntityType", EntityType);
        command.Parameters.AddWithValue("@WebsiteId", websiteId);

        return command.ExecuteNonQuery();
    }

    private static SqliteConnection OpenConnection()
    {
        SqliteConnection connection = new($"Data Source={AppPaths.DatabaseFilePath()}");
        connection.Open();
        return connection;
    }

    private static string GetOrCreateTagId(SqliteConnection connection, SqliteTransaction transaction, string tagName)
    {
        string? existingTagId = FindTagId(connection, transaction, tagName);

        if (!string.IsNullOrWhiteSpace(existingTagId))
        {
            return existingTagId;
        }

        string tagId = $"tag-{Guid.NewGuid():N}";

        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
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

        command.Parameters.AddWithValue("@TagId", tagId);
        command.Parameters.AddWithValue("@TagName", tagName);
        command.Parameters.AddWithValue("@TagKey", CreateTagKey(tagName));
        command.Parameters.AddWithValue("@TagPath", tagName);
        command.ExecuteNonQuery();

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

    private static void EnsureWebsiteTagAssignment(
        SqliteConnection connection,
        SqliteTransaction transaction,
        string websiteId,
        string tagId)
    {
        using SqliteCommand reactivateCommand = connection.CreateCommand();
        reactivateCommand.Transaction = transaction;
        reactivateCommand.CommandText =
            """
            UPDATE [TagAssignment]
            SET [IsActive] = 1
            WHERE [EntityType] = @EntityType
              AND [EntityId] = @WebsiteId
              AND [TagId] = @TagId;
            """;

        reactivateCommand.Parameters.AddWithValue("@EntityType", EntityType);
        reactivateCommand.Parameters.AddWithValue("@WebsiteId", websiteId);
        reactivateCommand.Parameters.AddWithValue("@TagId", tagId);

        if (reactivateCommand.ExecuteNonQuery() > 0)
        {
            return;
        }

        using SqliteCommand insertCommand = connection.CreateCommand();
        insertCommand.Transaction = transaction;
        insertCommand.CommandText =
            """
            INSERT INTO [TagAssignment] (
                [TagAssignmentId], [TagId], [EntityType], [EntityId], [SortIndex], [IsActive]
            )
            VALUES (
                @TagAssignmentId, @TagId, @EntityType, @WebsiteId, 0, 1
            );
            """;

        insertCommand.Parameters.AddWithValue("@TagAssignmentId", $"tagassignment-{Guid.NewGuid():N}");
        insertCommand.Parameters.AddWithValue("@TagId", tagId);
        insertCommand.Parameters.AddWithValue("@EntityType", EntityType);
        insertCommand.Parameters.AddWithValue("@WebsiteId", websiteId);
        insertCommand.ExecuteNonQuery();
    }

    private static string CreateTagKey(string tagName)
    {
        return tagName.Trim().ToLowerInvariant().Replace(' ', '-');
    }
}
