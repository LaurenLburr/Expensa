# DocsEditorForm AI Highlight Fix

## Problem

The AI review changed text was assigned back into `sourceEditor.Text`, which triggered `TextChanged`.
That event cleared the highlight set before the preview rendered, so no yellow highlighting appeared.

Tiny bug. Annoying bug. Very WinForms.

## Fix

Added:

```csharp
private bool _suppressPreviewHighlightClear;
```

During AI review, the code now:

1. Calculates changed line indexes.
2. Suppresses the `TextChanged` highlight clear.
3. Assigns the reviewed text.
4. Renders the preview with yellow highlights.
5. Asks whether to save.

## Expected behavior

Manual edits do **not** highlight immediately.

Yellow highlighting appears after:

```text
AI Review → AI returns corrected text
```

Then:

```text
Yes    saves and clears highlights
No     keeps changes in editor and leaves highlights visible
Cancel restores original text and clears highlights
```
