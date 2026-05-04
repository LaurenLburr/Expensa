# Extension Dev Host Refactor Step 2 - OpenAI API Key Panel

## Scope

This step converts the Project > OpenAI API Key tree node into an embedded main-panel form.

## Changes

- Added `OpenAiApiKeyPanelForm`.
- `Project > OpenAI API Key` now loads into the main content panel when selected.
- The old `OpenAiApiKeyCommand` remains available for menu compatibility.
- Added a reusable embedded panel path through `ShowCommandNode(...)`.

## New files

```text
UI/OpenAiApiKeyPanelForm.cs
UI/OpenAiApiKeyPanelForm.Designer.cs
UI/OpenAiApiKeyPanelForm.resx
```

## Modified file

```text
Extensions/MainForm.cs
```

## Notes

The `Test Key` button currently performs local validation only. The old `AI Test` command still exists. A later step should move the real AI test behavior into this panel.
