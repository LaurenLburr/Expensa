# Extension Dev Host Refactor Step 6c - Tree Single Click Routing

## Problem

Some embedded tool nodes still appeared to require a double-click.

## Cause

`AfterSelect` only fires when the selected node changes. If the node was already selected, clicking it again did nothing, while double-click still executed the command.

## Fix

Added:

```csharp
NavigationTreeView_NodeMouseClick
```

and wired:

```csharp
navigationTreeView.NodeMouseClick += NavigationTreeView_NodeMouseClick;
```

Single-clicking a node now always calls:

```csharp
ShowNavigationNode(e.Node);
```

Double-click remains available for folder/open actions.
