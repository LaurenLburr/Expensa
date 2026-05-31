# Websites Tree Context Menu Runtime Attach

The Websites tree context menu is now attached at runtime instead of relying only on designer wiring.

## Why

The designer wiring may not have been applied to the actual runtime form instance.

## Runtime behavior

On form load:

```text
AttachRuntimeTreeContextMenu()
```

creates and attaches:

```text
Sort ASC
Sort DESC
```

The form also handles right mouse-up directly and shows the menu at the clicked location.

Right-clicking a tree node selects that node before showing the context menu.
