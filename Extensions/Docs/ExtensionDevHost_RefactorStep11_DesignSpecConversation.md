# Extension Dev Host Refactor Step 11 - Design Spec Conversation

## Scope

Selecting an add-in project's `Design Spec` now opens a dedicated design conversation workspace.

## Behavior

When this node is selected:

```text
Add-in Projects
└── [Project]
    └── Design Spec
```

the host loads:

```text
DesignSpecConversationForm
```

instead of the plain markdown editor.

## Workspace

The form displays:

- project name
- design spec path
- conversation path
- editable design spec
- ongoing conversation history
- new message box

## Conversation Storage

The ongoing design conversation is saved beside the design spec:

```text
Modules\[ProjectName]\Docs\DesignConversation.md
```

## Buttons

- Save
- Reload
- Add Message
- Add AI Placeholder
- Open Docs Folder

## Notes

This step adds persistent project-specific design conversation storage.

Live AI response wiring is intentionally left for the next step so the conversation storage and UI workflow are stable first.
