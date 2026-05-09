# Extension Dev Host Refactor Step 7 - Document Workspace REAL REDO

## Scope

This is the corrected Step 7 package. It includes the actual `MainForm.cs` changes, not just documentation.

## Changes

### Templates

These nodes now open editable workspace markdown documents:

```text
Templates
├── Add-in Design Spec Template
└── Expensa Integration Spec Template
```

### Docs

This node now opens an editable workspace markdown document:

```text
Docs
└── AI Add-in Design Workflow
```

## Workspace Document Storage

Workspace-level docs are stored at:

```text
%AppData%\Expensa\Extensions\WorkspaceDocs
```

## Documents Created Automatically

```text
AddinDesignSpecTemplate.md
ExpensaIntegrationSpecTemplate.md
AiAddinDesignWorkflow.md
```

## Implementation

`MainForm` now includes:

```csharp
EnsureWorkspaceDocumentsExist()
GetWorkspaceDocsFolder()
EnsureWorkspaceDocument(...)
CreateWorkspaceDocNode(...)
```

The constructor calls:

```csharp
EnsureWorkspaceDocumentsExist();
```

before building the navigation tree.
