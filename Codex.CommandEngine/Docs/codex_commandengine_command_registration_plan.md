# Codex.CommandEngine — Next Step

## Command Registration Model

This phase introduces the reusable command registration system that will become the foundation for:

- workflow execution
- AI orchestration
- command discovery
- plugin integration
- future extension loading
- UI command browsing
- execution history correlation

The design goal is to keep execution logic separated from persistence and metadata.

---

# Architectural Goals

## Requirements

The registration system must:

- support synchronous and asynchronous commands
- support cancellation tokens
- support future AI-generated commands
- support future plugin discovery
- support workflow composition
- support command metadata inspection
- support validation
- support categories/tags
- support execution permissions/security later
- support database persistence
- support versioning
- avoid reflection-heavy runtime behavior where possible

---

# Planned Components

## Core Interfaces

### ICommandHandler

Represents a command implementation.

Responsibilities:

- execute commands
- validate requests
- return structured results
- support async execution

Planned members:

- CommandName
- ExecuteAsync
- ValidateAsync

---

### ICommandRegistry

Central in-memory registration system.

Responsibilities:

- register handlers
- resolve handlers
- enumerate handlers
- prevent duplicate registrations

---

### ICommandMetadataProvider

Provides metadata for UI and persistence.

Responsibilities:

- descriptions
- categories
- tags
- examples
- version info
- parameter definitions

---

# Planned Models

## CommandDefinition

Persistent database definition.

Fields:

- CommandDefinitionId
- CommandName
- DisplayName
- Description
- Category
- Version
- IsEnabled
- HandlerType
- CreatedUtc
- UpdatedUtc

---

## CommandParameterDefinition

Defines command parameters.

Fields:

- CommandParameterDefinitionId
- CommandDefinitionId
- ParameterName
- ParameterType
- IsRequired
- DefaultValue
- Description
- SortOrder

---

## CommandExecutionRequest

Expanded request model.

Responsibilities:

- correlation tracking
- context propagation
- workflow integration
- AI provider integration later

---

# Database Work

## New Tables

### CommandDefinition

Stores persistent command metadata.

### CommandParameterDefinition

Stores parameter definitions.

---

# SQL Catalog Entries

Planned catalog queries:

- CommandDefinition_InsertOrReplace
- CommandDefinition_SelectByName
- CommandDefinition_SelectAll
- CommandDefinition_Delete
- CommandParameterDefinition_SelectByCommand

---

# Host UI Additions

## Navigation Nodes

Planned additions:

- Commands
  - Registered Commands
  - Command Definitions
  - Command Parameters

---

# Testing Strategy

## Unit Tests

- duplicate registration rejection
- null guard validation
- command resolution
- metadata retrieval

## Integration Tests

- database persistence
- SQL catalog loading
- handler registration
- schema validation
- command enumeration

No mocks will be used.

---

# Future Compatibility

The registration model is intentionally designed to support:

- AI-generated workflows
- distributed execution
- extension/plugin loading
- remote execution
- workflow graphs
- scheduling
- execution retries
- audit logging
- role/security models
- multi-provider AI orchestration

---

# Immediate Next Deliverables

The next implementation package should include:

1. ICommandHandler
2. ICommandRegistry
3. CommandDefinition model
4. CommandParameterDefinition model
5. SQLite schema migration
6. SQL catalog entries
7. registration tests
8. integration tests
9. host navigation nodes
10. schema tracking update

---

# Notes

The command registration system becomes the backbone of the entire platform.

Everything later — workflows, AI chains, orchestration, automation, plugins — will ultimately depend on this layer behaving predictably.

This is one of the places where being conservative early prevents architectural regret later.

