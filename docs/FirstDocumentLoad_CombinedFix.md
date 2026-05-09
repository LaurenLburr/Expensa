# First Document Load Combined Fix

## Problem

The embedded document editor appeared on the first document click, but the document did not load until selecting away and returning.

## Fix

This fixes both sides of the timing issue.

### MainForm

`ShowGlobalDocument` and `ShowProjectDocument` now call:

```csharp
EnsureDocsEditorAndLoad(...)
```

If the editor was just created, the document load is deferred one UI cycle using:

```csharp
BeginInvoke(...)
```

### DocsEditorForm

`LoadDocumentPath` now forces control creation/layout before rendering.

`RenderPreview` now:
- creates the WebBrowser handle if needed,
- sets `DocumentText`,
- queues one additional refresh with `BeginInvoke`.

## Files

```text
Extensions/MainForm.cs
Extensions/MainForm.Designer.cs
Extensions/MainForm.resx
UI/DocsEditorForm.cs
UI/DocsEditorForm.Designer.cs
UI/DocsEditorForm.resx
```
