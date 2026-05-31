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
        string folder = Path.Combine(Path.GetTempPath(), "CodexExpensa", "WebsitesAddin", "Staging");
        Directory.CreateDirectory(folder);

        return Path.Combine(folder, $"websites.import.{DateTime.Now:yyyyMMdd_HHmmss_ffff}.db");
    }

    private static void TryReplaceCurrentDatabase(
        string stagingDatabasePath,
        string targetDatabasePath,
        string targetDescription,
        List<string> warnings)
    {
        try
        {
            ReplaceCurrentDatabase(stagingDatabasePath, targetDatabasePath);
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
            string archivePath = CreateArchivePath(targetDatabasePath);

            File.Move(targetDatabasePath, archivePath);
        }

        File.Copy(stagingDatabasePath, targetDatabasePath, overwrite: false);
    }

    private static string CreateArchivePath(string databasePath)
    {
        string folder = Path.GetDirectoryName(databasePath)!;
        string name = Path.GetFileNameWithoutExtension(databasePath);

        return Path.Combine(folder, $"{name}.before-prod-copy.{DateTime.Now:yyyyMMdd_HHmmss}.db");
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

        using SqliteConnection connection = new($"Data Source={targetDatabasePath}");
        connection.Open();

        string sourceTableName =
            FindSourceWebsiteTable(prodDatabasePath);

        HashSet<string> sourceColumns =
            GetSourceColumns(prodDatabasePath, sourceTableName);

        string copySql =
            BuildCopySql(
                prodDatabasePath,
                sourceTableName,
                sourceColumns);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = copySql;
        command.ExecuteNonQuery();
    }

    private static string FindSourceWebsiteTable(
        string prodDatabasePath)
    {
        string[] candidateTables =
        [
            "Website",
            "Websites",
            "AccountWebsite",
            "AccountWebsites"
        ];

        using SqliteConnection connection = new($"Data Source={prodDatabasePath};Mode=ReadOnly");
        connection.Open();

        foreach (string tableName in candidateTables)
        {
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                "SELECT [name] FROM [sqlite_master] WHERE [type] = 'table' AND [name] = @TableName LIMIT 1;";
            command.Parameters.AddWithValue("@TableName", tableName);

            object? result =
                command.ExecuteScalar();

            if (result is not null && result != DBNull.Value)
            {
                return tableName;
            }
        }

        throw new InvalidOperationException(
            $"No supported website source table was found in Expensa production DB. Checked: {string.Join(", ", candidateTables)}");
    }

    private static HashSet<string> GetSourceColumns(
        string prodDatabasePath,
        string sourceTableName)
    {
        using SqliteConnection connection = new($"Data Source={prodDatabasePath};Mode=ReadOnly");
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();

        string escapedSourceTableName =
            sourceTableName.Replace("'", "''", StringComparison.Ordinal);

        command.CommandText =
            $"SELECT group_concat([name], '|') FROM pragma_table_info('{escapedSourceTableName}');";

        object? result =
            command.ExecuteScalar();

        string columnList =
            Convert.ToString(result) ?? string.Empty;

        return columnList
            .Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private static string BuildCopySql(
        string prodDatabasePath,
        string sourceTableName,
        HashSet<string> sourceColumns)
    {
        string escapedProdDatabasePath =
            prodDatabasePath.Replace("'", "''", StringComparison.Ordinal);

        string sourceTable =
            EscapeIdentifier(sourceTableName);

        string idExpression =
            BuildFirstExistingColumnExpression(
                sourceColumns,
                [
                    "WebsiteId",
                    "WebsiteID",
                    "Id",
                    "ID",
                    "AccountWebsiteId",
                    "AccountWebsiteID"
                ],
                "lower(hex(randomblob(16)))",
                castToText: true);

        string displayNameExpression =
            BuildFirstExistingColumnExpression(
                sourceColumns,
                [
                    "DisplayName",
                    "Name",
                    "WebsiteName",
                    "Description",
                    "Title"
                ],
                "'Unnamed Website'",
                castToText: true,
                trimAndNullIfBlank: true);

        string urlExpression =
            BuildFirstExistingColumnExpression(
                sourceColumns,
                [
                    "Url",
                    "URL",
                    "WebsiteUrl",
                    "WebsiteURL",
                    "Link",
                    "Address"
                ],
                "''",
                castToText: true);

        string categoryExpression =
            BuildFirstExistingColumnExpression(
                sourceColumns,
                [
                    "Category",
                    "GroupName",
                    "WebsiteGroup",
                    "Type",
                    "Section"
                ],
                "'Uncategorized'",
                castToText: true,
                trimAndNullIfBlank: true);

        string isEnabledExpression =
            BuildFirstExistingColumnExpression(
                sourceColumns,
                [
                    "IsEnabled",
                    "Enabled",
                    "IsActive",
                    "Active",
                    "Include"
                ],
                "1",
                castToText: false);

        string sortOrderExpression =
            BuildFirstExistingColumnExpression(
                sourceColumns,
                [
                    "SortOrder",
                    "SortIndex",
                    "DisplayOrder",
                    "Sequence"
                ],
                "0",
                castToText: false);

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
    [DisplayName] TEXT NOT NULL,
    [Url] TEXT NOT NULL DEFAULT '',
    [Category] TEXT NOT NULL DEFAULT '',
    [IsEnabled] INTEGER NOT NULL DEFAULT 1,
    [SortOrder] INTEGER NOT NULL DEFAULT 0,
    [CreatedUtc] TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    [UpdatedUtc] TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);

ATTACH DATABASE '{escapedProdDatabasePath}' AS [prod];

INSERT INTO [Website] (
    [WebsiteId],
    [DisplayName],
    [Url],
    [Category],
    [IsEnabled],
    [SortOrder],
    [CreatedUtc],
    [UpdatedUtc]
)
SELECT
    {idExpression} AS [WebsiteId],
    {displayNameExpression} AS [DisplayName],
    {urlExpression} AS [Url],
    {categoryExpression} AS [Category],
    COALESCE({isEnabledExpression}, 1) AS [IsEnabled],
    COALESCE({sortOrderExpression}, 0) AS [SortOrder],
    CURRENT_TIMESTAMP AS [CreatedUtc],
    CURRENT_TIMESTAMP AS [UpdatedUtc]
FROM [prod].[{sourceTable}];

DETACH DATABASE [prod];

INSERT OR REPLACE INTO [SqlQuery] ([QueryName], [SqlText], [Description], [UpdatedUtc])
VALUES
('Website.Select.Enabled',
 'SELECT [WebsiteId], [DisplayName], [Url], [Category], [IsEnabled], [SortOrder] FROM [Website] WHERE [IsEnabled] = 1 ORDER BY [SortOrder], [DisplayName];',
 'Select enabled websites ordered for tree loading.',
 CURRENT_TIMESTAMP),
('Website.Select.All',
 'SELECT [WebsiteId], [DisplayName], [Url], [Category], [IsEnabled], [SortOrder] FROM [Website] ORDER BY [SortOrder], [DisplayName];',
 'Select all websites ordered for tree loading.',
 CURRENT_TIMESTAMP),
('Website.Select.Search.Enabled',
 'SELECT [WebsiteId], [DisplayName], [Url], [Category], [IsEnabled], [SortOrder] FROM [Website] WHERE [IsEnabled] = 1 AND ([DisplayName] LIKE @SearchText OR [Url] LIKE @SearchText OR [Category] LIKE @SearchText) ORDER BY [SortOrder], [DisplayName];',
 'Search enabled websites ordered for tree loading.',
 CURRENT_TIMESTAMP),
('Website.Select.Search.All',
 'SELECT [WebsiteId], [DisplayName], [Url], [Category], [IsEnabled], [SortOrder] FROM [Website] WHERE ([DisplayName] LIKE @SearchText OR [Url] LIKE @SearchText OR [Category] LIKE @SearchText) ORDER BY [SortOrder], [DisplayName];',
 'Search all websites ordered for tree loading.',
 CURRENT_TIMESTAMP);

PRAGMA foreign_keys = ON;
""";
    }

    private static string BuildFirstExistingColumnExpression(
        HashSet<string> sourceColumns,
        IReadOnlyList<string> candidateColumns,
        string fallbackExpression,
        bool castToText,
        bool trimAndNullIfBlank = false)
    {
        List<string> expressions = [];

        foreach (string candidateColumn in candidateColumns)
        {
            if (!sourceColumns.Contains(candidateColumn))
            {
                continue;
            }

            string columnExpression =
                $"[{EscapeIdentifier(candidateColumn)}]";

            if (castToText)
            {
                columnExpression =
                    $"CAST({columnExpression} AS TEXT)";
            }

            if (trimAndNullIfBlank)
            {
                columnExpression =
                    $"NULLIF(TRIM({columnExpression}), '')";
            }

            expressions.Add(columnExpression);
        }

        if (expressions.Count == 0)
        {
            return fallbackExpression;
        }

        expressions.Add(fallbackExpression);

        if (expressions.Count == 1)
        {
            return expressions[0];
        }

        return $"COALESCE({string.Join(", ", expressions)})";
    }

    private static string EscapeIdentifier(
        string identifier)
    {
        return identifier.Replace("]", "]]", StringComparison.Ordinal);
    }

    private static string FindExtensionsRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

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
