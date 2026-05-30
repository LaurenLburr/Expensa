# Tree Contribution Descriptor Loader

This slice connects convention-based module contribution metadata to the host tree loading pipeline.

## Flow

```text
Modules folder
  -> load assemblies
  -> discover *TreeContributionProvider types
  -> read descriptors
  -> create ModuleCommandTreeNodeLoader
  -> ExtensionTreeLoadOrchestrator loads nodes
```

## Current adapter support

The first supported command adapter is:

```text
websites.load
```

That command is routed through the existing Websites host contribution loader.

Unsupported contribution commands fail clearly with:

```text
No host tree contribution adapter is registered for command ...
```

This gives us a safe bridge while command-specific contribution adapters are formalized.
