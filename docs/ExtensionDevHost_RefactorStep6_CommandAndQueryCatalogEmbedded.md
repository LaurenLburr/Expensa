# Extension Dev Host Refactor Step 6 - Embedded Command and Query Catalog

## Scope

Converts the remaining Tools catalog nodes into embedded main-panel workspaces:

```text
Tools -> Command Catalog
Tools -> Query Catalog
```

## Command Catalog

`CommandCatalogForm` is loaded into the main content panel using:

```csharp
ShowCommandCatalogPanel()
```

It still receives:

```csharp
_commandRegistry
_commandConfigPath
RebuildMenu
```

## Query Catalog

Added `QueryCatalogForm`.

It displays rows from:

```text
%AppData%\Expensa\Extensions\ExtensionMgr.db
```

table:

```text
SqlQuery
```

It can also create the table if missing.

## Files

```text
Extensions/MainForm.cs
UI/CommandCatalogForm.cs
UI/CommandCatalogForm.resx
UI/QueryCatalogForm.cs
```

## Notes

The old commands remain available for menu compatibility. Tree selection now uses embedded panels.
