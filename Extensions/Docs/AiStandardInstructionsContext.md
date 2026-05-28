# AI Standard Instructions Context

## Scope

Adds standard instruction/context loading for Design Spec AI conversations.

## New File

```text
Services/AiInstructionContextService.cs
```

## Included Context

When the Design Spec conversation calls AI, the prompt now includes available content from:

```text
%AppData%\Expensa\Extensions\WorkspaceDocs\GeneralCodingSpec.md
%AppData%\Expensa\Extensions\WorkspaceDocs\GeneralCodingRules.md
%AppData%\Expensa\Extensions\WorkspaceDocs\AiAddinDesignWorkflow.md
%AppData%\Expensa\Extensions\WorkspaceDocs\AddinDesignSpecTemplate.md
%AppData%\Expensa\Extensions\WorkspaceDocs\ExpensaIntegrationSpecTemplate.md
```

It also includes project-local docs beside the design spec:

```text
CatchUp.md
Expensa_Integration_Design_Spec.md
```

## Updated Behavior

The AI now receives:
- standard coding rules
- standard design workflow
- templates
- project catch-up context
- project integration context
- current design spec
- current conversation
- latest user message

## Files

```text
Services/AiInstructionContextService.cs
UI/DesignSpecConversationForm.cs
UI/DesignSpecConversationForm.Designer.cs
UI/DesignSpecConversationForm.resx
```
