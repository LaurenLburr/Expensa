# CommandEngine Dev Database Object

This package drops the migration track for the development/test bootstrap path.

Use `DevDatabase/CommandEngine.dev.db` as the canonical development database object and copy it to the runtime/test location before the app or tests open the database.

## Included

- `DevDatabase/CommandEngine.dev.db`
- `DevDatabase/CommandEngine.dev.seed.sql`

## Intended runtime behavior

1. Keep this file as the clean source database.
2. On startup/test setup, copy it to the writable runtime database path.
3. Open the copied database.
4. Do not run migrations for this dev path.

Suggested copy rule:

```csharp
if (!File.Exists(runtimeDbPath))
{
    Directory.CreateDirectory(Path.GetDirectoryName(runtimeDbPath)!);
    File.Copy(devDbPath, runtimeDbPath, overwrite: false);
}
```

For repeatable tests, copy with `overwrite: true`.

This database includes:
- uploaded schema tables
- SQL catalog entries
- AI Provider Foundation entries
- ExecutionContext entries
- EngineContext compatibility catalog aliases
- one default OpenAI provider seed
- one default execution context seed
