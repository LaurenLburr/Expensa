# In-Memory Runtime Database

The Websites add-in now treats file databases as source snapshots.

When a runtime database path is supplied, the add-in:

1. Opens the source file read-only.
2. Copies it into SQLite memory using `BackupDatabase`.
3. Closes the source file.
4. Runs all tree-load queries against the in-memory copy.

This prevents the test tree from holding file locks on:

```text
websites.current.db
```
