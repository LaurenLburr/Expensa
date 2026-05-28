# Codex Command Engine Roadmap

## Purpose

Codex Command Engine is a reusable command orchestration platform for CodexExpensa and future Codex projects.

## Starting Scope

- command routing
- command history
- workflow orchestration
- SQLite persistence
- future AI manager conversation support

## Testing Rule

Tests are mandatory from the beginning.

## Schema Tracking

Database schema changes are tracked through versioned SQL snapshots in `Docs\Database\SchemaSnapshots`, the `MigrationHistory` table, and the `DatabaseSchemaSnapshot` table. Every schema-changing step should include matching documentation and integration tests.
