# Extension Dev Host Refactor Step 11b - Rendered Design Conversation

## Problem

The Design Spec conversation workspace showed the design spec as raw Markdown only.

## Fix

The left side of `DesignSpecConversationForm` now has:

```text
Rendered Design Spec
Markdown Source
```

The rendered preview updates as the Markdown source changes.

## Conversation Layout

The right side now explicitly flows as:

```text
Ongoing Conversation
New Message
```

The new message box is directly underneath the main conversation text box.

## Files

```text
UI/DesignSpecConversationForm.cs
UI/DesignSpecConversationForm.Designer.cs
UI/DesignSpecConversationForm.resx
```
