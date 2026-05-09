# DocsEditorForm First Load Render Retry Fix

## Problem

The previous queued-load fix prevented documents from loading at all because `DocsEditorForm` is embedded as a non-top-level child form. `Shown` is not reliable for that case.

## Fix

This version removes the queued-load approach.

Documents load immediately again.

`RenderPreview()` now:
- ensures the WebBrowser handle exists,
- sets `DocumentText`,
- then queues one additional preview refresh with `BeginInvoke`.

That gives WinForms one UI-message cycle to finish layout before the rendered Markdown is refreshed.

## Expected behavior

The first document click should load the selected document immediately.

No selecting another node and coming back. No ritual sacrifice to WinForms.
