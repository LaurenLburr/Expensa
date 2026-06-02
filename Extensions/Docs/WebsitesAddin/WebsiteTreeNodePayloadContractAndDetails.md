# Website Tree Node Payload Contract and Details

All objects loaded into the Websites tree now carry a definitive node type.

## Interface

```text
IHostWebsiteTreeNodePayload
```

## Node types

```text
CategoryGroup
Website
```

## Runtime loading path

Database-backed website loading starts in:

```text
SqliteWebsiteRepository.LoadWebsites(...)
```

Tree rendering into WinForms starts in:

```text
HostWebsiteTreeViewRenderer.Render(...)
```

Selecting a website node loads the details panel.
