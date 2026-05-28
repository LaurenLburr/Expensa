<#
.SYNOPSIS
Creates one upload ZIP per C# project in the Codex.CommandEngine solution.

.DESCRIPTION
Place this script in:
D:\Git\CodexExpensa\Codex.CommandEngine

Run:
CreateProjectUploads.bat

Generated files are written to:
D:\Git\CodexExpensa\Codex.CommandEngine\Upload
#>

param(
    [string]$RootPath = (Get-Location).Path
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Write-Header {
    Clear-Host

    Write-Host "============================================================" -ForegroundColor Cyan
    Write-Host "  ____          _           ____                                          " -ForegroundColor Cyan
    Write-Host " / ___|___   __| | _____  __/ ___|___  _ __ ___  _ __ ___   __ _ _ __   " -ForegroundColor Cyan
    Write-Host "| |   / _ \ / _` |/ _ \ \/ / |   / _ \| '_ ` _ \| '_ ` _ \ / _` | '_ \  " -ForegroundColor Cyan
    Write-Host "| |__| (_) | (_| |  __/>  <| |__| (_) | | | | | | | | | | | (_| | | | | " -ForegroundColor Cyan
    Write-Host " \____\___/ \__,_|\___/_/\_\\____\___/|_| |_| |_|_| |_| |_|\__,_|_| |_| " -ForegroundColor Cyan
    Write-Host "                                                                       " -ForegroundColor Cyan
    Write-Host "        Codex.CommandEngine Project Upload Builder" -ForegroundColor Yellow
    Write-Host "============================================================" -ForegroundColor Cyan
    Write-Host ""
}

function Write-Section {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Text
    )

    Write-Host ""
    Write-Host "------------------------------------------------------------" -ForegroundColor DarkCyan
    Write-Host $Text -ForegroundColor Yellow
    Write-Host "------------------------------------------------------------" -ForegroundColor DarkCyan
}

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

    return $excludedExtensions -contains $File.Extension.ToLowerInvariant()
}

function Add-ProjectZip {
    param(
        [Parameter(Mandatory = $true)]
        [System.IO.FileInfo]$ProjectFile,

        [Parameter(Mandatory = $true)]
        [string]$DestinationZip
    )

    $projectDirectory = $ProjectFile.Directory.FullName

    Write-Host ""
    Write-Host "Project:" -NoNewline -ForegroundColor Gray
    Write-Host " $($ProjectFile.BaseName)" -ForegroundColor White

    if (Test-Path -LiteralPath $DestinationZip) {
        Write-Host "Removing old ZIP: $DestinationZip" -ForegroundColor DarkGray
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

        Write-Host "Files included:" -NoNewline -ForegroundColor Gray
        Write-Host " $($files.Count)" -ForegroundColor White

        Write-Host "Created:" -NoNewline -ForegroundColor Green
        Write-Host " $DestinationZip" -ForegroundColor White
    }
    finally {
        if (Test-Path -LiteralPath $tempRoot) {
            Remove-Item -LiteralPath $tempRoot -Recurse -Force
        }
    }
}

Write-Header

$resolvedRoot = (Resolve-Path -LiteralPath $RootPath).Path
$outputFolder = Join-Path $resolvedRoot "Upload"

Write-Section "Paths"
Write-Host "Solution root : $resolvedRoot" -ForegroundColor White
Write-Host "Upload folder : $outputFolder" -ForegroundColor White

New-Item -ItemType Directory -Path $outputFolder -Force | Out-Null

Write-Section "Scanning projects"

$projectFiles = @(Get-ChildItem -LiteralPath $resolvedRoot -Recurse -Filter "*.csproj" -File |
    Where-Object {
        -not (Test-IsUnderExcludedDirectory -FullName $_.FullName -BasePath $resolvedRoot)
    } |
    Sort-Object FullName)

if ($projectFiles.Count -eq 0) {
    Write-Warning "No .csproj files were found under: $resolvedRoot"
    exit 0
}

Write-Host "Projects found: $($projectFiles.Count)" -ForegroundColor White

Write-Section "Creating upload ZIP files"

foreach ($projectFile in $projectFiles) {
    $zipName = "$($projectFile.BaseName).upload.zip"
    $zipPath = Join-Path $outputFolder $zipName

    Add-ProjectZip -ProjectFile $projectFile -DestinationZip $zipPath
}

Write-Section "Complete"
Write-Host "Upload ZIP files created in:" -ForegroundColor Green
Write-Host $outputFolder -ForegroundColor White
Write-Host ""
Write-Host "Done. Tiny ZIP gremlins have been contained." -ForegroundColor Yellow
