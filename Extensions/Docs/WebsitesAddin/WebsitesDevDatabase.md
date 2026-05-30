# Websites Dev Database

This slice adds the first Websites dev database scaffold.

## Folder

```text
Extensions\Modules\WebsitesAddin\DevDatabase
```

## Database

```text
websites.dev.db
```

## Tables

```text
SqlQuery
Website
```

## Rebuild script

```text
Extensions\Modules\WebsitesAddin\Scripts\Rebuild_Websites_DevDatabase.cmd
```

## Query catalog rule

Website queries are stored in `[SqlQuery]`.

Repository code loads SQL by query name instead of embedding tree-loading SQL directly.
