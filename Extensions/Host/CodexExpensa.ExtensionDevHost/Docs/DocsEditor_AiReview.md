# Docs Editor AI Review

## Project

`CodexExpensa.ExtensionDevHost`

## Files

```text
MainForm.cs
UI/DocsEditorForm.cs
Services/Ai/OpenAiDocumentReviewService.cs
```

## Changes

- Selecting a Docs TreeView node now loads the docs editor into the right-side content panel.
- The docs editor includes an `AI Review` button.
- `AI Review` sends the selected document text to OpenAI for spelling, grammar, clarity, and professional Markdown formatting.
- The reviewed text replaces the editor text but does not save automatically.
- The user must click `Save` after reviewing the AI output.

## Notes

The document review service uses the OpenAI Responses API endpoint already used elsewhere in this project.
