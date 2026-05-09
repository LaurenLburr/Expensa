# DocsEditorForm HtmlDocument Close Fix

Removed invalid call:

```csharp
document.Close();
```

`HtmlDocument` does not expose a `Close()` method in WinForms.
