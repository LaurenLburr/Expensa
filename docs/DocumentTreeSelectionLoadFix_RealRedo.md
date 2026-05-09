# Document Tree Selection Load Fix - Real Redo

## Important

This package was regenerated from the uploaded project file:

```text
CodexExpensa.ExtensionDevHost.upload.zip
```

It is not placeholder content.

## Fix

Selecting a document in the main tree now:

1. Ensures `DocsEditorForm` exists.
2. Immediately calls `SelectDocumentByTag(...)` or `LoadDocumentPath(...)`.
3. Requests one deferred preview refresh after host layout.

`DocsEditorForm` now:

- loads the file synchronously,
- creates its handle if needed,
- updates `sourceEditor.Text`,
- renders immediately,
- exposes `RefreshPreviewAfterHostLayout()`.

## Files

```text
Extensions/Host/CodexExpensa.ExtensionDevHost/Extensions/MainForm.cs
Extensions/Host/CodexExpensa.ExtensionDevHost/Extensions/MainForm.Designer.cs
Extensions/Host/CodexExpensa.ExtensionDevHost/Extensions/MainForm.resx
Extensions/Host/CodexExpensa.ExtensionDevHost/UI/DocsEditorForm.cs
Extensions/Host/CodexExpensa.ExtensionDevHost/UI/DocsEditorForm.Designer.cs
Extensions/Host/CodexExpensa.ExtensionDevHost/UI/DocsEditorForm.resx
```
