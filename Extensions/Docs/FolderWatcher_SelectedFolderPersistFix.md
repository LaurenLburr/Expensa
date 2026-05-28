# Folder Watcher Selected Folder + Persistence Fix

## Changes

### Extract behavior

Zip contents are now extracted directly into the selected **Extract To** folder.

Previous behavior created a child folder named after the zip file. That has been removed.

### Folder persistence

The user-entered folders are now saved to:

```text
%AppData%\Expensa\Extensions\FolderWatcherAutoUnzipSettings.json
```

Persisted fields:

```text
WatchFolder
ExtractFolder
```

Settings are saved when:
- Start Watching is clicked
- Stop is clicked
- Import Existing Zips is clicked
- Browse folder is used
- Open folder is used
- form is disposed

## Files

```text
Services/FolderWatcherAutoUnzipService.cs
UI/FolderWatcherAutoUnzipForm.cs
UI/FolderWatcherAutoUnzipForm.Designer.cs
UI/FolderWatcherAutoUnzipForm.resx
```
