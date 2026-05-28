# Folder Watcher Processed / Failed Cleanup

## Change

After the watcher processes a zip file, it now moves the original zip out of the watch folder.

## Success

Successful imports move to:

```text
[WatchFolder]\Processed
```

## Failure

Failed imports move to:

```text
[WatchFolder]\Failed
```

## Why

This prevents:
- repeated imports on restart
- old zip files being processed again
- the watch folder filling up with stale uploads

## Duplicate Names

If the cleanup folder already contains a file with the same name, the watcher appends a counter:

```text
Upload.zip
Upload_001.zip
Upload_002.zip
```

## Files

```text
Services/FolderWatcherAutoUnzipService.cs
UI/FolderWatcherAutoUnzipForm.cs
UI/FolderWatcherAutoUnzipForm.Designer.cs
UI/FolderWatcherAutoUnzipForm.resx
```
