# Codex.CommandEngine - Execution History Foundation

## Purpose

This step adds the first usable execution tracking layer for the command engine.

Execution history is the audit trail for commands, workflows, AI requests, retries, failures, and future scheduling. Without it, the engine can do work, but it cannot explain what happened afterward. That is how debugging turns into archaeology.

## Added Capabilities

- Record execution start records.
- Complete execution records with final status, output, and errors.
- Record workflow or command step history.
- List recent executions for the host UI.
- List steps for a selected execution.
- Track a correlation id for future workflow, AI, and distributed execution tracing.
- Seed execution history SQL catalog entries.
- Track schema version 0005.

## New Data Classes

- `ExecutionHistoryRecord`
- `ExecutionHistoryStart`
- `ExecutionHistoryCompletion`
- `ExecutionStepHistoryRecord`
- `ExecutionStepHistoryStart`
- `ExecutionStepHistoryCompletion`

## New Repository

- `ExecutionHistoryRepository`

The repository supports:

- `StartExecution`
- `CompleteExecution`
- `FindExecution`
- `ListRecent`
- `StartStep`
- `CompleteStep`
- `ListSteps`

## Schema Changes

Schema version is now `5`.

Migration id:

```text
0005_execution_history_foundation
```

The `ExecutionHistory` table now includes:

```sql
CorrelationId TEXT NOT NULL DEFAULT ''
```

New indexes:

```sql
CREATE INDEX IF NOT EXISTS IX_ExecutionHistory_Status_StartedUtc ON ExecutionHistory (Status, StartedUtc);
CREATE INDEX IF NOT EXISTS IX_ExecutionHistory_CorrelationId ON ExecutionHistory (CorrelationId);
CREATE INDEX IF NOT EXISTS IX_ExecutionStepHistory_ExecutionId_StepOrder ON ExecutionStepHistory (ExecutionId, StepOrder);
```

## SQL Catalog Entries

Added category:

```text
Execution History
```

New query entries:

- `ExecutionHistory_Insert`
- `ExecutionHistory_Complete`
- `ExecutionHistory_SelectRecent`
- `ExecutionStepHistory_Insert`
- `ExecutionStepHistory_SelectByExecution`

## Host UI Additions

The host navigation tree now includes:

```text
Execution History
  Execution Runs
  Execution Steps
```

`Execution Runs` lists recent execution records.

`Execution Steps` prints recorded steps grouped by execution.

## Tests Added

Integration tests verify:

- start and find execution history records
- complete execution history records
- insert and list execution steps in order
- execution history SQL catalog seeding
- schema snapshot tracking for version 0005

## Notes

This is intentionally persistence-focused. The actual command/workflow runner will use this repository in the next orchestration phase.
