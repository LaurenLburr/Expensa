# Codex.Data.SQLiteEngine

Reusable C# SQLite engine project for .NET 8.

## Features

- Centralized SQLite execution wrapper
- Events for:
  - CommandExecuting
  - CommandExecuted
  - CommandFailed
  - TransactionBegan
  - TransactionCommitted
  - TransactionRolledBack
  - TransactionFailed
- Query name support for SQL catalog scenarios
- Optional single shared connection mode
- Optional construction from an existing open `SqliteConnection`
- Methods for:
  - ExecuteNonQuery
  - ExecuteScalar
  - Query
  - QueryDataTable
  - BeginTransaction

## Basic usage with existing connection

```csharp
using Codex.Data.SQLiteEngine;
using Microsoft.Data.Sqlite;

using SqliteConnection connection = new("Data Source=mydb.db");
connection.Open();

using SqliteEngine engine = new(connection);

int rows = engine.ExecuteNonQuery(
    "UPDATE [Txn] SET [Status] = @Status WHERE [TransactionId] = @TransactionId;",
    new[]
    {
        new SqliteParameter("@Status", "Cleared"),
        new SqliteParameter("@TransactionId", 42)
    },
    queryName: "Txn.MarkCleared");
```
