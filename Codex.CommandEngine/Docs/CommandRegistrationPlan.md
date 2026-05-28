# Codex.CommandEngine - Command Registration Foundation

This phase introduces the reusable command registration system that becomes the foundation for workflows, AI orchestration, plugin integration, execution history, and future host tooling.

## Implemented Components

### Abstractions

- `ICommandHandler`
- `ICommandRegistry`
- `CommandDefinition`
- `CommandParameterDefinition`
- `CommandHandlerDescriptor`

### Core

- `CommandRegistry`
- `DelegateCommandHandler`
- updated `CommandEngine` using the registry

### Data

- `CommandDefinitionRepository`
- `CommandDefinitionRecord`
- `CommandParameterDefinitionRecord`
- `CommandDefinitionUpsert`
- `CommandParameterDefinitionUpsert`

### Database

- schema version `3`
- migration ID `0003_command_registration_foundation`
- `CommandDefinition.DisplayName`
- `CommandDefinition.HandlerType`
- `CommandParameterDefinition`
- command registration SQL catalog entries

### Host UI

- `Commands -> Registered Commands`
- `Commands -> Command Definitions`
- `Commands -> Command Parameters`

## Design Notes

The registry is intentionally in-memory. Persistence stores command metadata, not executable code. This keeps runtime handler resolution separated from database state and leaves room for future plugin loading without turning the database into a bucket of executable strings. That road leads to goblins.

## Testing

Tests cover:

- duplicate command registration rejection
- command resolution
- handler listing order
- validation failure behavior
- command definition persistence
- parameter persistence
- SQL catalog seed verification
- schema snapshot synchronization

## Next Step

The next practical layer is workflow definition foundation:

- workflow model cleanup
- workflow step persistence
- workflow SQL catalog entries
- host workflow nodes
- workflow validation tests
