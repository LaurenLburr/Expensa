# Website Add-in SQLite Dependency Loading

The runtime error:

```text
Could not load file or assembly 'Microsoft.Data.Sqlite, Version=9.0.0.0'
```

means the add-in assembly was found, but its dependency was not resolved.

## Required fixes

`WebsitesAddin.csproj` must copy package assemblies locally:

```xml
<CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
```

The app and host loaders should use:

```text
AssemblyDependencyResolver
AssemblyLoadContext
```

instead of plain `Assembly.LoadFrom(...)`.

## Deploy check

After building `WebsitesAddin`, confirm the add-in output folder contains:

```text
WebsitesAddin.dll
WebsitesAddin.deps.json
Microsoft.Data.Sqlite.dll
SQLitePCLRaw.*
```
