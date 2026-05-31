using Microsoft.Data.Sqlite;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class HostWebsiteExpensaProdDatabaseCopyService
{
    private const string CurrentDatabaseFileName = "websites.current.db";

    public HostWebsiteExpensaProdDatabaseCopyResult CopyToDevAndRuntime()
    {
        string prodDatabasePath = GetDefaultExpensaProdDatabasePath();

        if (!File.Exists(prodDatabasePath))
        {
            throw new FileNotFoundException(
                $"Expensa production database was not found: {prodDatabasePath}",
                prodDatabasePath);
        }

        string devDatabasePath = GetWebsitesDevCurrentDatabasePath();
        string runtimeDatabasePath = GetWebsitesRuntimeCurrentDatabasePath();
        string activeRuntimePathFile = GetActiveRuntimePathFile();
        string stagingDatabasePath = CreateStagingDatabasePath();

        Directory.CreateDirectory(Path.GetDirectoryName(devDatabasePath)!);
        Directory.CreateDirectory(Path.GetDirectoryName(runtimeDatabasePath)!);
        Directory.CreateDirectory(Path.GetDirectoryName(activeRuntimePathFile)!);

        CopyWebsiteData(prodDatabasePath, stagingDatabasePath);

        List<string> warnings = [];

        TryReplaceCurrentDatabase(stagingDatabasePath, devDatabasePath, "development", warnings);
        TryReplaceCurrentDatabase(stagingDatabasePath, runtimeDatabasePath, "runtime", warnings);

        File.WriteAllText(activeRuntimePathFile, runtimeDatabasePath);

        return new HostWebsiteExpensaProdDatabaseCopyResult
        {
            ProdDatabasePath = prodDatabasePath,
            StagingDatabasePath = stagingDatabasePath,
            DevDatabasePath = devDatabasePath,
            RuntimeDatabasePath = runtimeDatabasePath,
            ActiveRuntimePathFile = activeRuntimePathFile,
            Warnings = warnings
        };
    }

    private static string CreateStagingDatabasePath()
    {
        string folder =
            Path.Combine(
                Path.GetTempPath(),
                "CodexExpensa",
                "WebsitesAddin",
                "Staging");

        Directory.CreateDirectory(folder);

        return Path.Combine(
            folder,
            $"websites.import.{DateTime.Now:yyyyMMdd_HHmmss_ffff}.db");
    }

    private static void TryReplaceCurrentDatabase(
        string stagingDatabasePath,
        string targetDatabasePath,
        string targetDescription,
        List<string> warnings)
    {
        try
        {
            ReplaceCurrentDatabase(
                stagingDatabasePath,
                targetDatabasePath);
        }
        catch (IOException exception)
        {
            warnings.Add(
                $"Could not replace the {targetDescription} database because it is currently in use: {targetDatabasePath}{Environment.NewLine}{exception.Message}");
        }
        catch (UnauthorizedAccessException exception)
        {
            warnings.Add(
                $"Could not replace the {targetDescription} database because access was denied: {targetDatabasePath}{Environment.NewLine}{exception.Message}");
        }
    }

    private static void ReplaceCurrentDatabase(
        string stagingDatabasePath,
        string targetDatabasePath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(targetDatabasePath)!);

        if (File.Exists(targetDatabasePath))
        {
            string archivePath =
                CreateArchivePath(targetDatabasePath);

            File.Move(
                targetDatabasePath,
                archivePath);
        }

        File.Copy(
            stagingDatabasePath,
            targetDatabasePath,
            overwrite: false);
    }

    private static string CreateArchivePath(
        string databasePath)
    {
        string folder =
            Path.GetDirectoryName(databasePath)!;

        string name =
            Path.GetFileNameWithoutExtension(databasePath);

        return Path.Combine(
            folder,
            $"{name}.before-prod-copy.{DateTime.Now:yyyyMMdd_HHmmss}.db");
    }

    private static string GetDefaultExpensaProdDatabasePath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CodexExpensa",
            "db",
            "codexexpensa.db");
    }

    private static string GetWebsitesDevCurrentDatabasePath()
    {
        return Path.Combine(
            FindExtensionsRoot(),
            "Modules",
            "WebsitesAddin",
            "DevDatabase",
            CurrentDatabaseFileName);
    }

    private static string GetWebsitesRuntimeCurrentDatabasePath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions",
            "Runtime",
            "WebsitesAddin",
            CurrentDatabaseFileName);
    }

    private static string GetActiveRuntimePathFile()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions",
            "Runtime",
            "WebsitesAddin",
            "active-runtime-db.txt");
    }

    private static void CopyWebsiteData(
        string prodDatabasePath,
        string targetDatabasePath)
    {
        File.Delete(targetDatabasePath);

        ValidateRequiredSourceTables(prodDatabasePath);

        using SqliteConnection connection =
            new($"Data Source={targetDatabasePath}");

        connection.Open();

        using SqliteCommand command =
            connection.CreateCommand();

        command.CommandText =
            BuildCopySql(prodDatabasePath);

        command.ExecuteNonQuery();
    }

    private static void ValidateRequiredSourceTables(
        string prodDatabasePath)
    {
        string[] requiredTables =
        [
            "Website",
            "Tag",
            "TagAssignment"
        ];

        using SqliteConnection connection =
            new($"Data Source={prodDatabasePath};Mode=ReadOnly");

        connection.Open();

        foreach (string tableName in requiredTables)
        {
            using SqliteCommand command =
                connection.CreateCommand();

            command.CommandText =
                "SELECT 1 FROM [sqlite_master] WHERE [type] = 'table' AND [name] = @TableName LIMIT 1;";

            command.Parameters.AddWithValue(
                "@TableName",
                tableName);

            object? result =
                command.ExecuteScalar();

            if (result is null || result == DBNull.Value)
            {
                throw new InvalidOperationException(
                    $"Expensa production database is missing required table [{tableName}].");
            }
        }
    }

    private static string BuildCopySql(
        string prodDatabasePath)
    {
        string escapedProdDatabasePath =
            prodDatabasePath.Replace("'", "''", StringComparison.Ordinal);

        return $"""
PRAGMA foreign_keys = OFF;

CREATE TABLE IF NOT EXISTS [SqlQuery] (
    [QueryName] TEXT PRIMARY KEY,
    [SqlText] TEXT NOT NULL,
    [Description] TEXT NOT NULL DEFAULT '',
    [UpdatedUtc] TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS [Website] (
    [WebsiteId] TEXT PRIMARY KEY,
    [Name] TEXT NOT NULL,
    [Url] TEXT NOT NULL DEFAULT '',
    [SortIndex] INTEGER NOT NULL DEFAULT 0,
    [Notes] TEXT NOT NULL DEFAULT '',
    [IsActive] INTEGER NOT NULL DEFAULT 1
);

CREATE TABLE IF NOT EXISTS [Tag] (
    [TagId] TEXT PRIMARY KEY,
    [TagName] TEXT NOT NULL,
    [SortIndex] INTEGER NOT NULL DEFAULT 0,
    [IsActive] INTEGER NOT NULL DEFAULT 1,
    [ParentTagId] TEXT NULL,
    [TagKey] TEXT NULL,
    [TagPath] TEXT NULL,
    [NodeType] TEXT NOT NULL DEFAULT 'Normal'
);

CREATE TABLE IF NOT EXISTS [TagAssignment] (
    [TagAssignmentId] TEXT PRIMARY KEY,
    [TagId] TEXT NOT NULL,
    [EntityType] TEXT NOT NULL,
    [EntityId] TEXT NOT NULL,
    [SortIndex] INTEGER NOT NULL DEFAULT 0,
    [IsActive] INTEGER NOT NULL DEFAULT 1
);

ATTACH DATABASE '{escapedProdDatabasePath}' AS [prod];

INSERT INTO [Website] (
    [WebsiteId],
    [Name],
    [Url],
    [SortIndex],
    [Notes],
    [IsActive]
)
SELECT
    CAST([WebsiteId] AS TEXT),
    COALESCE(NULLIF(TRIM(CAST([Name] AS TEXT)), ''), 'Unnamed Website'),
    COALESCE(CAST([Url] AS TEXT), ''),
    COALESCE([SortIndex], 0),
    COALESCE(CAST([Notes] AS TEXT), ''),
    COALESCE([IsActive], 1)
FROM [prod].[Website];

INSERT INTO [Tag] (
    [TagId],
    [TagName],
    [SortIndex],
    [IsActive],
    [ParentTagId],
    [TagKey],
    [TagPath],
    [NodeType]
)
SELECT
    CAST([TagId] AS TEXT),
    COALESCE(NULLIF(TRIM(CAST([TagName] AS TEXT)), ''), 'Unnamed Tag'),
    COALESCE([SortIndex], 0),
    COALESCE([IsActive], 1),
    CAST([ParentTagId] AS TEXT),
    CAST([TagKey] AS TEXT),
    CAST([TagPath] AS TEXT),
    COALESCE(CAST([NodeType] AS TEXT), 'Normal')
FROM [prod].[Tag];

INSERT INTO [TagAssignment] (
    [TagAssignmentId],
    [TagId],
    [EntityType],
    [EntityId],
    [SortIndex],
    [IsActive]
)
SELECT
    CAST([TagAssignmentId] AS TEXT),
    CAST([TagId] AS TEXT),
    CAST([EntityType] AS TEXT),
    CAST([EntityId] AS TEXT),
    COALESCE([SortIndex], 0),
    COALESCE([IsActive], 1)
FROM [prod].[TagAssignment]
WHERE [EntityType] = 'Website';

DETACH DATABASE [prod];

INSERT OR REPLACE INTO [SqlQuery] ([QueryName], [SqlText], [Description], [UpdatedUtc])
VALUES
(
    'Website.Select.WithTags.Enabled',
    'SELECT w.[WebsiteId], w.[Name], w.[Url], w.[SortIndex], w.[IsActive], t.[TagId], t.[TagName], t.[SortIndex] AS [TagSortIndex], ta.[SortIndex] AS [AssignmentSortIndex] FROM [Website] w LEFT JOIN [TagAssignment] ta ON ta.[EntityType] = ''Website'' AND ta.[EntityId] = w.[WebsiteId] AND ta.[IsActive] = 1 LEFT JOIN [Tag] t ON t.[TagId] = ta.[TagId] AND t.[IsActive] = 1 WHERE w.[IsActive] = 1 ORDER BY COALESCE(t.[SortIndex], 999999), COALESCE(t.[TagName], ''Uncategorized''), ta.[SortIndex], w.[SortIndex], w.[Name];',
    'Select active websites with Expensa tag data for tree loading.',
    CURRENT_TIMESTAMP
),
(
    'Website.Select.WithTags.All',
    'SELECT w.[WebsiteId], w.[Name], w.[Url], w.[SortIndex], w.[IsActive], t.[TagId], t.[TagName], t.[SortIndex] AS [TagSortIndex], ta.[SortIndex] AS [AssignmentSortIndex] FROM [Website] w LEFT JOIN [TagAssignment] ta ON ta.[EntityType] = ''Website'' AND ta.[EntityId] = w.[WebsiteId] AND ta.[IsActive] = 1 LEFT JOIN [Tag] t ON t.[TagId] = ta.[TagId] AND t.[IsActive] = 1 ORDER BY COALESCE(t.[SortIndex], 999999), COALESCE(t.[TagName], ''Uncategorized''), ta.[SortIndex], w.[SortIndex], w.[Name];',
    'Select all websites with Expensa tag data for tree loading.',
    CURRENT_TIMESTAMP
);

PRAGMA foreign_keys = ON;
""";
    }

    private static string FindExtensionsRoot()
    {
        DirectoryInfo? directory =
            new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (string.Equals(directory.Name, "Extensions", StringComparison.OrdinalIgnoreCase))
            {
                return directory.FullName;
            }

            if (Directory.Exists(Path.Combine(directory.FullName, "Modules")) &&
                Directory.Exists(Path.Combine(directory.FullName, "Host")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Extensions");
    }
}
