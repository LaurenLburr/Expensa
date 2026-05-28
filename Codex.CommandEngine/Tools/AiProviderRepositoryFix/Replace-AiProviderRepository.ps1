$ErrorActionPreference = "Stop"

$solutionRoot = "D:\Git\CodexExpensa\Codex.CommandEngine"
$replacement = Join-Path $solutionRoot "Tools\AiProviderRepositoryFix\AiProviderRepository.cs"

if (-not (Test-Path $replacement)) {
    throw "Replacement file not found: $replacement"
}

$matches = Get-ChildItem -Path $solutionRoot -Recurse -Filter "AiProviderRepository.cs" |
    Where-Object {
        $_.FullName -notlike "*\bin\*" -and
        $_.FullName -notlike "*\obj\*" -and
        $_.FullName -notlike "*\Tools\AiProviderRepositoryFix\*"
    }

if ($matches.Count -eq 0) {
    throw "No AiProviderRepository.cs files found under $solutionRoot"
}

Write-Host "Found AiProviderRepository.cs files:"
$matches | ForEach-Object { Write-Host "  $($_.FullName)" }

foreach ($file in $matches) {
    Copy-Item -Path $file.FullName -Destination "$($file.FullName).bak" -Force
    Copy-Item -Path $replacement -Destination $file.FullName -Force
    Write-Host "Replaced: $($file.FullName)"
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
Write-Host "Done. Rebuild the solution."
