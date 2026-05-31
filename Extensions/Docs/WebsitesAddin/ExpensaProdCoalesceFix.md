# Expensa Prod COALESCE Fix

The dynamic production import no longer generates invalid single-argument `COALESCE(...)` expressions.

SQLite requires at least two arguments for `COALESCE`.

When no matching production column exists, the importer now emits the fallback expression directly.

Example:

```sql
'Uncategorized'
```

instead of:

```sql
COALESCE('Uncategorized')
```

The stale dynamic-column test was also updated to expect the newer `pragma_table_info` column-scan strategy.
