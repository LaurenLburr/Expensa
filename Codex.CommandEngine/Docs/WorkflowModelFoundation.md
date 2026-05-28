# Codex.CommandEngine - Workflow Model Foundation

## Purpose

This step introduces the first persistent workflow model layer.

Workflows are ordered collections of command steps. The workflow layer does not execute steps yet. This phase only defines how workflows and steps are stored, listed, and inspected.

## Added Concepts

- `WorkflowDefinition`
- `WorkflowStepDefinition`
- `WorkflowDefinitionRepository`
- workflow SQL catalog entries
- host navigation nodes for workflow definitions and steps
- schema version `0004_workflow_model_foundation`

## Database Changes

Schema version `0004` adds explicit workflow metadata fields:

- `WorkflowDefinition.DisplayName`
- `WorkflowStep.StepName`

These are intentionally UI-friendly fields. The engine should not force users to decode raw IDs just to understand a workflow.

## SQL Catalog Entries

The initializer now seeds workflow catalog queries:

- `WorkflowDefinition_InsertOrReplace`
- `WorkflowDefinition_SelectAll`
- `WorkflowDefinition_SelectByNameAndVersion`
- `WorkflowStep_InsertOrReplace`
- `WorkflowStep_SelectByWorkflow`

## Host UI

The host tree now includes:

- `Workflows`
  - `Workflow Definitions`
  - `Workflow Steps`

The nodes inspect persisted workflow metadata in the engine database.

## Testing

Integration tests cover:

- workflow upsert and retrieval
- workflow step ordering
- workflow SQL catalog seeding
- workflow schema snapshot storage

No mocks are used.

## Next Step

The next planned step is execution history foundation.

That will add the durable audit trail for command and workflow executions before the actual workflow runner gets smarter.
