# Extension Dev Host Refactor Step 12 - Folder Watcher / Auto Unzip

## Scope

Adds a workspace tool that watches a folder for zip files and automatically extracts them.

## New Tool Node

```text
Tools
└── Folder Watcher / Auto Unzip
```

## Default Folders

Watch folder:

```text
%AppData%\Expensa\Extensions\UploadWatch
```

Extract folder:

```text
%AppData%\Expensa\Extensions\ImportedUploads
```

## Behavior

The watcher:
- monitors for `*.zip`
- waits until the file copy completes
- extracts the zip into a folder named after the zip file
- deletes/replaces the existing extracted folder if it already exists
- logs imports and failures in the embedded panel

## Added Files

```text
Services/FolderWatcherAutoUnzipService.cs
UI/FolderWatcherAutoUnzipForm.cs
UI/FolderWatcherAutoUnzipForm.Designer.cs
UI/FolderWatcherAutoUnzipForm.resx
```

## Modified

```text
Extensions/MainForm.cs
```
