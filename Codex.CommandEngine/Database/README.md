# Full Current CommandEngine Database

This is the complete current development database snapshot.

Use this directly instead of incremental migrations during active development.

Recommended workflow:
1. Copy this DB to the runtime/test location.
2. Open it directly.
3. Skip migrations entirely in dev/test mode.

This DB already contains:
- aligned AiProvider schema
- aligned ExecutionContext schema
- SQL catalog entries
- compatibility aliases
- default provider/context seed data
