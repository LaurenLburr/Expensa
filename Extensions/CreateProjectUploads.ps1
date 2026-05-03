<#
.SYNOPSIS
Creates one upload ZIP per C# project in this solution.

.DESCRIPTION
Run this script from the SOLUTION ROOT.

Example:
D:\Git\CodexExpensa\Extensions

Output:
D:\Git\CodexExpensa\Extensions\Upload

Compatible with Windows PowerShell 5.1.
#>

param(
    [string]$RootPath = (Get-Location).Path
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Get-RelativePathSafe {
    param(
        [Parameter(Mandatory = $true)]
        [string]$BasePath,

        [Parameter(Mandatory = $true)]
        [string]$FullPath
    )

    $resolvedBase = [System.IO.Path]::GetFullPath($BasePath).TrimEnd('\', '/')
    $resolvedFull = [System.IO.Path]::GetFullPath($FullPath)

    if ($resolvedFull.Length -le $resolvedBase.Length) {
        return ""
    }

    return $resolvedFull.Substring($resolvedBase.Length).TrimStart('\', '/')
}

function Test-IsUnderExcludedDirectory {
    param(
        [Parameter(Mandatory = $true)]
        [string]$FullName,

        [Parameter(Mandatory = $true)]
        [string]$BasePath
    )

    $relative = Get-RelativePathSafe -BasePath $BasePath -FullPath $FullName
    $parts = $relative -split '[\\/]+'

    $excludedDirectories = @(
        ".git",
        ".vs",
        ".vscode",
        "bin",
        "obj",
        "packages",
        "node_modules",
        "TestResults",
        "Upload",
        ".idea"
    )

    foreach ($part in $parts) {
        if ($excludedDirectories -contains $part) {
            return $true
        }
    }

    return $false
}

function Test-IsExcludedFile {
    param(
        [Parameter(Mandatory = $true)]
        [System.IO.FileInfo]$File
    )

    $excludedExtensions = @(
        ".zip",
        ".7z",
        ".rar",
        ".dll",
        ".exe",
        ".pdb",
        ".cache",
        ".user",
        ".suo",
        ".db",
        ".sqlite",
        ".sqlite3",
        ".log",
        ".tmp",
        ".nupkg",
        ".snupkg",
        ".bin"
    )

    if ($excludedExtensions -contains $File.Extension.ToLowerInvariant()) {
        return $true
    }

    return $false
}

function Add-ProjectZip {
    param(
        [Parameter(Mandatory = $true)]
        [System.IO.FileInfo]$ProjectFile,

        [Parameter(Mandatory = $true)]
        [string]$DestinationZip
    )

    $projectDirectory = $ProjectFile.Directory.FullName

    if (Test-Path -LiteralPath $DestinationZip) {
        Remove-Item -LiteralPath $DestinationZip -Force
    }

    $tempRoot = Join-Path ([System.IO.Path]::GetTempPath()) ("ProjectUpload_" + [System.Guid]::NewGuid().ToString("N"))
    $projectCopyRoot = Join-Path $tempRoot $ProjectFile.BaseName

    try {
        New-Item -ItemType Directory -Path $projectCopyRoot -Force | Out-Null

        $files = @(Get-ChildItem -LiteralPath $projectDirectory -Recurse -File |
            Where-Object {
                -not (Test-IsUnderExcludedDirectory -FullName $_.FullName -BasePath $projectDirectory) -and
                -not (Test-IsExcludedFile -File $_)
            })

        foreach ($file in $files) {
            $relativePath = Get-RelativePathSafe -BasePath $projectDirectory -FullPath $file.FullName
            $targetPath = Join-Path $projectCopyRoot $relativePath
            $targetDirectory = Split-Path -Parent $targetPath

            if (-not [string]::IsNullOrWhiteSpace($targetDirectory)) {
                New-Item -ItemType Directory -Path $targetDirectory -Force | Out-Null
            }

            Copy-Item -LiteralPath $file.FullName -Destination $targetPath -Force
        }

        if ($files.Count -eq 0) {
            Write-Warning "No uploadable files found for project: $($ProjectFile.Name)"
            return
        }

        Compress-Archive -Path (Join-Path $projectCopyRoot "*") -DestinationPath $DestinationZip -Force

        Write-Host "Created: $DestinationZip"
    }
    finally {
        if (Test-Path -LiteralPath $tempRoot) {
            Remove-Item -LiteralPath $tempRoot -Recurse -Force
        }
    }
}

$resolvedRoot = (Resolve-Path -LiteralPath $RootPath).Path
$outputFolder = Join-Path $resolvedRoot "Upload"

New-Item -ItemType Directory -Path $outputFolder -Force | Out-Null

$projectFiles = @(Get-ChildItem -LiteralPath $resolvedRoot -Recurse -Filter "*.csproj" -File |
    Where-Object {
        -not (Test-IsUnderExcludedDirectory -FullName $_.FullName -BasePath $resolvedRoot)
    } |
    Sort-Object FullName)

if ($projectFiles.Count -eq 0) {
    Write-Warning "No .csproj files were found under: $resolvedRoot"
    exit 0
}

foreach ($projectFile in $projectFiles) {
    $zipName = "$($projectFile.BaseName).upload.zip"
    $zipPath = Join-Path $outputFolder $zipName

    Add-ProjectZip -ProjectFile $projectFile -DestinationZip $zipPath
}

Write-Host ""
Write-Host "Done."
Write-Host "Upload ZIP files created in:"
Write-Host $outputFolder
