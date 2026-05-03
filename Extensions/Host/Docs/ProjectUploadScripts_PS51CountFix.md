# Project Upload Scripts - PowerShell 5.1 Count Fix

## What changed

PowerShell 5.1 may return a single pipeline result as a scalar object instead of an array.
That caused this error:

```text
The property 'Count' cannot be found on this object.
```

This version wraps pipeline results in `@(...)` so `.Count` always exists.

## Run

Double-click:

```text
CreateProjectUploads.bat
```

## Output

```text
UploadPackages\[ProjectName].upload.zip
```
