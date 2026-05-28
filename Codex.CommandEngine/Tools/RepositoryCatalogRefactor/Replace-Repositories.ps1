$ErrorActionPreference = "Stop"

$solutionRoot = "D:\Git\CodexExpensa\Codex.CommandEngine"
$replacementRoot = Join-Path $solutionRoot "Tools\RepositoryCatalogRefactor"

$files = @(
    "AiProviderRepository.cs",
    "CommandDefinitionRepository.cs",
    "EngineContextRepository.cs",
    "ExecutionHistoryRepository.cs",
    "WorkflowDefinitionRepository.cs"
)

foreach ($fileName in $files) {
    $replacement = Join-Path $replacementRoot $fileName

    if (-not (Test-Path $replacement)) {
        throw "Replacement file not found: $replacement"
    }

    $matches = Get-ChildItem -Path $solutionRoot -Recurse -Filter $fileName |
        Where-Object {
            $_.FullName -notlike "*\bin\*" -and
            $_.FullName -notlike "*\obj\*" -and
            $_.FullName -notlike "*\Tools\RepositoryCatalogRefactor\*"
        }

    if ($matches.Count -eq 0) {
        Write-Host "No existing file found for $fileName"
        continue
    }

    foreach ($match in $matches) {
        Copy-Item -Path $match.FullName -Destination "$($match.FullName).bak" -Force
        Copy-Item -Path $replacement -Destination $match.FullName -Force
        Write-Host "Replaced: $($match.FullName)"
    }
}

Write-Host ""
Write-Host "Cleaning bin/obj folders..."
Get-ChildItem -Path $solutionRoot -Recurse -Directory |
    Where-Object { $_.Name -in @("bin", "obj") } |
    ForEach-Object {
        Write-Host "Deleting: $($_.FullName)"
        Remove-Item -Path $_.FullName -Recurse -Force
    }

Write-Host ""
Write-Host "Done. Apply the SQL catalog patch to the template DB, then rebuild."
