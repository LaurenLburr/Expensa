# Codex.CommandEngine Planned Workflow

## Purpose

Codex.CommandEngine is a reusable command orchestration and AI workflow platform intended to be shared across multiple future projects.

The engine should provide a durable foundation for defining commands, composing workflows, executing those workflows, integrating AI providers, storing execution context, and reviewing execution history.

This document defines the planned development workflow for the project. It follows the current priority order and keeps the early work focused on stable foundations instead of building UI features too soon. Tempting, yes. Wise, no.

## Project Rules

- Target .NET 8.
- Use WinForms for the host application.
- Use SQLite for engine storage.
- Keep the command engine reusable across future projects.
- Use a separate SQLite database for the command engine.
- Prefer guard clauses for validation and invalid state checks.
- Avoid mocks.
- Prefer integration-style tests that exercise real behavior.
- Add tests as features are introduced.
- Use full files for code changes.
- Use the SQL catalog for reusable named SQL where appropriate.
- Keep core logic independent of WinForms and other UI concerns.
- Keep host/UI code thin.
- Design commands, workflows, AI providers, contexts, and execution history as first-class concepts.

## Development Priority Order

The project will be built in this order:

1. Engine database foundation
2. SQL catalog system
3. Command registration model
4. Workflow model
5. Execution history
6. Context system
7. AI provider abstraction
8. Workflow runner/orchestrator
9. Host UI tooling

Each phase should leave the solution buildable, testable, and usable before moving to the next phase.

## Phase 1 - Engine Database Foundation

### Goal

Create a reliable SQLite database foundation for the command engine.

The database layer should be reusable, testable, and independent of the WinForms host.

### Planned Work

- Define the engine database location strategy.
- Ensure the engine database can be created on first run.
- Add schema initialization logic.
- Add a schema version or migration tracking table.
- Add baseline tables needed by later phases.
- Add integration tests using a real SQLite database.
- Keep file-backed and in-memory database usage clear and predictable.

### Early Tables

Likely early tables:

- `SchemaVersion` or `MigrationHistory`
- `SqlQuery`
- `Log`
- `CommandDefinition`
- `WorkflowDefinition`
- `WorkflowStep`
- `ExecutionHistory`
- `ExecutionStepHistory`
- `ExecutionContext`
- `AiProvider`

Some tables may start minimal and expand later. The important part is establishing the structure without pretending we already know every future column. That road leads to schema lasagna.

### Exit Criteria

- Database can be created from scratch.
- Schema initialization is repeatable.
- Integration tests prove initialization works.
- Database path behavior is covered by tests.
- No UI dependency exists in the database layer.

## Phase 2 - SQL Catalog System

### Goal

Create a SQL catalog system for reusable named SQL statements.

The catalog should prevent SQL from being scattered through the codebase like confetti after a bad office party.

### Planned Work

- Define the `SqlQuery` table.
- Add idempotent catalog seeding.
- Add lookup by query name.
- Add tests for insert/update behavior.
- Add tests for missing query behavior.
- Decide which queries belong in the catalog and which should remain local implementation details.

### Exit Criteria

- Named SQL can be inserted or replaced safely.
- Code can retrieve SQL by name.
- Tests cover catalog creation, lookup, and update.
- Initial engine queries are cataloged where appropriate.

## Phase 3 - Command Registration Model

### Goal

Define how commands are described, registered, discovered, and executed.

A command should have a stable identity, display information, optional metadata, expected inputs, and a handler path that can execute it.

### Planned Work

- Define command domain models.
- Define command registration interfaces.
- Store command definitions in SQLite.
- Separate persisted command definitions from runtime command handlers.
- Add validation for duplicate command IDs.
- Add tests for registration and retrieval.
- Keep command execution separate from registration.

### Possible Concepts

- Command ID
- Command name
- Description
- Category
- Version
- Enabled/disabled state
- Input contract metadata
- Output contract metadata
- Handler key or handler type name

### Exit Criteria

- Commands can be registered.
- Commands can be listed.
- Duplicate command IDs are rejected or handled predictably.
- Runtime handlers are not tightly coupled to persistence.
- Tests cover registration behavior.

## Phase 4 - Workflow Model

### Goal

Define workflows as ordered sets of command steps.

Workflows should be persistent, versionable, and executable later by the orchestrator.

### Planned Work

- Define workflow domain models.
- Define workflow step models.
- Store workflow definitions and steps in SQLite.
- Support step ordering.
- Support enabled/disabled workflows.
- Add basic workflow validation.
- Add tests for workflow persistence and retrieval.

### Recommended Design Choices

- Add workflow versioning early.
- Keep workflow metadata flexible.
- Store step input mappings as JSON where appropriate.
- Avoid making the first workflow model too clever.

### Exit Criteria

- Workflows can be created and retrieved.
- Workflow steps preserve order.
- Invalid workflows fail validation.
- Tests cover workflow persistence.

## Phase 5 - Execution History

### Goal

Track command and workflow executions in a durable, reviewable history.

