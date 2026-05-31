# Copy Expensa Production Data to Websites Dev Database

This script copies website data from the Expensa production database into the WebsitesAddin dev database.

## Script

```text
Extensions\Modules\WebsitesAddin\Scripts\Copy_Expensa_Prod_To_Websites_DevDatabase.cmd
```

## Default source

```text
%LOCALAPPDATA%\CodexExpensa\db\codexexpensa.db
```

## Default target

```text
Extensions\Modules\WebsitesAddin\DevDatabase\websites.dev.db
```

## Behavior

- Locates a supported website table in Expensa production.
- Backs up the existing `websites.dev.db` first.
- Clears and reloads the dev `[Website]` table.
- Re-seeds the WebsitesAddin `[SqlQuery]` catalog entries.

## Supported source table names

```text
Website
Websites
AccountWebsite
AccountWebsites
```

If production uses a different table name or different column names, update the script mapping section.
