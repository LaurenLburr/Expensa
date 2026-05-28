# Update Design Spec From Conversation

## Problem

The Design Spec workspace saved the conversation, but the Design Spec itself did not include what was discussed in the conversation.

## Change

Added a new button:

```text
Update Design Spec
```

## Behavior

The button asks AI to rewrite the Design Spec using:

- current Design Spec markdown
- current Design Conversation markdown
- project name

Then it replaces the Design Spec Markdown Source with the updated markdown, saves the file, and refreshes the rendered preview.

## Files

```text
UI/DesignSpecConversationForm.cs
UI/DesignSpecConversationForm.Designer.cs
UI/DesignSpecConversationForm.resx
```

## Notes

This keeps conversation and spec separate by default, but gives an explicit controlled action to apply conversation decisions into the spec.
