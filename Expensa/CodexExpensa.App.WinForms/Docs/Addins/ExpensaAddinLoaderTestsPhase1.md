# Expensa Add-in Loader Tests Phase 1

This slice adds first-pass regression tests around the Expensa aggregate tree add-in loader.

## Covered

```text
MainForm aggregate loader wiring
TreeAddinDefinition
TreeAddinAssemblyLocator
TreeAddinRuntimeInvoker
TreeAddinTreeViewLoader
TreeAddinTreeNodeFactory
```

These are structure tests. The next phase should add live integration tests gated by an environment variable after the deployed add-ins path is stable.
