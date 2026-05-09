# Folder Watcher Settings Persistence Fix 2

## Problem

Folder watcher settings were not persisting reliably.

## Fix

`FolderWatcherAutoUnzipForm` now saves folder settings when:

- Watch Folder text changes
- Extract Folder text changes
- either folder textbox loses focus
- Start Watching is clicked
- Stop is clicked
- Import Existing Zips is clicked
- Browse is used
- Open is used
- form closes/disposes

Settings file:

```text
%AppData%\Expensa\Extensions\FolderWatcherAutoUnzipSettings.json
```

## Files

```text
UI/FolderWatcherAutoUnzipForm.cs
UI/FolderWatcherAutoUnzipForm.Designer.cs
UI/FolderWatcherAutoUnzipForm.resx
Services/FolderWatcherAutoUnzipService.cs
```
