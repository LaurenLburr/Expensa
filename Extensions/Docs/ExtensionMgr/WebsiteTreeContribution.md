# Website Tree Contribution

The Websites extension must not own or clear the host tree.

The host owns the `TreeView`.

Websites contributes only this known root node:

```text
Websites
```

Behavior:
- If the `Websites` root node does not exist, it is added.
- If the `Websites` root node already exists, only its child nodes are replaced.
- Other top-level nodes, such as `Accounts` or `Budgets`, are preserved.
