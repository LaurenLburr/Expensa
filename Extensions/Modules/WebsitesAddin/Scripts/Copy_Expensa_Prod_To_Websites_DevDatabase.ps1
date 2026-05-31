param(
    [string] $ProdDatabasePath = "",
    [string] $WebsitesDevDatabasePath = ""
)

$ErrorActionPreference = "Stop"

function Resolve-DefaultProdDatabasePath {
    return Join-Path $env:LOCALAPPDATA "CodexExpensa\db\codexexpensa.db"
}

function Resolve-DefaultWebsitesDevDatabasePath {
    $scriptFolder = Split-Path -Parent $PSCommandPath
    $moduleFolder = Split-Path -Parent $scriptFolder
    return Join-Path $moduleFolder "DevDatabase\websites.dev.db"
}

function Require-Sqlite3 {
    $sqlite = Get-Command sqlite3 -ErrorAction SilentlyContinue

    if ($null -eq $sqlite) {
        throw "sqlite3.exe was not found on PATH."
    }

    return $sqlite.Source
}

function Get-FirstExistingSourceTable {
    param(
        [string] $SqliteExe,
        [string] $DatabasePath
    )

    $candidateTables = @(
        "Website",
        "Websites",
        "AccountWebsite",
        "AccountWebsites"
    )

    foreach ($tableName in $candidateTables) {
        $sql = "SELECT name FROM sqlite_master WHERE type = 'table' AND name = '$tableName';"
        $result = & $SqliteExe $DatabasePath $sql

        if ($LASTEXITCODE -ne 0) {
            throw "Failed checking source table '$tableName'."
        }

        if (-not [string]::IsNullOrWhiteSpace($result)) {
            return $tableName
        }
    }

    throw "No supported website source table was found in Expensa production DB. Checked: $($candidateTables -join ', ')"
}

function Invoke-SqliteScript {
    param(
        [string] $SqliteExe,
        [string] $DatabasePath,
        [string] $Sql
    )

    $tempFile = [System.IO.Path]::GetTempFileName()

    try {
        Set-Content -Path $tempFile -Value $Sql -Encoding UTF8
        & $SqliteExe $DatabasePath ".read $tempFile"

        if ($LASTEXITCODE -ne 0) {
            throw "sqlite3 failed while applying SQL script."
        }
    }
    finally {
        Remove-Item -Path $tempFile -Force -ErrorAction SilentlyContinue
    }
}

if ([string]::IsNullOrWhiteSpace($ProdDatabasePath)) {
    $ProdDatabasePath = Resolve-DefaultProdDatabasePath
}

if ([string]::IsNullOrWhiteSpace($WebsitesDevDatabasePath)) {
    $WebsitesDevDatabasePath = Resolve-DefaultWebsitesDevDatabasePath
}

$sqliteExe = Require-Sqlite3

if (-not (Test-Path $ProdDatabasePath)) {
    throw "Expensa production database was not found: $ProdDatabasePath"
}

$devFolder = Split-Path -Parent $WebsitesDevDatabasePath

if (-not (Test-Path $devFolder)) {
    New-Item -ItemType Directory -Path $devFolder | Out-Null
}

$backupPath = ""

if (Test-Path $WebsitesDevDatabasePath) {
    $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
    $backupPath = Join-Path $devFolder "websites.dev.before-prod-copy.$timestamp.db"
    Copy-Item -Path $WebsitesDevDatabasePath -Destination $backupPath -Force
}

$sourceTable = Get-FirstExistingSourceTable -SqliteExe $sqliteExe -DatabasePath $ProdDatabasePath

$prodPathEscaped = $ProdDatabasePath.Replace("'", "''")
$sourceTableEscaped = $sourceTable.Replace("]", "]]")

$sql = @"
PRAGMA foreign_keys = OFF;

