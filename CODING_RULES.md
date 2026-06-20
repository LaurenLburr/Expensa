# CODING_RULES.md

## Purpose

These rules define the coding, testing, packaging, and documentation standards for the CodexExpensa / Expensa / Extensions / Codex.CommandEngine projects.

Codex and other AI coding tools must read and follow this file before making changes.

The goal is not just to make code compile. The goal is to preserve project conventions, avoid unnecessary churn, and prevent “helpful” changes that quietly break working behavior.

---

## Core Principles

1. Make the smallest safe change that solves the requested problem.
2. Do not rewrite working code without a specific reason.
3. Do not modernize, refactor, rename, or reorganize unrelated code unless explicitly requested.
4. Prefer boring, understandable code over clever code.
5. Preserve existing architecture and project boundaries.
6. Keep UI, data access, business logic, and tests separated according to the existing project pattern.
7. When in doubt, diagnose first and explain what files need to change before editing.

---

## Technology Defaults

Unless explicitly told otherwise:

* Language: C#
* UI: WinForms
* Database: SQLite
* Target framework: follow the existing project
* ORM: none
* Entity Framework: do not use
* Tests: xUnit unless the project already uses something else
* Data access style: DataTables preferred
* UI layout style: standard WinForms designer pattern

---

## WinForms Rules

Preserve the standard WinForms designer structure.

* UI/control/layout/menu creation belongs in `.Designer.cs` through `InitializeComponent()`.
* Runtime behavior, event handlers, loading logic, validation, and business/UI interaction belong in the main `.cs` code-behind file.
* Resources belong in `.resx`.
* Do not convert designer-built forms into runtime-built layouts unless explicitly requested.
* Do not add controls to derived forms when the intended pattern is to use the base/template form.
* Do not modify manual template files unless explicitly requested.

For Extension Manager / CommandEngineIntegration work:

* Files under `CommandEngineIntegration\Templates` are manual-only.
* Do not modify template files unless the task specifically says to.
* When the instruction says “do not add controls,” it usually means do not add controls to the derived form, not that the template itself should be changed.

---

## Database Rules

SQLite is the default database.

Do not introduce Entity Framework.

Prefer readable, maintainable database behavior over clever abstraction.

### Enum Storage

Store enum values by name/text in the database, not by numeric value.

Reason:

* Names are readable in the database.
* Names are safer across future enum reordering.
* Names are easier for humans and AI tools to inspect later.

Example:

```sql
TransactionStatus = 'Projected'
```

Not:

```sql
TransactionStatus = 1
```

### SQL Catalog Rules

Where the project uses a `SqlQuery` / SQL catalog table, use named SQL queries.

* Do not scatter raw SQL strings throughout repositories if the project pattern expects catalog queries.
* Updates to SQL catalog entries should use `INSERT OR REPLACE` unless the existing migration pattern says otherwise.
* Keep query names stable and descriptive.
* Prefer matching the existing naming convention over inventing a new one.

### Data Access Rules

Avoid direct use of data readers unless explicitly requested.

Preferred patterns:

* `DataTable`
* `DataTable.Load(...)`
* existing `DataTableQueryExecutor`
* existing repository/query helper classes

Do not introduce a new data-access pattern just because it is convenient.

### Migration Rules

Follow the existing migration system.

* Use embedded `.mig` files where the project uses migration files.
* Do not create runtime schema-building code for projects that use template databases or migrations.
* Do not silently alter database schema without saying so.
* Always state whether a change is:

  * code-only
  * database/schema change
  * data/template database change
  * migration change

---

## CodexExpensa Rules

CodexExpensa is a personal checkbook/budget tracker using WinForms and SQLite.

Important existing concepts:

* Banks
* Accounts
* Payees
* Budgets
* Transactions
* Tags
* Websites helper
* Reconcile
* Monthly budget history
* Template budgets

The primary production database is normally located under:

```text
%LOCALAPPDATA%\CodexExpensa\db\codexexpensa.db
```

Do not point test/dev add-ins directly at production data.

---

## Extension Manager / Extensions Rules

The Extensions solution lives under:

```text
D:\Git\CodexExpensa\Extensions
```

Extension Manager must not connect directly to live production data.

Add-ins should use copied/sandbox databases.

### Add-in Database Copy Rules

When copying production data for add-ins:

* The active copied database file must be named:

```text
current.db
```

* Archived/backup copies may use timestamped names.
* The top link in Extension Manager should point to the add-in database, not the production database.
* Update/copy actions should replace the add-in database copy safely.
* After copying, add-in databases may be loaded into memory when that is the intended project pattern.
* File handles must be closed after loading/copying so the database file is not locked.

### Deployment Rules

Deployment changes must be narrowly scoped.

When modifying deploy behavior:

