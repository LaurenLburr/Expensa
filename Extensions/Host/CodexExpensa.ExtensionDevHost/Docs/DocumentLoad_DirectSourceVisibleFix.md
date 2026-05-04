# Document Load Direct Source Visible Fix

## Why this version exists

The previous preview/timing fixes still left the editor appearing without visible document content.

This version removes the ambiguity:

- MainForm loads the selected document synchronously.
- DocsEditorForm loads the file text directly into `sourceEditor`.
- The source pane is automatically shown when a document loads.

If the selected file loads, you will see its Markdown text immediately.

## Notes

This intentionally shows the source pane after document selection. Once document routing is confirmed stable, the default can be switched back to rendered-preview-only.

## Files

```text
Extensions/MainForm.cs
Extensions/MainForm.Designer.cs
Extensions/MainForm.resx
UI/DocsEditorForm.cs
UI/DocsEditorForm.Designer.cs
UI/DocsEditorForm.resx
```
