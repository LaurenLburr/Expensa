# Expensa Host Website Tree Adapter

This slice adds the WinForms-side adapter for rendering `websites.load` output.

The important boundary:

```text
WebsitesAddin
  produces JSON / UI-neutral tree model

Host / Expensa WinForms layer
  parses JSON
  renders TreeView
```

## New host types

```text
HostWebsiteLoadResult
HostWebsiteTreeNode
HostWebsiteLoadResultParser
HostWebsiteTreeViewNodeMapper
HostWebsiteTreeViewRenderer
HostWebsiteLoadExecutionResultAdapter
```

## Flow

```text
websites.load
  -> CommandExecutionResult.OutputJson
  -> HostWebsiteLoadExecutionResultAdapter
  -> HostWebsiteLoadResult
  -> TreeView
```

This keeps WinForms out of `WebsitesAddin`.
