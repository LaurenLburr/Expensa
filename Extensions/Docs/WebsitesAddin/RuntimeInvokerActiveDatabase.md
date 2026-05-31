# Runtime Invoker Active Database

The selected active runtime database is now passed into the Websites module command path.

```text
Make Active
  -> active-runtime-db.txt
  -> HostWebsiteRuntimeModuleInvoker
  -> DatabasePath parameter
  -> WebsiteLoadCommand
  -> SqliteWebsiteRepository
```

This connects both the Database page refresh and the Websites tree load to the selected active DB.
