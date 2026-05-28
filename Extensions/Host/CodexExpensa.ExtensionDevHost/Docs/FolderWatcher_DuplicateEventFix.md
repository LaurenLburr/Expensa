# Folder Watcher Duplicate Event Fix

## Problem

The watcher could log both success and failure for the same zip.

Example:

```text
Imported and moved zip to Processed
FAILED and moved zip to Failed
File was not ready for import
```

## Cause

`FileSystemWatcher` can fire multiple events for the same file. After the first event successfully extracted and moved the zip to `Processed`, a later stale event tried to process the original path again.

## Fix

The watcher now:

- tracks completed source zip paths
- ignores stale duplicate events
- treats missing original zip files as already handled
- no longer imports existing zips automatically on watcher start
- only imports existing zips when `Import Existing Zips` is clicked

## Included

```text
Services/FolderWatcherAutoUnzipService.cs
```
