# Website Tree Pipeline Alignment

There are two tree-loading surfaces:

```text
Websites test tree:
HostWebsiteTreeContributionLoader.LoadContributionAsync(...)
    -> HostWebsiteTreeViewRenderer.Render(...)

Extension manager contribution tree:
HostWebsiteTreeContributionRenderer.RenderContribution(...)
    -> HostWebsiteTreeViewNodeMapper.ToTreeNodes(...)
```

Both now use the same typed payload path:

```text
HostWebsiteTreeViewNodeMapper.ToTreeNode(...)
    -> HostWebsiteCategoryGroupTreeNodePayload
    -> HostWebsiteTreeNodePayload
```

This means category/group nodes and website nodes expose a definitive node type through:

```text
IHostWebsiteTreeNodePayload.NodeType
```

No more guessing from text, except as a compatibility fallback in `HostWebsiteTreeNodeTagReader`.
