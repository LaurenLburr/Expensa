# Website Load Command

This slice starts the bridge from the CommandEngine system into real Expensa behavior.

## Command

```text
websites.load
```

## Purpose

Loads Websites data into a UI-neutral tree model.

The command does **not** directly manipulate a WinForms `TreeView`.

Instead it returns:

```text
WebsiteLoadResult
  Nodes[]
    WebsiteTreeNode
```

The DevHost can execute the command first. Later, Expensa can map the returned nodes into the real UI.

## Parameters

```json
{
  "searchText": "",
  "includeDisabled": false,
  "maximumRows": 500
}
```

## Why This Shape

This keeps the bridge safe:

- no direct Expensa form dependency
- no direct TreeView dependency
- command is testable
- output can be persisted/replayed
- AI can inspect the output later
- Expensa UI stays responsible for rendering only
