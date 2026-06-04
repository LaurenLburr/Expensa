# Expensa Loader TreeView Phase 2

This slice replaces the raw Expensa add-in loader test with a TreeView-based diagnostic form.

## Changes

- The loader test now renders add-in output into a TreeView.
- It can load Websites, Budgets, or both.
- Both add-ins are invoked through the same `ExpensaStyleAddinRuntimeInvoker`.
- MainForm routes Budgets database/test nodes correctly.

## Node

```text
Tools -> Expensa Add-in Loader Test
```

## Important

This is the Extension Manager diagnostic version of the loader that Expensa should use when the add-ins are integrated back into the app tree.