Execution history should be append-focused. Changing history after the fact is how bugs put on fake mustaches and sneak back in.

### Planned Work

- Define execution history tables.
- Track execution start/end times.
- Track status, duration, error details, and result metadata.
- Track per-step execution details for workflows.
- Store relevant inputs and outputs safely.
- Add tests for success and failure history records.

### Possible Execution Status Values

- `Pending`
- `Running`
- `Succeeded`
- `Failed`
- `Canceled`
- `Skipped`

### Exit Criteria

- Command executions are recorded.
- Workflow step executions are recorded.
- Failures preserve useful error details.
- Tests prove history is written correctly.

## Phase 6 - Context System

### Goal

Create a context system that allows workflows and commands to share structured data.

Context should support both execution-time state and reusable named contexts.

### Planned Work

- Define context domain models.
- Store context values in SQLite.
- Support JSON payloads for flexible structured data.
- Support context scopes.
- Add tests for context creation and retrieval.
- Add validation for missing required context values.

### Possible Context Scopes

- Global
- Project
- Workflow
- Execution
- Step

### Exit Criteria

- Context values can be stored and retrieved.
- JSON payloads are preserved correctly.
- Required context validation exists.
- Tests cover common context behavior.

## Phase 7 - AI Provider Abstraction

### Goal

Add an abstraction for AI providers without binding the engine to one vendor or one API shape.

The engine should know how to request AI work. Provider implementations should know how to talk to specific external services.

### Planned Work

- Define provider interfaces.
- Define request and response models.
- Store AI provider configuration metadata.
- Keep secrets out of the database unless secure storage is explicitly added later.
- Support provider enable/disable state.
- Add integration-style tests where possible, gated by environment variables.
- Avoid mocks.

### Design Notes

- API keys must not be stored in code or committed files.
- Provider calls should support cancellation tokens.
- Provider results should be recordable in execution history.
- Provider errors should be captured cleanly.

### Exit Criteria

- AI provider interfaces exist.
- Provider metadata can be stored.
- A provider can be called through an abstraction.
- Tests are present and safe to run without secrets.

## Phase 8 - Workflow Runner / Orchestrator

### Goal

Execute workflows by running command steps in order, passing context, recording history, and handling failures.

This is where the engine earns its name.

### Planned Work

- Define orchestration service interfaces.
- Load workflow definitions from persistence.
- Resolve command handlers.
- Execute steps in order.
- Pass context between steps.
- Record execution history.
- Support cancellation tokens.
- Define failure behavior.
- Add integration tests for complete workflow execution.

### Failure Behavior Decisions

The project should explicitly decide how workflows behave when a step fails:

- Stop immediately.
- Continue to next step.
- Run compensating steps.
- Mark later steps as skipped.

The first implementation should probably stop immediately and record the failure clearly. Fancy failure choreography can wait its turn.

### Exit Criteria

- A simple workflow can execute end-to-end.
- Execution history is recorded.
- Failed steps stop execution predictably.
- Cancellation is supported.
- Tests cover success, failure, and cancellation paths.

## Phase 9 - Host UI Tooling

### Goal

Build WinForms tooling around the engine after the core architecture is stable.

The host should inspect, configure, and execute engine features without becoming the engine itself.

### Planned Work

- Add database status display.
- Add command list view.
- Add workflow list view.
- Add execution history view.
- Add context inspection tooling.
- Add AI provider configuration tooling.
- Add command/workflow execution buttons.
- Keep UI logic thin and delegate engine behavior to services.

### Exit Criteria

- Host can display engine state.
- Host can run simple commands/workflows.
- Host can show execution results and history.
- UI code remains separate from core logic.

## Testing Strategy

Testing starts immediately and grows with the project.

### Rules

- No mocks.
- Prefer real SQLite databases.
- Use in-memory SQLite where useful.
- Use temporary file-backed databases where path and persistence behavior matters.
- Gate external AI provider tests behind environment variables.
- Keep tests deterministic.
- Add tests with each new feature.

### Test Project Roles

`Codex.CommandEngine.Tests` should cover pure domain and service behavior that does not require external services.

`Codex.CommandEngine.IntegrationTests` should cover real SQLite behavior, schema creation, persistence, and later external provider integration when explicitly enabled.

## Documentation Strategy

Documentation should stay close to the implementation.

Planned docs:

- `Roadmap.md`
- `PlannedWorkflow.md`
- `DatabaseDesign.md`
- `SqlCatalog.md`
- `CommandModel.md`
- `WorkflowModel.md`
- `ExecutionHistory.md`
- `AiProviderModel.md`
- `HostUiPlan.md`

Docs should be updated as design decisions become real implementation decisions.

## Immediate Next Step

The next implementation task should be Phase 1: Engine Database Foundation.

Recommended first work item:

1. Review the current database schema class.
2. Add or refine the migration/schema initialization model.
3. Add the `SqlQuery` and `Log` tables if they are not already present.
4. Add integration tests proving the database initializes cleanly and repeatably.
5. Keep the host untouched unless needed to verify startup behavior.

