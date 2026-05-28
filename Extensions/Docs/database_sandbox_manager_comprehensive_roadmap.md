# CodexExpensa Database Sandbox Manager Roadmap

## Vision

The Database Sandbox Manager will become the primary development and AI-assistance environment for Expensa database exploration, sandbox creation, schema analysis, add-in development, and AI-assisted engineering.

The long-term goal is to create a safe, isolated, AI-assisted database engineering platform that works alongside ExtensionDevHost.

This project should:

- isolate development from production databases
- provide realistic sandbox databases
- support add-in and extension development
- support schema exploration and query analysis
- support AI-assisted engineering workflows
- support controlled database experimentation
- become a foundation for future AI-driven code generation

---

# Core Philosophy

## Production Safety First

Production databases should never be modified directly by default.

The Sandbox Manager exists specifically to:

- create safe copies
- isolate experimentation
- validate migrations
- test add-ins
- validate generated SQL
- validate AI-generated code

---

## AI-Assisted Engineering

The Sandbox Manager is intended to become one of the first true AI-assisted engineering environments in the CodexExpensa ecosystem.

The AI should eventually:

- understand schemas
- understand relationships
- understand query catalogs
- understand naming patterns
- understand business workflows
- generate repositories
- generate models
- generate migrations
- generate tests
- explain database structures
- suggest architectural improvements

---

## Structured Architecture

The project should avoid:

- magic automation
- uncontrolled scaffolding
- hidden behavior
- uncontrolled AI modifications

The system should remain:

- explicit
- inspectable
- reviewable
- testable
- modular

---

# Solution Structure

## Proposed Solution Layout

```text
CodexExpensa.DatabaseSandboxManager
│
├── Host
│   └── CodexExpensa.DatabaseSandboxManager
│
├── Core
│   ├── CodexExpensa.DatabaseSandboxManager.Abstractions
│   ├── CodexExpensa.DatabaseSandboxManager.Core
│   ├── CodexExpensa.DatabaseSandboxManager.Data
│   ├── CodexExpensa.DatabaseSandboxManager.Models
│   └── CodexExpensa.DatabaseSandboxManager.Services
│
├── Shared
│   ├── CodexExpensa.Shared.SQLite
│   ├── CodexExpensa.Shared.AI
│   └── CodexExpensa.Shared.UI
│
├── Tests
│   ├── CodexExpensa.DatabaseSandboxManager.Tests
│   └── CodexExpensa.DatabaseSandboxManager.IntegrationTests
│
└── Docs
```

---

# Phase 1 — Foundation

## Goals

Create the initial standalone application foundation.

## Deliverables

### WinForms Host

Create:

```text
CodexExpensa.DatabaseSandboxManager
```

Features:

- menu strip
- toolbar
- status bar
- TreeView navigation
- embedded workspace panel host
- logging panel
- settings persistence

---

## Database Connection System

### Features

- register production databases
- register sandbox databases
- test connections
- open database folders
- persist connection settings
- support multiple environments

### Modes

```text
- Production
- Sandbox Copy
- In-Memory
```

---

## SQLite Integration

### Requirements

- use SQLite
- support live schema inspection
- support transaction-safe operations
- support backup operations
- support WAL mode inspection

---

## Logging

### Requirements

- transparent logging
- operation history
- AI interaction logging
- migration logging
- query execution logging

---

# Phase 2 — Schema Exploration

## Goals

Allow deep inspection of database structures.

## Tree Structure

```text
Sandbox Manager
├── Connections
├── Production Sources
├── Sandbox Databases
├── Schemas
├── Tables
├── Views
├── Indexes
├── Foreign Keys
├── Query Catalog
└── Settings
```

---

## Table Explorer

### Features

- list tables
- list columns
- show PKs
- show FKs
- show indexes
- show row counts
- show create scripts
- show sample rows

---

## Relationship Explorer

### Features

- FK graph visualization
- parent/child relationships
- dependency chains
- orphan detection
- circular relationship detection

---

## Query Catalog Integration

### Features

- inspect SqlQuery table
- categorize queries
- validate queries
- search queries
- AI-assisted query explanation

---

# Phase 3 — Sandbox Builder

## Goals

Allow creation of safe development databases.

## Sandbox Creation Workflow

### Step 1

Select source database.

### Step 2

Select tables.

### Step 3

Choose copy strategy.

### Step 4

Generate sandbox database.

---

## Table Selection System

### Features

- multi-select tables
- FK-aware selection
- dependency suggestions
- include/exclude child tables
- schema-only mode
- schema + sample data mode
- full copy mode

---

## Data Sampling

### Features

- row limits
- deterministic sampling
- random sampling
- date filtering
- relationship-aware sampling

---

## Data Anonymization

