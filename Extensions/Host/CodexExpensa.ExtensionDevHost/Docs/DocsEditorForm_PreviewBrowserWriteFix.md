# DocsEditorForm Preview Browser Write Fix

## Problem

The source text loads, but the rendered Markdown preview remains blank.

## Cause

`WebBrowser.DocumentText` is unreliable during first render/layout for embedded WinForms forms. The control may not have an initialized document yet.

## Fix

`RenderPreview()` now:

1. Builds the HTML.
2. Stores it in `_pendingPreviewHtml`.
3. Ensures the WebBrowser handle exists.
4. Navigates to `about:blank` if no document exists.
5. Writes HTML using:

```csharp
document.OpenNew(replaceInHistory: true);
document.Write(html);
document.Close();
```

6. Handles `DocumentCompleted` and writes pending HTML after the browser document is ready.

## Files

```text
UI/DocsEditorForm.cs
UI/DocsEditorForm.Designer.cs
UI/DocsEditorForm.resx
```
