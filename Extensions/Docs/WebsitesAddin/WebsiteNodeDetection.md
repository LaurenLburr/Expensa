# Website Node Detection

The context menu uses:

```text
HostWebsiteTreeNodeTagReader.GetWebsiteId(TreeNode)
```

to decide whether a selected tree node is an individual website.

## Website nodes

A node is treated as a website when one of these can provide an ID:

- `TreeNode.Tag.WebsiteId`
- `TreeNode.Tag.NodeId` when the tag also has a non-empty `Url`
- JSON/string tag with `WebsiteId`
- JSON/string tag with `NodeId`
- leaf node `Name` fallback

## Group nodes

A node is not treated as a website when:

- it has child nodes
- its ID starts with `tag:`
- its ID starts with `category:`
- its ID starts with `group:`

This keeps right-click menus off category/tag group nodes while allowing website leaf nodes to receive tag assignments.
