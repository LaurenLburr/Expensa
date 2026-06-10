# CommonTree Expansion State and Context Menu

Adds shared tree behavior to:

```text
AddinTreeRenderer
```

## New behavior

Every tree rendered through `AddinTreeRenderer.Render(...)` now gets:

```text
remember expanded nodes across reloads
right-click node with children -> Expand All
right-click node with children -> Collapse All
```

## Expanded state

Before clearing and rebuilding the tree, the renderer captures expanded node names:

```csharp
CaptureExpandedNodeNames(treeView)
```

After rebuilding, if `expandAll` is false, it restores matching expanded nodes by `TreeNode.Name`.

If `expandAll` is true, it still expands everything.

## Context menu

The renderer installs a shared context menu on the `TreeView`.

The menu only appears when right-clicking a node with children.

```text
Expand All
Collapse All
```

## Why here

This belongs in the shared renderer, not in Payees/Websites/Budgets forms.

That keeps tree behavior consistent across all add-ins using the CommonTree template.
