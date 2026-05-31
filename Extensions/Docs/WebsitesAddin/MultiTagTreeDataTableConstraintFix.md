# Multi-Tag Tree DataTable Constraint Fix

The Websites tree query now returns one row per website/tag assignment.

That means the same `WebsiteId` can appear more than once, which is correct.

`DataTable.Load(reader)` can infer constraints from the reader schema and fail when duplicate website IDs appear.

The repository now manually loads the reader into a constraint-free `DataTable`:

```text
SqliteDataReader
  -> DataTable.Columns.Add(...)
  -> DataRow values
  -> tree grouping by TagName
```

This preserves DataTable-based handling without letting `DataTable.Load()` invent constraints we do not want.