CREATE TABLE IF NOT EXISTS [SqlQuery] (
    [QueryName]   TEXT PRIMARY KEY,
    [SqlText]     TEXT NOT NULL,
    [Description] TEXT NOT NULL DEFAULT '',
    [UpdatedUtc]  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS [Website] (
    [WebsiteId]   TEXT PRIMARY KEY,
    [DisplayName] TEXT NOT NULL,
    [Url]         TEXT NOT NULL DEFAULT '',
    [Category]    TEXT NOT NULL DEFAULT '',
    [IsEnabled]   INTEGER NOT NULL DEFAULT 1,
    [SortOrder]   INTEGER NOT NULL DEFAULT 0,
    [CreatedUtc]  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    [UpdatedUtc]  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);

DELETE FROM [Website];

ATTACH DATABASE '$prodPathEscaped' AS [prod];

INSERT OR REPLACE INTO [Website] (
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
    COALESCE(CAST([WebsiteId] AS TEXT), lower(hex(randomblob(16)))) AS [WebsiteId],
    COALESCE(NULLIF(TRIM([DisplayName]), ''), NULLIF(TRIM([Name]), ''), 'Unnamed Website') AS [DisplayName],
    COALESCE([Url], [WebsiteUrl], '') AS [Url],
    COALESCE([Category], [GroupName], '') AS [Category],
    COALESCE([IsEnabled], 1) AS [IsEnabled],
    COALESCE([SortOrder], [SortIndex], 0) AS [SortOrder],
    COALESCE([CreatedUtc], CURRENT_TIMESTAMP) AS [CreatedUtc],
    COALESCE([UpdatedUtc], CURRENT_TIMESTAMP) AS [UpdatedUtc]
FROM [prod].[$sourceTableEscaped];

DETACH DATABASE [prod];

INSERT OR REPLACE INTO [SqlQuery] (
    [QueryName],
    [SqlText],
    [Description],
    [UpdatedUtc]
)
VALUES
(
    'Website.Select.Enabled',
    'SELECT [WebsiteId], [DisplayName], [Url], [Category], [IsEnabled], [SortOrder] FROM [Website] WHERE [IsEnabled] = 1 ORDER BY [SortOrder], [DisplayName];',
    'Select enabled websites ordered for tree loading.',
    CURRENT_TIMESTAMP
),
(
    'Website.Select.All',
    'SELECT [WebsiteId], [DisplayName], [Url], [Category], [IsEnabled], [SortOrder] FROM [Website] ORDER BY [SortOrder], [DisplayName];',
    'Select all websites ordered for tree loading.',
    CURRENT_TIMESTAMP
),
(
    'Website.Select.Search.Enabled',
    'SELECT [WebsiteId], [DisplayName], [Url], [Category], [IsEnabled], [SortOrder] FROM [Website] WHERE [IsEnabled] = 1 AND ([DisplayName] LIKE @SearchText OR [Url] LIKE @SearchText OR [Category] LIKE @SearchText) ORDER BY [SortOrder], [DisplayName];',
    'Search enabled websites ordered for tree loading.',
    CURRENT_TIMESTAMP
),
(
    'Website.Select.Search.All',
    'SELECT [WebsiteId], [DisplayName], [Url], [Category], [IsEnabled], [SortOrder] FROM [Website] WHERE ([DisplayName] LIKE @SearchText OR [Url] LIKE @SearchText OR [Category] LIKE @SearchText) ORDER BY [SortOrder], [DisplayName];',
    'Search all websites ordered for tree loading.',
    CURRENT_TIMESTAMP
);

PRAGMA foreign_keys = ON;
"@

Invoke-SqliteScript -SqliteExe $sqliteExe -DatabasePath $WebsitesDevDatabasePath -Sql $sql

$count = & $sqliteExe $WebsitesDevDatabasePath "SELECT COUNT(*) FROM [Website];"

Write-Host ""
Write-Host "Copied Expensa production website data into WebsitesAddin dev database."
Write-Host "Source DB:  $ProdDatabasePath"
Write-Host "Source table: $sourceTable"
Write-Host "Target DB:  $WebsitesDevDatabasePath"
Write-Host "Website rows: $count"

if (-not [string]::IsNullOrWhiteSpace($backupPath)) {
    Write-Host "Backup:     $backupPath"
}

Write-Host ""
