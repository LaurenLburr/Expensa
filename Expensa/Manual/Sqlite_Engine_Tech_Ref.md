# SQLite Engine Technical Reference

## Purpose

`Codex.Data.SQLiteEngine` is a reusable SQLite execution layer intended to sit below application-specific repositories and above `Microsoft.Data.Sqlite`.

Its job is to provide a stable place for:

- SQL command execution
- query execution
- scalar execution
- DataTable retrieval
- transaction scope handling
- optional named-query resolution through a SQL catalog
- optional command logging
- optional transaction participation for catalog and logging behavior

The engine is meant to stay application-agnostic. It should not know what `Account`, `Bank`, `Payee`, or `Expensa` are.

---

## Project Role in Solution

### Engine project
```text
Codex.Data.SQLiteEngine
```

### Current consumer
```text
CodexExpensa.Data.Sqlite
```

The consumer project uses the engine to back `SqliteDatabase`, which is then used by application repositories.

---

## Design Goals

- Keep SQLite execution code out of app repositories
- Support reuse across multiple projects
- Allow optional SQL catalog lookup by query name
- Allow optional command logging
- Support transaction-aware execution
- Keep engine configuration separate from app schema decisions
- Avoid direct dependency on Expensa-specific types or tables

---

## Core Architectural Rule

The engine may own the **mechanism**, but the application must supply the **schema decisions**.

That means the engine can support:
- SQL catalog lookup
- logging
- transaction handling

But table names, column names, and participation rules must be configured from outside the engine.

---

## Main Types

## `SqliteEngine`

Primary execution class for SQLite operations.

### Responsibilities
- Execute ad hoc non-query SQL
- Execute named non-query SQL
- Run ad hoc queries
- Run named queries
- Return `DataTable` results
- Create transaction scopes
- Coordinate SQL catalog access
- Coordinate command logging
- Respect transaction participation mode

### Constructor shape
Conceptually accepts:
- `SqliteConnection`
- optional ownership flag
- optional `ISqlCatalog`
- optional `ICommandLogger`
- optional `SqliteEngineOptions`

### Public responsibilities
- `ExecuteNonQuery(...)`
- `ExecuteNamedNonQuery(...)`
- `Query<T>(...)`
- `QueryNamed<T>(...)`
- `QueryDataTable(...)`
- `QueryNamedDataTable(...)`
- `ExecuteInTransaction(...)`
- `BeginTransaction()`
- `ResolveSql(...)`

### Notes
- `ResolveSql(...)` must be visible to consumers that need named-query execution inside a transaction
- logging is optional
- SQL catalog access is optional
- engine behavior changes based on configured transaction participation options

---

## `ISqliteTransactionScope`

Represents an active SQLite transaction in engine terms.

### Responsibilities
- Execute non-query commands inside a transaction
- Execute scalar commands inside a transaction
- Query lists inside a transaction
- Query `DataTable` inside a transaction
- commit
- rollback
- expose a transaction identifier

### Current expected members
- `Guid TransactionId`
- `int ExecuteNonQuery(...)`
- `T ExecuteScalar<T>(...)`
- `IReadOnlyList<T> Query<T>(...)`
- `DataTable QueryDataTable(...)`
- `Commit()`
- `Rollback()`

### Notes
This interface is the transaction boundary the app should prefer instead of raw `SqliteTransaction`.

---

## `SqliteTransactionScope`

Concrete implementation of `ISqliteTransactionScope`.

### Responsibilities
- Hold the active `SqliteTransaction`
- Execute commands within that transaction
- log transaction-aware operations
- provide `TransactionId`
- ensure rollback if disposed before completion

### Behavioral expectations
- `Commit()` marks the scope complete
- `Rollback()` marks the scope complete
- `Dispose()` rolls back if commit/rollback never happened
- attempts to use the scope after completion should fail

---

## Configuration Types

These live under:

```text
Codex.Data.SQLiteEngine\Configuration
```

## `SqliteEngineOptions`

Top-level engine options object.

### Intended properties
- `SqlCatalog`
- `Logging`

---

## `SqlCatalogOptions`

Controls SQL catalog behavior.

### Intended properties
- `TableName`
- `NameColumn`
- `SqlColumn`
- `TransactionParticipation`

---

## `LoggingOptions`

Controls logging behavior.

### Intended properties
- `TableName`
- `TimestampColumn`
- `ActionColumn`
- `DetailsColumn`
- `TransactionParticipation`

---

## `TransactionParticipationMode`

Controls whether a subsystem uses the connection directly or participates in the active transaction.

### Values
- `UseConnectionOnly`
- `UseTransactionWhenAvailable`
- `RequireTransaction`

### Meaning

#### `UseConnectionOnly`
Always use the base connection, even if a transaction exists.

Useful when:
- logs should survive rollback
- simple catalog access is acceptable outside transactions

#### `UseTransactionWhenAvailable`
Use the transaction when one exists, otherwise use the base connection.

Useful when:
- named query resolution should stay transaction-aware
- logging can follow transaction behavior when convenient

#### `RequireTransaction`
Fail if no transaction exists.

Useful when:
- strict atomic behavior is required
- catalog or logging must remain transaction-bound

---

## Catalog Abstractions

## `ISqlCatalog`

Resolves SQL text by logical name.

### Intended members
- `string GetSql(string name)`
- `string GetSql(string name, ISqliteTransactionScope tx)`

### Purpose
Allows the engine to execute named queries without hardcoding any table schema.

---

## `TableSqlCatalog`

Table-backed implementation of `ISqlCatalog`.

### Responsibilities
- query configured table for SQL text
- support connection-based access
- support transaction-based access

### Important rule
This class may know table/column names **only through options**.