* Do not change unrelated add-in loading behavior.
* Do not change tree-building behavior unless the task asks for it.
* Do not add unrelated nodes to the tree.
* Do not make Extension Manager depend on live Expensa internals beyond the established interfaces.

---

## Codex.CommandEngine Rules

Codex.CommandEngine is a reusable command orchestration and AI workflow platform.

Default rules:

* .NET 8 unless the project has been intentionally upgraded.
* SQLite.
* WinForms host.
* Integration tests preferred.
* Avoid mocks unless there is a specific reason.
* Separate engine database from application databases.
* Use the SQL catalog pattern.
* Use template databases where the project expects them.
* Do not build schemas at runtime when a template DB is the project convention.

Important database convention:

* Development database lives under:

```text
D:\Git\CodexExpensa\Codex.CommandEngine\DevDatabase
```

Tests should copy template/dev databases to temporary runtime locations rather than mutating source/template databases.

---

## Testing Rules

Tests should verify real behavior whenever practical.

Preferred:

* integration tests
* real SQLite databases copied to temp folders
* real repositories
* real query execution
* trace logging for diagnostics

Avoid:

* mocks that do not prove the real behavior
* tests that only verify implementation details
* resurrecting removed features just to satisfy stale tests

### Stale Test Rule

Do not restore removed or obsolete features merely to satisfy old tests.

If a feature was intentionally removed, update or remove the stale test.

If a feature is intentionally reintroduced, add or update tests for the new intended behavior.

### Test Scope Rule

Run the smallest relevant test set first.

Suggested order:

1. Run the failing test or test class.
2. Run the affected project’s tests.
3. Run broader solution tests only when the change crosses project boundaries.

Do not run full-solution tests repeatedly when working on a small isolated fix unless necessary.

---

## File and Zip Delivery Rules

When producing code changes for delivery:

* Provide full replacement files, not snippets.
* Include the full relative file path for each replacement file.
* Package generated changes in a downloadable zip when requested.
* Every generated code/project zip must include a clearly named `README.md` at the archive root.
* The README must not be buried in a subfolder.
* Include tests when appropriate.
* State whether the change is code-only or includes database/schema/data changes.

### Zip Root Rule

Generated project zips should be rooted at:

```text
D:\Git\CodexExpensa
```

Do not root zips at a nested folder such as `Extensions` unless explicitly requested.

---

## Documentation Rules

Documentation should explain why important technical decisions were made, not just what the code does.

Good documentation captures:

* architectural rationale
* database/storage conventions
* workflow rules
* deployment rules
* project boundaries
* assumptions future maintainers should not have to rediscover
* decisions that matter to future AI or human maintainers

Avoid documentation that merely restates obvious code behavior.

---

## Commit Guidance

When a meaningful change is complete, remind the user to commit.

Provide a ready-to-paste body-only commit message.

The commit message should describe:

* what changed
* why it changed
* whether tests were added/updated
* whether there were database/schema/data changes

Do not include fake test results.

If tests were not run, say so.

---

## AI/Codex Workflow Rules

For AI coding tasks:

1. Read this file first.
2. Diagnose before editing when the problem is unclear.
3. Keep scope narrow.
4. Do not inspect or modify unrelated projects unless needed.
5. List files that need changes before broad edits.
6. Make minimal safe changes.
7. Preserve existing conventions.
8. Run relevant tests when possible.
9. Summarize changed files.
10. Clearly state anything not completed.

### Preferred AI Task Format

```text
Task:
[Describe the specific task.]

Scope:
- Only inspect/change these projects:
  - [...]
- Do not modify unrelated projects.
- Do not modify manual templates unless explicitly requested.

Rules:
- Follow CODING_RULES.md.
- Preserve WinForms designer pattern.
- Use DataTables rather than direct DataReaders.
- Use SQL catalog queries where applicable.
- Store enums by name.
- Include/update tests if behavior changes.
- Include README.md if producing a zip.
- State whether this is code-only or includes DB/schema/data changes.

Process:
1. Diagnose first.
2. List files that need changes.
3. Make the smallest safe change.
4. Run only relevant tests first.
5. Provide final changed-files summary.
```

---

## Things Not To Do

Do not:

* introduce Entity Framework
* switch to WPF/MAUI/web UI
* replace WinForms designer structure with runtime layout code
* modify manual templates unless explicitly requested
* point add-ins directly at production databases
* store enums as integers
* use direct data readers when DataTables fit the project pattern
* restore obsolete features just to satisfy stale tests
* rewrite unrelated code
* rename public APIs without a reason
* silently change schema/data
* omit README.md from generated zips
* claim tests passed if they were not run

---

## Final Response Expectations

When returning completed code work, include:

* summary of what changed
* files changed
* tests run
* whether the change is code-only or includes DB/schema/data changes
* any risks or follow-up work
* commit recommendation when appropriate
* ready-to-paste body-only commit message

