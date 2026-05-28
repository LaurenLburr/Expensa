# Design Conversation AI Service Fix

## Problem

The Design Spec conversation was using `OpenAiCommandAiService.GenerateScaffoldAsync(...)`.

That forced responses into scaffold-summary format, producing text like:

```text
AI scaffold suggestion:
Project: AiGeneratedAddin
Assembly: ...
```

That is wrong for the Design Spec workflow.

## Fix

Added:

```text
Services/Ai/OpenAiDesignConversationService.cs
```

Updated:

```text
UI/DesignSpecConversationForm.cs
```

The Design Spec conversation now uses the new design conversation service for:

- `Send to AI`
- `Update Design Spec`

## Result

The AI is no longer constrained to scaffold output. It can now respond as a real design/spec assistant and generate proper architecture/spec content.
