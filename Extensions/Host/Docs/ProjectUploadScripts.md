# Project Upload Scripts

## Files

```text
Host/CreateProjectUploads.ps1
Host/CreateProjectUploads.bat
```

## What it does

Creates one upload ZIP per C# project under the solution/root folder.

Output goes here:

```text
Host/UploadPackages/
```

Example:

```text
Host/UploadPackages/CodexExpensa.ExtensionDevHost.upload.zip
```

## It removes old upload zips first

If the output ZIP already exists, the script deletes it before creating the new one.

## Skipped folders

The script skips:

```text
.git
.vs
.vscode
bin
obj
packages
node_modules
TestResults
UploadPackages
.idea
```

## Skipped file types

The script skips binary/cache/output files such as:

```text
.zip
.dll
.exe
.pdb
.cache
.user
.suo
.db
.sqlite
.log
.tmp
.nupkg
.snupkg
.bin
```

## How to run

Double-click:

```text
CreateProjectUploads.bat
```

or from PowerShell:

```powershell
.\CreateProjectUploads.ps1
```

## Notes

Put these files in the solution/root folder. For the uploaded project, that is:

```text
Host/
```
