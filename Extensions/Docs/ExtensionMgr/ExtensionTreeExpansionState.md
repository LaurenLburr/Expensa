# Extension Tree Expansion State

The Extension Tree Load Test now defaults to collapsed nodes on first load, but remembers expanded nodes from the previous run.

State is tracked by `TreeNode.Name`.

Behavior:
- First load: collapsed.
- Expand nodes.
- Reload: previously expanded nodes are restored.
