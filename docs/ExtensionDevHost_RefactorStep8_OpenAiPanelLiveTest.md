# Extension Dev Host Refactor Step 8 - OpenAI Panel Live Test

## Scope

Moves the remaining AI Test behavior into the embedded OpenAI API Key panel.

## Behavior

`Project -> OpenAI API Key` now supports a real live AI test from the embedded panel.

The Test Key button now:

1. Validates that a key is entered.
2. Saves the key and key name.
3. Runs the same AI service path used by the old AI Test command.
4. Shows the result or error.
5. Displays wait cursor while the request is running.

## Modified

```text
UI/OpenAiApiKeyPanelForm.cs
```

## Notes

The old `AiTestCommand` can remain registered for compatibility, but the intended user workflow is now through:

```text
Project -> OpenAI API Key -> Test Key
```
