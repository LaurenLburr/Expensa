# Folder Watcher Strip Matching Root Folder Fix

## Problem

Manual unzip worked because dragging the top-level `Extensions` folder from the zip merged its contents into:

```text
D:\Git\CodexExpensa\Extensions
```

The watcher extracted literally, creating:

```text
D:\Git\CodexExpensa\Extensions\Extensions\...
```

## Fix

The watcher now detects when the zip has one top-level folder and that folder name matches the selected extraction folder name.

Example:

```text
Zip root: Extensions
Extract To: D:\Git\CodexExpensa\Extensions
```

In that case, the watcher strips the top-level `Extensions` folder before extracting.

## Result

Zip entry:

```text
Extensions\Host\CodexExpensa.ExtensionDevHost\UI\File.cs
```

extracts to:

```text
D:\Git\CodexExpensa\Extensions\Host\CodexExpensa.ExtensionDevHost\UI\File.cs
```

not:

```text
D:\Git\CodexExpensa\Extensions\Extensions\Host\...
```

## Files

```text
Services/FolderWatcherAutoUnzipService.cs
UI/FolderWatcherAutoUnzipForm.cs
UI/FolderWatcherAutoUnzipForm.Designer.cs
UI/FolderWatcherAutoUnzipForm.resx
```
