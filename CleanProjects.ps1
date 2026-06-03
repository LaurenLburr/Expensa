<#
.SYNOPSIS
Cleans generated build folders for all C# projects under a solution/repository folder.

.DESCRIPTION
Drop CleanProjects.cmd and CleanProjects.ps1 into the solution root, then run CleanProjects.cmd.

Default behavior:
- Searches recursively for .csproj files.
- Deletes each project's bin folder.
- Deletes each project's obj folder.
- Skips .git, .vs, .vscode, .idea, packages, node_modules, Upload, and TestResults.

The script is intentionally conservative: it cleans project build outputs only.
#>

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

Write-Host "Root: $resolvedRoot"
Write-Host ""

function Get-RelativePathSafe {
    param(
        [Parameter(Mandatory = $true)]
        [string]$BasePath,

        [Parameter(Mandatory = $true)]
        [string]$FullPath
    )

    $resolvedBase =
        [System.IO.Path]::GetFullPath($BasePath).TrimEnd('\', '/')

    $resolvedFull =
        [System.IO.Path]::GetFullPath($FullPath)

    if ($resolvedFull.Length -le $resolvedBase.Length) {
        return ""
    }

    return $resolvedFull.Substring($resolvedBase.Length).TrimStart('\', '/')
}

function Test-IsUnderSkippedDirectory {
    param(
        [Parameter(Mandatory = $true)]
        [string]$FullName,

        [Parameter(Mandatory = $true)]
        [string]$BasePath
    )

    $relative =
        Get-RelativePathSafe `
            -BasePath $BasePath `
            -FullPath $FullName

    $parts =
        $relative -split '[\\/]+'

    $skipped =
        @(
            ".git",
            ".github",
            ".vs",
            ".vscode",
            ".idea",
            "packages",
            "node_modules",
            "Upload",
            "TestResults"
        )

    foreach ($part in $parts) {
        if ($skipped -contains $part) {
            return $true
        }
    }

    return $false
}

$projectFiles =
    @(Get-ChildItem `
        -LiteralPath $resolvedRoot `
        -Recurse `
        -Filter "*.csproj" `
        -File |
        Where-Object {
            -not (Test-IsUnderSkippedDirectory `
                -FullName $_.FullName `
                -BasePath $resolvedRoot)
        } |
        Sort-Object FullName)

if ($projectFiles.Count -eq 0) {
    Write-Warning "No .csproj files found under: $resolvedRoot"
    exit 0
}

$deletedCount = 0
$missingCount = 0

foreach ($projectFile in $projectFiles) {
    $projectDirectory =
        $projectFile.Directory.FullName

    Write-Host "Project: $($projectFile.Name)"

    $binFolder =
        Join-Path `
            -Path $projectDirectory `
            -ChildPath "bin"

    $objFolder =
        Join-Path `
            -Path $projectDirectory `
            -ChildPath "obj"

    $foldersToDelete =
        @(
            $binFolder,
            $objFolder
        )

    foreach ($folder in $foldersToDelete) {
        if (Test-Path -LiteralPath $folder) {
            Remove-Item `
                -LiteralPath $folder `
                -Recurse `
                -Force

            $deletedCount++
            Write-Host "  Deleted: $folder"
        }
        else {
            $missingCount++
            Write-Host "  Missing: $folder"
        }
    }

    Write-Host ""
}

Write-Host "Done."
Write-Host "Deleted folders: $deletedCount"
Write-Host "Missing folders: $missingCount"
