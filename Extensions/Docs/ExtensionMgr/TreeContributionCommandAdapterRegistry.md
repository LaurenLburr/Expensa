# Tree Contribution Command Adapter Registry

This slice removes command-specific branching from `ModuleCommandTreeNodeLoader`.

## Before

The loader directly checked:

```csharp
if (_descriptor.CommandName == "websites.load")
```

That was a smell. The loader knew too much about Websites.

## After

The loader asks:

```csharp
TreeContributionCommandAdapterRegistry
```

for an adapter by command name.

Current adapter:

```text
websites.load -> WebsitesTreeContributionCommandAdapter
```

## Flow

```text
Module descriptor
  -> ModuleCommandTreeNodeLoader
  -> TreeContributionCommandAdapterRegistry
  -> WebsitesTreeContributionCommandAdapter
  -> HostWebsiteTreeContributionLoader
  -> TreeView Websites node
```

This is not fully generic command dispatch yet, but it removes the hardcoded command branch and gives us a clean extension point for future tree contribution commands.
