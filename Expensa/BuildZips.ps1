param(
    [string]$RootPath = $PSScriptRoot
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($RootPath)) {
    $RootPath = $PSScriptRoot
}

$resolvedRoot =
    (Resolve-Path -LiteralPath $RootPath).Path

$uploadFolder =
    Join-Path $resolvedRoot "Upload"

New-Item `
    -ItemType Directory `
    -Path $uploadFolder `
    -Force | Out-Null

Write-Host "Root:   $resolvedRoot"
Write-Host "Upload: $uploadFolder"
Write-Host ""

$projectFiles =
    @(Get-ChildItem `
        -LiteralPath $resolvedRoot `
        -Recurse `
        -Filter "*.csproj" `
        -File |
        Where-Object {
            $_.FullName -notmatch '\\bin\\' -and
            $_.FullName -notmatch '\\obj\\' -and
            $_.FullName -notmatch '\\Upload\\'
        } |
        Sort-Object FullName)

if ($projectFiles.Count -eq 0) {
    Write-Warning "No .csproj files found under: $resolvedRoot"
    exit 0
}

foreach ($projectFile in $projectFiles) {
    $projectDirectory =
        $projectFile.Directory.FullName

    $zipName =
        "$($projectFile.BaseName).upload.zip"

    $zipPath =
        Join-Path $uploadFolder $zipName

    if (Test-Path -LiteralPath $zipPath) {
        Remove-Item `
            -LiteralPath $zipPath `
            -Force
    }

    $stagingRoot =
        Join-Path ([System.IO.Path]::GetTempPath()) ("BuildZips_" + [System.Guid]::NewGuid().ToString("N"))

    $stagingProjectFolder =
        Join-Path $stagingRoot $projectFile.BaseName

    try {
        New-Item `
            -ItemType Directory `
            -Path $stagingProjectFolder `
            -Force | Out-Null

        Get-ChildItem `
            -LiteralPath $projectDirectory `
            -Recurse `
            -File |
            Where-Object {
                $_.FullName -notmatch '\\bin\\' -and
                $_.FullName -notmatch '\\obj\\' -and
                $_.FullName -notmatch '\\Upload\\' -and
                $_.Extension.ToLowerInvariant() -notin @(".zip", ".7z", ".rar", ".dll", ".exe", ".pdb", ".db", ".sqlite", ".sqlite3", ".log", ".nupkg", ".snupkg", ".user", ".suo")
            } |
            ForEach-Object {
                $relativePath =
                    $_.FullName.Substring($projectDirectory.Length).TrimStart('\', '/')

                $targetPath =
                    Join-Path $stagingProjectFolder $relativePath

                $targetDirectory =
                    Split-Path -Parent $targetPath

                if (-not [string]::IsNullOrWhiteSpace($targetDirectory)) {
                    New-Item `
                        -ItemType Directory `
                        -Path $targetDirectory `
                        -Force | Out-Null
                }

                Copy-Item `
                    -LiteralPath $_.FullName `
                    -Destination $targetPath `
                    -Force
            }

        Compress-Archive `
            -Path (Join-Path $stagingProjectFolder "*") `
            -DestinationPath $zipPath `
            -Force

        Write-Host "Created: $zipPath"
    }
    finally {
        if (Test-Path -LiteralPath $stagingRoot) {
            Remove-Item `
                -LiteralPath $stagingRoot `
                -Recurse `
                -Force
        }
    }
}

Write-Host ""
Write-Host "Done."
