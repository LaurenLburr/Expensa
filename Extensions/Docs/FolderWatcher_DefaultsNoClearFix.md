# Folder Watcher Defaults + No-Clear Settings Fix

## Problem

When the app closed, the folder watcher settings file could be overwritten with blank values.

## Fix

The form now protects against blank settings overwrites.

Settings are only saved after loading is complete. If either folder is blank, it is replaced with the default before saving.

## New defaults

Watch Folder:

```text
D:\Git\CodexExpensa\Extensions\AI_Replies
```

Extract To:

```text
D:\Git\CodexExpensa\Extensions
```

## Settings file

```text
%AppData%\Expensa\Extensions\FolderWatcherAutoUnzipSettings.json
```

## Included files

```text
UI/FolderWatcherAutoUnzipForm.cs
UI/FolderWatcherAutoUnzipForm.Designer.cs
UI/FolderWatcherAutoUnzipForm.resx
Services/FolderWatcherAutoUnzipService.cs
```
