# Extension Dev Host Refactor Step 10 - Workspace Document Service

## Problem

`Docs -> Workspace Roadmap` could appear as an empty document.

## Cause

The workspace document seeding logic only created files when they were missing. If a document already existed but was empty, it was left empty.

Also, `MainForm` was starting to become a document factory. That was useful during the refactor, but it needed to stop before it became architecture by accident.

## Fix

Added:

```text
Services/WorkspaceDocumentService.cs
```

## Behavior

`WorkspaceDocumentService` now owns workspace document seeding.

It ensures these default docs exist:

```text
AddinDesignSpecTemplate.md
ExpensaIntegrationSpecTemplate.md
AiAddinDesignWorkflow.md
WorkspaceRoadmap.md
```

If a file is missing, it creates it.

If a file exists but is empty or whitespace-only, it reseeds it.

If a file already has content, it leaves it alone.

## MainForm Change

`MainForm` now delegates to:

```csharp
_workspaceDocuments.EnsureDefaultDocuments();
```

instead of directly creating workspace docs itself.

## Storage Location

```text
%AppData%\Expensa\Extensions\WorkspaceDocs
```

## Files

```text
Extensions/MainForm.cs
Services/WorkspaceDocumentService.cs
```
