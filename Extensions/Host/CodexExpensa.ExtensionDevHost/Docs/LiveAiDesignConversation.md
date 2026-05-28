# Live AI Design Conversation

## Scope

Adds a real `Send to AI` workflow to the Design Spec conversation workspace.

## Changed Files

```text
UI/DesignSpecConversationForm.cs
UI/DesignSpecConversationForm.Designer.cs
UI/DesignSpecConversationForm.resx
```

## Behavior

When viewing:

```text
Add-in Projects -> [Project] -> Design Spec
```

the conversation workspace now has a:

```text
Send to AI
```

button.

The workflow:

1. Appends the user's new message to `DesignConversation.md`.
2. Saves the current design spec and conversation.
3. Builds a prompt from:
   - project name
   - design spec path
   - conversation path
   - current design spec markdown
   - current conversation markdown
   - latest user message
4. Sends the prompt through the existing OpenAI key/service path.
5. Appends the AI response to the conversation.
6. Saves again.

## Notes

This uses the existing:

```text
OpenAiApiKeyStore
OpenAiCommandAiService
```

service path, so it uses the same key configured in:

```text
Project -> OpenAI API Key
```