It must not hardcode Expensa assumptions like:
- always use table `SqlQuery`
- always use column `QueryName`
- always use column `Sql`

Those can be defaults, but they must remain configurable.

---

## Logging Abstractions

## `ICommandLogger`

Writes command execution logs.

### Intended members
- `void Log(string action, string? details)`
- `void Log(string action, string? details, ISqliteTransactionScope tx)`

### Purpose
Allows the engine to emit logs without knowing how logging is persisted.

---

## `TableCommandLogger`

Table-backed implementation of `ICommandLogger`.

### Responsibilities
- insert log rows into configured table
- support connection-based logging
- support transaction-based logging

### Notes
This class is where table-backed logging belongs, not in the core engine itself.

---

## Consumer Layer: `SqliteDatabase`

Project:
```text
CodexExpensa.Data.Sqlite
```

`SqliteDatabase` acts as the application-facing database session that wraps the engine.

### Responsibilities
- own or open the SQLite connection
- optionally create table-backed catalog/logger implementations from engine options
- expose higher-level DB methods to repositories
- provide in-memory seeded-from-file behavior
- save memory DB contents back to file

### Current role
This is the bridge between:
- reusable engine
- Expensa repositories

### Important rule
`SqliteDatabase` may be app-aware.  
`SqliteEngine` must remain app-agnostic.

---

## Current Flow

### Ad hoc non-query
Repository -> `SqliteDatabase.ExecuteNonQuery(sql, params)` -> `SqliteEngine.ExecuteNonQuery(...)`

### Named non-query
Repository -> `SqliteDatabase.ExecuteNamedNonQuery(name, params)` -> engine resolves SQL through `ISqlCatalog`

### Transactional work
Repository -> `SqliteDatabase.ExecuteInTransaction(...)` -> engine creates `ISqliteTransactionScope` -> repository executes through transaction scope

### Transactional named query
Repository -> `SqliteDatabase.ExecuteNamedNonQuery(name, params, tx)` -> engine resolves SQL using configured transaction participation -> transaction scope executes SQL

---

## Reuse Guidance

The engine is reusable **only if these rules remain true**:

- no reference to any `CodexExpensa.*` namespace from engine code
- no hardcoded app table names in core engine classes
- no hardcoded app logging assumptions in core engine classes
- no UI dependency
- no domain-type knowledge

### Safe engine knowledge
- SQLite connection
- SQLite commands
- SQLite transactions
- configurable catalog
- configurable logging
- transaction participation rules

### Unsafe engine knowledge
- `Account`
- `Bank`
- `Payee`
- `Transaction` (domain object)
- app menus
- WinForms
- Expensa table semantics beyond configurable mapping

---

## Known Risks / Cleanup Items

## 1. Duplicate `SqliteEngineOptions`
At one point the solution had both:
- `Codex.Data.SQLiteEngine.SqliteEngineOptions`
- `Codex.Data.SQLiteEngine.Configuration.SqliteEngineOptions`

This causes ambiguity and should be cleaned up so only the configuration version remains.

### Recommendation
Keep:
```text
Codex.Data.SQLiteEngine.Configuration.SqliteEngineOptions
```

Remove or rename the older root-namespace version.

---

## 2. Interface drift risk
Several compile issues came from interface evolution not being mirrored in concrete classes.

Examples:
- `ISqliteTransactionScope` gained members
- `SqliteTransactionScope` had to be updated to match

### Recommendation
Whenever interface members change:
- update implementation immediately
- add/update tests at the same time

---

## 3. Cross-assembly visibility
`ResolveSql(...)` needed to be callable from `CodexExpensa.Data.Sqlite`.

### Recommendation
Keep cross-assembly access intentional and minimal.
If a member must be shared across projects, choose visibility explicitly and document why.

---

## 4. Transaction participation complexity
The engine now supports configurable transaction participation, which is useful but increases behavioral complexity.

### Recommendation
Document expected defaults clearly:
- SQL catalog: `UseTransactionWhenAvailable`
- logging: `UseConnectionOnly`

And add tests for all three participation modes.

---

## Recommended Defaults

### SQL Catalog
Default:
```text
UseTransactionWhenAvailable
```

Reason:
- named query execution should work inside or outside transactions
- transaction-aware lookup is usually the safer behavior

### Logging
Default:
```text
UseConnectionOnly
```

Reason:
- logs survive rollback
- easier to debug
- less surprise during development

---

## Testing Priorities

When continuing work on the engine, the highest-value tests are:

### Transaction scope tests
- commit succeeds
- rollback succeeds
- dispose without commit rolls back
- use after completion throws
- `TransactionId` exists and is stable for scope lifetime

### Catalog tests
- resolves by name from table
- fails when name missing
- respects transaction-aware lookup

### Logger tests
- writes outside transaction
- writes inside transaction
- respects participation mode

### Engine tests
- ad hoc command execution
- named command execution
- scalar execution
- query execution
- DataTable execution
- transaction participation mode behavior

---

## Suggested Future Direction

### Keep the engine boring
Boring code is reusable code.

### Keep app events out of the engine
App-level table events belong in Expensa repositories and UI layers, not in the engine.

### Keep schema configuration outside the engine core
Table names and column names should continue to flow through options and adapter classes.

### Prefer small vertical improvements
When changing the engine:
- update one behavior
- update tests
- stop

Avoid combining:
- interface expansion
- configuration redesign
- logging redesign
- consumer rewiring
all in one pass.

---

## Summary

The SQLite engine is now positioned as:

- a reusable execution backbone
- configurable for SQL catalog and logging
- transaction-aware by option
- safe to consume from Expensa
- reusable in other projects if kept clean

Its long-term quality depends on preserving one rule above all:

**The engine must stay ignorant of the application.**
