# Extension Tree Loader Registry

This slice introduces provider-based tree loader registration.

## Previous shape

The host catalog directly returned concrete loaders:

```csharp
new WebsitesExtensionTreeNodeLoader()
```

That worked, but it made the host know too much about each add-in.

## New shape

Providers expose loaders:

```csharp
IExtensionTreeNodeLoaderProvider
```

The registry asks providers for loaders, flattens them, and sorts by:

```text
SortOrder
AddinId
```

## Current flow

```text
ExtensionTreeNodeLoaderProviderCatalog
  -> WebsitesExtensionTreeNodeLoaderProvider
      -> WebsitesExtensionTreeNodeLoader

ExtensionTreeNodeLoaderRegistry
  -> flattens loaders
  -> sorts loaders

ExtensionTreeLoadOrchestrator
  -> loads nodes in sorted order
```

## Why this matters

This is the bridge toward module discovery.

The next version can replace the hardcoded provider catalog with a module-scanning provider catalog.
