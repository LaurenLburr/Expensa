# Project Upload Scripts - PowerShell 5.1 Fix

## What changed

The first version used:

```powershell
[System.IO.Path]::GetRelativePath(...)
```

That works in newer .NET, but not in Windows PowerShell 5.1.

This version uses a compatible helper:

```powershell
Get-RelativePathSafe
```

## Run

Double-click:

```text
CreateProjectUploads.bat
```

or run:

```powershell
.\CreateProjectUploads.ps1
```

## Output

```text
UploadPackages\[ProjectName].upload.zip
```

## Skips

```text
bin
obj
.vs
.git
.vscode
node_modules
TestResults
UploadPackages
packages
```
