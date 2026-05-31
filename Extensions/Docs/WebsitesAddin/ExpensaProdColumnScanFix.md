# Expensa Prod Column Scan Fix

The production import no longer uses `DataTable.Load()` against `PRAGMA table_info(...)`.

That path can fail because SQLite may expose unexpected value types in PRAGMA metadata columns.

The importer now uses:

```sql
SELECT group_concat([name], '|')
FROM pragma_table_info('Website');
```

Then it splits the returned column-name list.

This gives the importer only what it needs: the source column names.
