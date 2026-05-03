# DocsEditorForm Initial Document Load Fix

## Problem

On startup, the first text-doc selection created the embedded `DocsEditorForm`, but the editor/preview appeared empty. Selecting another node and then returning loaded the document correctly.

## Cause

The initial document load could run before the embedded `DocsEditorForm` completed its first `Shown` lifecycle and before the preview/editor controls were fully ready.

## Fix

`DocsEditorForm.LoadDocumentPath(...)` now queues the first requested document when the form has not completed its first `Shown` event.

The queued document is loaded by:

```csharp
FlushPendingInitialDocumentLoad()
```

after the form has completed its first show/layout cycle.

`RenderPreview()` also now ensures the WebBrowser handle exists before assigning `DocumentText`.

## Files

```text
Extensions/Host/CodexExpensa.ExtensionDevHost/UI/DocsEditorForm.cs
Extensions/Host/CodexExpensa.ExtensionDevHost/UI/DocsEditorForm.Designer.cs
Extensions/Host/CodexExpensa.ExtensionDevHost/UI/DocsEditorForm.resx
```
