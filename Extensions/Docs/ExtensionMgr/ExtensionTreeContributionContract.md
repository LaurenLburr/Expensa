# Extension Tree Contribution Contract

This slice introduces the contract Expensa should use for tree loading.

## Core idea

Expensa owns the tree.

Extensions do **not** own the whole tree.

Each extension contributes only its known node.

```text
Expensa TreeView
  -> ExtensionTreeLoadOrchestrator
      -> WebsitesExtensionTreeNodeLoader
          loads Websites node(s)
      -> AccountsExtensionTreeNodeLoader
          later loads Accounts node(s)
      -> BudgetsExtensionTreeNodeLoader
          later loads Budgets node(s)
```

## Contract

```csharp
public interface IExtensionTreeNodeLoader
{
    string AddinId { get; }
    string DisplayName { get; }
    int SortOrder { get; }

    Task LoadNodeAsync(TreeView treeView, CancellationToken cancellationToken = default);
}
```

## Orchestrator

```csharp
ExtensionTreeLoadOrchestrator
```

The orchestrator:
- sorts loaders by SortOrder
- clears the host tree
- calls each loader in order
- records success/failure summary

## Websites implementation

```csharp
WebsitesExtensionTreeNodeLoader
```

This implementation calls the existing Websites tree bridge and loads only the Websites contribution.
