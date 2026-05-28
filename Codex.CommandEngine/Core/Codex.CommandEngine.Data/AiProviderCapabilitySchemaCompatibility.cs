namespace Codex.CommandEngine.Data;

public static class AiProviderCapabilitySchemaCompatibility
{
    public static void Ensure(DataTableQueryExecutor executor)
    {
        ArgumentNullException.ThrowIfNull(executor);

        executor.ExecuteNonQuery(
            """
            CREATE TABLE IF NOT EXISTS AiProviderCapability (
                AiProviderCapabilityId TEXT PRIMARY KEY,
                AiProviderId           TEXT NOT NULL DEFAULT '',
                CapabilityName         TEXT NOT NULL DEFAULT '',
                CapabilityKind         TEXT NOT NULL DEFAULT '',
                CapabilityValue        TEXT NOT NULL DEFAULT '',
                MetadataJson           TEXT NOT NULL DEFAULT '{}',
                UpdatedUtc             TEXT NULL
            );
            """,
            new Dictionary<string, object?>());

        AddColumnIfMissing(executor, "AiProviderCapability", "CapabilityName", "TEXT NOT NULL DEFAULT ''");
        AddColumnIfMissing(executor, "AiProviderCapability", "CapabilityKind", "TEXT NOT NULL DEFAULT ''");
        AddColumnIfMissing(executor, "AiProviderCapability", "CapabilityValue", "TEXT NOT NULL DEFAULT ''");
        AddColumnIfMissing(executor, "AiProviderCapability", "MetadataJson", "TEXT NOT NULL DEFAULT '{}'");
        AddColumnIfMissing(executor, "AiProviderCapability", "UpdatedUtc", "TEXT NULL");

        executor.ExecuteNonQuery(
            """
            UPDATE AiProviderCapability
            SET CapabilityName = CapabilityKind
            WHERE (CapabilityName IS NULL OR trim(CapabilityName) = '')
              AND CapabilityKind IS NOT NULL
              AND trim(CapabilityKind) <> '';

            UPDATE AiProviderCapability
            SET CapabilityKind = CapabilityName
            WHERE (CapabilityKind IS NULL OR trim(CapabilityKind) = '')
              AND CapabilityName IS NOT NULL
              AND trim(CapabilityName) <> '';

            UPDATE AiProviderCapability
            SET CapabilityValue = CapabilityName
            WHERE (CapabilityValue IS NULL OR trim(CapabilityValue) = '')
              AND CapabilityName IS NOT NULL
              AND trim(CapabilityName) <> '';
            """,
            new Dictionary<string, object?>());
    }

    private static void AddColumnIfMissing(
        DataTableQueryExecutor executor,
        string tableName,
        string columnName,
        string columnDefinition)
    {
        object? result =
            executor.ExecuteScalar(
                $"SELECT COUNT(*) FROM pragma_table_info('{tableName}') WHERE name = $ColumnName;",
                new Dictionary<string, object?>
                {
                    ["ColumnName"] = columnName
                });

        if (Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture) > 0)
        {
            return;
        }

        executor.ExecuteNonQuery(
            $"ALTER TABLE {tableName} ADD COLUMN {columnName} {columnDefinition};",
            new Dictionary<string, object?>());
    }
}
