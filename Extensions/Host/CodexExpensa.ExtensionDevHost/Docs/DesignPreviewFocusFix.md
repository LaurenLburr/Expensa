# Design Preview Focus Fix

## Problem

Typing in the Markdown Source pane could cause focus to jump into the Rendered Design Spec preview.

## Fix

The Design Spec conversation form now:

- debounces preview rendering with a short timer
- saves the Markdown editor selection before rendering
- restores the selection after rendering
- restores focus to the Markdown editor if it had focus before rendering
- disables tab focus on the preview browser

## Files

```text
UI/DesignSpecConversationForm.cs
UI/DesignSpecConversationForm.Designer.cs
UI/DesignSpecConversationForm.resx
```

## Notes

This preserves caret and selection first. If scroll position still jumps, the next refinement should use Win32 `EM_GETFIRSTVISIBLELINE` and `EM_LINESCROLL` to preserve TextBox scroll position exactly.
