# Rendered Design Spec Preview Fix

## Problem

The `Rendered Design Spec` pane could show blank even when the Markdown source was loaded.

## Cause

`WebBrowser.DocumentText` can be unreliable when assigned before the embedded browser has fully initialized inside an embedded WinForms panel.

## Fix

`RenderDesignSpecPreview()` now:

- ensures the browser handle exists
- uses `Document.OpenNew(true)`
- writes the generated HTML directly into the document
- falls back to `DocumentText` if needed
- schedules an initial render through `BeginInvoke(...)` after load

## Files

```text
UI/DesignSpecConversationForm.cs
UI/DesignSpecConversationForm.Designer.cs
UI/DesignSpecConversationForm.resx
```
