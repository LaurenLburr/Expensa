# DocsEditorForm Missing Methods Fix

## Problem

`DocsEditorForm.cs` called these methods:

```csharp
ApplyToolbarIcons();
ApplySafeSourceHeight();
```

but the methods were missing from the file, causing compile errors.

## Fix

Restored:

```csharp
private void ApplyToolbarIcons()
private static Bitmap CreateTextIcon(...)
private static Bitmap CreateFolderIcon()
private static Bitmap CreateTableIcon()
private void ApplySafeSourceHeight()
```

## Files Included

```text
Extensions/Host/CodexExpensa.ExtensionDevHost/UI/DocsEditorForm.cs
Extensions/Host/CodexExpensa.ExtensionDevHost/UI/DocsEditorForm.Designer.cs
Extensions/Host/CodexExpensa.ExtensionDevHost/UI/DocsEditorForm.resx
```
