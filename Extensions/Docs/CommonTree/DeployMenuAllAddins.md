# Deploy Menu for All Add-ins

The Deploy menu is no longer hard-coded to Websites.

## New behavior

```text
Deploy
    Deploy All Add-ins to Expensa
    ----------------------------
    Deploy BudgetsAddin to Expensa
    Deploy WebsitesAddin to Expensa
```

The menu is built from the Extension Manager registration store.

## Deployment destination

Each add-in deploys to:

```text
Expensa\CodexExpensa.App.WinForms\bin\Debug\net8.0-windows\Modules\<AddinName>
```

## Dependency files

The generic deployment copy keeps files required by dependency-aware add-in loading, including:

```text
*.deps.json
*.runtimeconfig.json
Microsoft.Data.Sqlite.dll
SQLitePCLRaw*.dll
```

Only XML docs and `obj`/`ref` output are skipped.
