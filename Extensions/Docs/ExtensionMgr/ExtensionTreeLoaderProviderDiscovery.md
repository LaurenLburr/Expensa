# Extension Tree Loader Provider Discovery

This slice starts moving the tree loader system toward real module discovery.

## New pieces

```text
ExtensionModuleAssemblyLoader
ExtensionTreeNodeLoaderProviderDiscovery
ExtensionModuleFolderResolver
ExtensionTreeNodeLoaderProviderCatalog
```

## Flow

```text
Modules folder
  -> load *.dll files
  -> scan assemblies for IExtensionTreeNodeLoaderProvider
  -> instantiate providers
  -> collect loaders
  -> orchestrator loads tree
```

## Temporary fallback

If no providers are found in the Modules folder, the catalog falls back to:

```csharp
WebsitesExtensionTreeNodeLoaderProvider
```

That keeps the current DevHost test surface working while we move toward true module discovery.
