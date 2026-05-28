# Database Schema Tracking

Codex.CommandEngine tracks database schema changes at every database-facing step. The goal is to make schema drift obvious before it becomes a swamp monster with indexes.

## Tracking Rules

1. Every schema-changing step gets a versioned SQL snapshot under `Docs\Database\SchemaSnapshots`.
2. The database stores a matching row in `DatabaseSchemaSnapshot`.
3. The database stores applied migration IDs in `MigrationHistory`.
4. New databases are created from the current schema.
5. Existing databases are upgraded by applying missing migrations.
6. Integration tests must verify that the current documented snapshot matches the schema used by code.

## Current Schema Version

Current schema version: `5`

## Snapshot History

| Version | Migration ID | File | Purpose |
| --- | --- | --- | --- |
| 1 | `0001_engine_database_foundation` | `0001_engine_database_foundation.sql` | Initial database foundation: migrations, schema snapshots, SQL catalog table, log table, commands, workflows, execution history, contexts, and AI providers. |
| 2 | `0002_sql_catalog_foundation` | `0002_sql_catalog_foundation.sql` | Adds `SqlQuery.UpdatedUtc` and introduces the SQL catalog service layer for upserting, reading, requiring, and listing cataloged SQL. |
| 3 | `0003_command_registration_foundation` | `0003_command_registration_foundation.sql` | Adds command registration metadata, `CommandDefinition.DisplayName`, `CommandDefinition.HandlerType`, `CommandParameterDefinition`, and command registration SQL catalog entries. |
| 4 | `0004_workflow_model_foundation` | `0004_workflow_model_foundation.sql` | Adds workflow metadata, workflow steps, and workflow SQL catalog entries. |
| 5 | `0005_execution_history_foundation` | `0005_execution_history_foundation.sql` | Adds execution correlation tracking, execution history repository support, indexes, and execution SQL catalog entries. |

## Runtime Tables Used For Tracking

### MigrationHistory

Stores migration IDs after they are applied. This table is intentionally simple so it can bootstrap the rest of the engine.

### DatabaseSchemaSnapshot

Stores the schema snapshot SQL and SHA-256 hash associated with a migration. This lets the engine preserve the exact schema state tied to each database-facing step.

## Current Command Registration Shape

`CommandDefinition` now includes:

- `CommandId`
- `CommandName`
- `DisplayName`
- `Description`
- `Category`
- `Version`
- `HandlerKey`
- `HandlerType`
- `InputJson`
- `OutputJson`
- `MetadataJson`
- `IsEnabled`
- `CreatedUtc`
- `UpdatedUtc`

`CommandParameterDefinition` includes:

- `CommandParameterDefinitionId`
- `CommandId`
- `ParameterName`
- `ParameterType`
- `IsRequired`
- `DefaultValue`
- `Description`
- `SortOrder`
- `CreatedUtc`
- `UpdatedUtc`

## Current SQL Catalog Shape

`SqlQuery` includes:

- `QueryName`
- `Description`
- `SqlText`
- `CreatedUtc`
- `Category`
- `IsActive`
- `UpdatedUtc`

## Command Registration Catalog Entries

The initializer seeds these active SQL catalog entries:

- `CommandDefinition_InsertOrReplace`
- `CommandDefinition_SelectByName`
- `CommandDefinition_SelectAll`
- `CommandParameterDefinition_InsertOrReplace`
- `CommandParameterDefinition_SelectByCommand`

## Test Expectations

The integration tests should verify:

- database creation succeeds
- initialization is repeatable
- foreign keys are enabled
- migration rows are stored
- schema snapshot rows are stored
- current documented schema snapshot matches `CommandEngineSchema.SchemaSql`
- SQL catalog insert, update, inactive query behavior, required query behavior, and active-list ordering work
- command definition persistence works
- command parameter persistence works
- command registration SQL catalog entries are seeded
- workflow definition and workflow step persistence work
- workflow SQL catalog entries are seeded
- execution history and execution step persistence work
- execution history SQL catalog entries are seeded

## 0004 - Workflow Model Foundation

Adds workflow model metadata and workflow SQL catalog support.

Tracked snapshot:

`Docs\Database\SchemaSnapshots\0004_workflow_model_foundation.sql`

Main additions:

- `WorkflowDefinition.DisplayName`
- `WorkflowStep.StepName`
- workflow repository support
- workflow SQL catalog entries


## 0005 - Execution History Foundation

Adds execution history repository support and correlation tracking.

Tracked snapshot:

`Docs\Database\SchemaSnapshots\0005_execution_history_foundation.sql`

Main additions:

- `ExecutionHistory.CorrelationId`
- `IX_ExecutionHistory_Status_StartedUtc`
- `IX_ExecutionHistory_CorrelationId`
- `IX_ExecutionStepHistory_ExecutionId_StepOrder`
- execution history repository support
- execution history SQL catalog entries

Execution history is the audit backbone for command runs, workflow runs, future AI orchestration, retries, diagnostics, and debugging.
