# CommandEngine Full Current DB + Code Fix

This package replaces the migration-chasing path with a full-current schema creation path.

## Changed files

- `Codex.CommandEngine.Data/CommandEngineSchema.cs`
- `Codex.CommandEngine.Data/CommandEngineDatabaseInitializer.cs`
- `Codex.CommandEngine.Data/SqlQueryCatalog.cs`
- `Docs/Database/SchemaSnapshots/0005_execution_history_foundation.sql`
- `Docs/Database/SchemaSnapshots/0006_context_system_foundation.sql`
- `Docs/Database/SchemaSnapshots/0007_ai_provider_foundation.sql`

## Why the same tests kept failing

The tests create fresh temporary databases and call `EnsureCreated()`.
They were never using the standalone `.db` file.

So this patch makes `EnsureCreated()` build the full current database schema directly.

## Also fixed

`SqlQueryCatalog.ListActive()` now excludes built-in foundation catalog rows so the catalog unit test that inserts three active user queries still sees exactly three rows.
