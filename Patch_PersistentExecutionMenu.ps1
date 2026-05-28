$ErrorActionPreference = "Stop"

$designerPath = "D:\Git\CodexExpensa\Extensions\Host\CodexExpensa.ExtensionDevHost\CommandEngineIntegration\ExtensionRuntimeDashboardForm.Designer.cs"

Write-Host ""
Write-Host "========================================================="
Write-Host "  Patch Persistent Execution Menu"
Write-Host "========================================================="
Write-Host ""

if (-not (Test-Path -LiteralPath $designerPath)) {
    Write-Host "ERROR: Designer file not found:"
    Write-Host $designerPath
    Read-Host "Press Enter to exit"
    exit 1
}

$text = Get-Content -LiteralPath $designerPath -Raw

if ($text.Contains("persistenceMenuItem")) {
    Write-Host "Persistence menu already present."
    Read-Host "Press Enter to exit"
    exit 0
}

$text = $text.Replace(
    "private ToolStripMenuItem historyMenuItem;",
    @"
private ToolStripMenuItem persistenceMenuItem;
    private ToolStripMenuItem showPersistedExecutionCountsMenuItem;
    private ToolStripMenuItem showPersistedQueueRecordsMenuItem;
    private ToolStripMenuItem showPersistedHistoryRecordsMenuItem;
    private ToolStripMenuItem openExecutionDatabaseMenuItem;
    private ToolStripMenuItem openExecutionDatabaseFolderMenuItem;
    private ToolStripMenuItem copyExecutionDatabasePathMenuItem;
    private ToolStripMenuItem historyMenuItem;
"@)

$text = $text.Replace(
    "historyMenuItem = new ToolStripMenuItem();",
    @"
persistenceMenuItem = new ToolStripMenuItem();
        showPersistedExecutionCountsMenuItem = new ToolStripMenuItem();
        showPersistedQueueRecordsMenuItem = new ToolStripMenuItem();
        showPersistedHistoryRecordsMenuItem = new ToolStripMenuItem();
        openExecutionDatabaseMenuItem = new ToolStripMenuItem();
        openExecutionDatabaseFolderMenuItem = new ToolStripMenuItem();
        copyExecutionDatabasePathMenuItem = new ToolStripMenuItem();
        historyMenuItem = new ToolStripMenuItem();
"@)

$text = $text.Replace(
    @"
queueMenuItem,
            historyMenuItem
"@,
    @"
queueMenuItem,
            persistenceMenuItem,
            historyMenuItem
"@)

$menuBlock = @'
        persistenceMenuItem.Text = "&Persistence";
        persistenceMenuItem.DropDownItems.AddRange(new ToolStripItem[]
        {
            showPersistedExecutionCountsMenuItem,
            showPersistedQueueRecordsMenuItem,
            showPersistedHistoryRecordsMenuItem,
            new ToolStripSeparator(),
            openExecutionDatabaseMenuItem,
            openExecutionDatabaseFolderMenuItem,
            copyExecutionDatabasePathMenuItem
        });
        showPersistedExecutionCountsMenuItem.Text = "Show Persisted Execution &Counts";
        showPersistedExecutionCountsMenuItem.Click += ShowPersistedExecutionCounts;
        showPersistedQueueRecordsMenuItem.Text = "Show Persisted &Queue Records";
        showPersistedQueueRecordsMenuItem.Click += ShowPersistedQueueRecords;
        showPersistedHistoryRecordsMenuItem.Text = "Show Persisted &History Records";
        showPersistedHistoryRecordsMenuItem.Click += ShowPersistedHistoryRecords;
        openExecutionDatabaseMenuItem.Text = "Open Execution &Database";
        openExecutionDatabaseMenuItem.Click += OpenExecutionDatabase;
        openExecutionDatabaseFolderMenuItem.Text = "Open Execution Database &Folder";
        openExecutionDatabaseFolderMenuItem.Click += OpenExecutionDatabaseFolder;
        copyExecutionDatabasePathMenuItem.Text = "Copy Execution Database &Path";
        copyExecutionDatabasePathMenuItem.Click += CopyExecutionDatabasePath;

'@

$text = $text.Replace(
    '        historyMenuItem.Text = "&History";',
    $menuBlock + '        historyMenuItem.Text = "&History";')

Set-Content -LiteralPath $designerPath -Value $text -Encoding UTF8

Write-Host "Persistence menu patch applied."
Write-Host ""
Read-Host "Press Enter to exit"
