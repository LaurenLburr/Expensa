# Extension Dev Host Roadmap Node Fix

## Problem

`WorkspaceRoadmap.md` existed or was being seeded, but the roadmap still did not appear in the tree.

## Fix

`MainForm.BuildNavigationTree()` now explicitly adds:

```csharp
docs.Nodes.Add(CreateWorkspaceDocNode("Workspace Roadmap", "WorkspaceRoadmap.md"));
```

The fix also ensures:

```csharp
CreateWorkspaceDocNode(...)
EnsureWorkspaceDocumentsExist()
GetWorkspaceDocsFolder()
```

are present and wired to `WorkspaceDocumentService`.

## Behavior

The tree should now show:

```text
Docs
├── AI Add-in Design Workflow
└── Workspace Roadmap
```

Selecting `Workspace Roadmap` opens the editable Markdown document.

## Files

```text
Extensions/MainForm.cs
Services/WorkspaceDocumentService.cs
```
