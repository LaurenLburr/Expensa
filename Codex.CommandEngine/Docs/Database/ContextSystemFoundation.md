# Context System Foundation

Schema version: 0006

This step adds the first formal context repository layer to Codex.CommandEngine. Contexts are reusable JSON payloads that can describe the current solution, project, workflow, command execution, AI prompt state, or any other structured runtime environment.

## Added

- `EngineContextUpsert`
- `EngineContextRecord`
- `EngineContextRepository`
- schema version `0006_context_system_foundation`
- `ExecutionContext` scoping columns
- context SQL catalog entries
- host node: `Contexts -> Context Definitions`
- integration tests for insert, update, list, catalog seeding, and schema snapshot tracking

## ExecutionContext table

The table now tracks:

- `ContextId`
- `ContextName`
- `Scope`
- `Description`
- `ContextJson`
- `MetadataJson`
- `IsEnabled`
- `CreatedUtc`
- `UpdatedUtc`

## SQL catalog entries

- `ExecutionContext_InsertOrReplace`
- `ExecutionContext_SelectByName`
- `ExecutionContext_SelectAll`

## Notes

This is intentionally lightweight. The context system stores structured JSON but does not interpret it yet. Interpretation belongs to future orchestration and AI workflow layers. That keeps this foundation reusable instead of turning it into a hard-coded junk drawer with a logo.
