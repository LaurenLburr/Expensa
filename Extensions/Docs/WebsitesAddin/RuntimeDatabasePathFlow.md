# Runtime Database Path Flow

The Websites add-in now preserves the active runtime database path through the full add-in command path.

## Flow

```text
HostWebsiteRuntimeModuleInvoker
  -> reflected WebsiteLoadRequest.DatabasePath
  -> WebsiteLoadRuntimeSmokeRunner
  -> CommandExecutionRequest.Parameters["databasePath"]
  -> WebsiteLoadRequestParser
  -> WebsiteLoadCommand
  -> SqliteWebsiteRepository
```

This prevents the test tree from falling back to demo/dev data when an active runtime database has been selected.
