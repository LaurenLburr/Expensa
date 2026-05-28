$ErrorActionPreference = "Stop"

$designerPath = "D:\Git\CodexExpensa\Extensions\Host\CodexExpensa.ExtensionDevHost\CommandEngineIntegration\ExtensionRuntimeDashboardForm.Designer.cs"

Write-Host ""
Write-Host "========================================================="
Write-Host "  Patch Persisted Record Actions Menu"
Write-Host "========================================================="
Write-Host ""

if (-not (Test-Path -LiteralPath $designerPath)) {
    Write-Host "ERROR: Designer file not found:"
    Write-Host $designerPath
    Read-Host "Press Enter to exit"
    exit 1
}

$text = Get-Content -LiteralPath $designerPath -Raw

if ($text.Contains("replaySelectedPersistedExecutionMenuItem")) {
    Write-Host "Persisted record action menu items already present."
    Read-Host "Press Enter to exit"
    exit 0
}

$text = $text.Replace(
    "private ToolStripMenuItem copyExecutionDatabasePathMenuItem;",
    @"
private ToolStripMenuItem copyExecutionDatabasePathMenuItem;
    private ToolStripMenuItem replaySelectedPersistedExecutionMenuItem;
    private ToolStripMenuItem showSelectedPersistedExecutionDetailsMenuItem;
    private ToolStripMenuItem copySelectedPersistedExecutionParametersMenuItem;
    private ToolStripMenuItem copySelectedPersistedExecutionOutputMenuItem;
"@)

$text = $text.Replace(
    "copyExecutionDatabasePathMenuItem = new ToolStripMenuItem();",
    @"
copyExecutionDatabasePathMenuItem = new ToolStripMenuItem();
        replaySelectedPersistedExecutionMenuItem = new ToolStripMenuItem();
        showSelectedPersistedExecutionDetailsMenuItem = new ToolStripMenuItem();
        copySelectedPersistedExecutionParametersMenuItem = new ToolStripMenuItem();
        copySelectedPersistedExecutionOutputMenuItem = new ToolStripMenuItem();
"@)

$text = $text.Replace(
    @"
            openExecutionDatabaseMenuItem,
            openExecutionDatabaseFolderMenuItem,
            copyExecutionDatabasePathMenuItem
"@,
    @"
            openExecutionDatabaseMenuItem,
            openExecutionDatabaseFolderMenuItem,
            copyExecutionDatabasePathMenuItem,
            new ToolStripSeparator(),
            replaySelectedPersistedExecutionMenuItem,
            showSelectedPersistedExecutionDetailsMenuItem,
            copySelectedPersistedExecutionParametersMenuItem,
            copySelectedPersistedExecutionOutputMenuItem
"@)

$actionBlock = @'
        replaySelectedPersistedExecutionMenuItem.Text = "Replay Selected Persisted &Execution";
        replaySelectedPersistedExecutionMenuItem.Click += ReplaySelectedPersistedExecution;
        showSelectedPersistedExecutionDetailsMenuItem.Text = "Show Selected Persisted Execution &Details";
        showSelectedPersistedExecutionDetailsMenuItem.Click += ShowSelectedPersistedExecutionDetails;
        copySelectedPersistedExecutionParametersMenuItem.Text = "Copy Selected Persisted &Parameters";
        copySelectedPersistedExecutionParametersMenuItem.Click += CopySelectedPersistedExecutionParameters;
        copySelectedPersistedExecutionOutputMenuItem.Text = "Copy Selected Persisted &Output";
        copySelectedPersistedExecutionOutputMenuItem.Click += CopySelectedPersistedExecutionOutput;

'@

$text = $text.Replace(
    '        historyMenuItem.Text = "&History";',
    $actionBlock + '        historyMenuItem.Text = "&History";')

Set-Content -LiteralPath $designerPath -Value $text -Encoding UTF8

Write-Host "Persisted record action menu patch applied."
Write-Host ""
Read-Host "Press Enter to exit"
