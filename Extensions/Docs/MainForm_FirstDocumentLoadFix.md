# MainForm First Document Load Fix

## Problem

On startup, the first click on a text document created and showed the embedded `DocsEditorForm`, but the selected document sometimes did not load into the editor until selecting another node and then returning.

## Cause

The first document load was happening immediately after creating the embedded WinForms child form. The form and child controls had not fully completed the show/layout lifecycle yet.

WinForms: because apparently one lifecycle event was too simple.

## Fix

`ShowGlobalDocument` and `ShowProjectDocument` now route through:

```csharp
EnsureDocsEditorReady(Action loadDocumentAction)
```

When the editor is newly created, the first document load is deferred with:

```csharp
_docsEditorForm.BeginInvoke(...)
```

This lets the embedded editor finish handle/control initialization before `SelectDocumentByTag` or `LoadDocumentPath` runs.

## Files

```text
Extensions/Host/CodexExpensa.ExtensionDevHost/Extensions/MainForm.cs
Extensions/Host/CodexExpensa.ExtensionDevHost/Extensions/MainForm.Designer.cs
Extensions/Host/CodexExpensa.ExtensionDevHost/Extensions/MainForm.resx
```
