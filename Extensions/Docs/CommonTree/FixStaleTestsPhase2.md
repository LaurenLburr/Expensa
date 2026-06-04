# Fix Stale Tests Phase 2

This slice updates tests that were still checking old implementation details.

## Updated

```text
HostWebsiteTreeViewRendererCompatibilityTests
HostWebsiteTreeNodeTagReaderTests
BudgetsAddinPhase1StructureTests
BudgetsExtMgrPanelsPhase1StructureTests
```

## Why

Production moved forward:

- Budgets now queries `BudgetMonth`, not `Budget`.
- Budgets database panel now uses a Websites-style add-in DB workflow.
- Website tree reader now intentionally has compatibility fallback behavior.
- Renderer tests verify public compatibility behavior instead of exact newline formatting.