### Features

- replace names
- replace addresses
- replace emails
- replace account numbers
- deterministic masking
- configurable masking rules

---

# Phase 4 — AI Database Intelligence

## Goals

Teach the AI to understand the database.

---

## Schema Explanation

### AI Features

AI should explain:

- probable table purpose
- likely workflows
- probable repository usage
- naming inconsistencies
- normalization concerns
- indexing concerns
- migration risks

---

## Relationship Suggestions

### AI Features

AI should:

- infer relationships
- suggest missing indexes
- suggest related tables
- identify weak naming patterns
- identify likely audit/history tables

---

## Query Analysis

### AI Features

AI should:

- explain queries
- suggest optimizations
- detect anti-patterns
- explain JOIN chains
- explain business purpose

---

## Migration Review

### AI Features

AI should:

- review migrations
- explain risk
- detect destructive operations
- suggest rollback strategies

---

# Phase 5 — AI-Assisted Code Generation

## Goals

Generate practical implementation code from real schema.

---

## Repository Generation

### Generate

- repositories
- models
- DTOs
- interfaces
- services
- query wrappers

---

## SQLite Query Generation

### Generate

- SELECT queries
- INSERT queries
- UPDATE queries
- DELETE queries
- UPSERT queries
- SqlQuery catalog entries

---

## UI Generation

### Generate

- WinForms editors
- grid forms
- search forms
- detail forms
- validation logic

---

## Test Generation

### Generate

- integration tests
- repository tests
- migration tests
- schema validation tests

---

# Phase 6 — ExtensionDevHost Integration

## Goals

Connect Sandbox Manager with ExtensionDevHost.

---

## Shared Context

### Shared Items

- database settings
- sandbox locations
- query catalogs
- AI instruction documents
- project metadata

---

## Launch Integration

### Features

ExtensionDevHost should:

- launch Sandbox Manager
- request sandbox generation
- request schema inspection
- request AI schema explanation

---

## Add-in Validation

### Features

Sandbox Manager should:

- validate add-in DB usage
- validate query usage
- validate migration compatibility

---

# Phase 7 — Manager AI Conversation

## Goals

Create a dedicated operational AI lane.

---

## Conversation Types

### Design Conversation

Purpose:

- architecture
- workflows
- specifications
- planning

---

### Manager Conversation

Purpose:

- operational tasks
- tool actions
- maintenance
- generation requests
- sandbox management

---

## Example Manager Requests

```text
Create a sandbox with Website tables.

Explain the AccountTag schema.

Generate repositories for Budget tables.

Analyze unused queries.

Suggest indexes.
```

---

# Phase 8 — Advanced AI Engineering

## Goals

Move toward true AI-assisted engineering workflows.

---

## Long-Term AI Goals

### AI should eventually:

- understand the full schema graph
- understand business workflows
- understand add-in architecture
- understand coding conventions
- generate implementation plans
- generate migrations
- generate repositories
- generate UI
- validate generated code
- explain failures
- recommend architecture changes

---

## AI Safety Rules

### Requirements

AI must:

- never modify production directly
- require review before applying generated code
- preserve architectural conventions
- follow GeneralCodingRules.md
- follow project templates
- preserve SqlQuery catalog usage

---

# UI Roadmap

## Planned Panels

```text
- Database Connection
- Schema Explorer
- Table Explorer
- FK Graph Viewer
- Query Catalog Viewer
- Sandbox Builder
- Data Sampler
- Data Masking Rules
- AI Schema Analysis
- AI Query Analysis
- Migration Review
- Repository Generator
- UI Generator
- Test Generator
- AI Manager Conversation
```

---

# Persistence

## Settings

Persist:

- database connections
- sandbox paths
- AI settings
- recent sandboxes
- query history
- window layouts
- selected tables

---

# Coding Standards

## Project Rules

- use C#
- use WinForms
- use SQLite
- avoid mocks
- prefer integration tests
- use guard clauses
- store reusable SQL in SqlQuery
- use full-file updates
- preserve explicit architecture

---

# Immediate Next Steps

## Priority Order

### 1

Create standalone solution.

### 2

Implement:

```text
Database Connection Panel
```

### 3

Implement:

```text
Schema Explorer
```

### 4

Implement:

```text
Sandbox Builder
```

### 5

Implement:

```text
Table Selection UI
```

### 6

Implement:

```text
AI Schema Explanation
```

### 7

Implement:

```text
Manager AI Conversation
```

---

# Final Goal

The long-term goal is to evolve from:

```text
Database utility
```

into:

```text
AI-assisted database engineering platform
```

for:

- Expensa
- add-ins
- extensions
- migrations
- schema exploration
- query engineering
- repository generation
- UI generation
- AI-assisted software development

