# Step 14 – Scaffold files and improve API key form

## Project

`CodexExpensa.ExtensionDevHost`

## Files included

```text
Services/Ai/OpenAiApiKeyStore.cs
UI/OpenAiApiKeyForm.cs
Commands/AiScaffoldFilesCommand.cs
MainForm.cs
```

## Changes

### OpenAI API Key form

Adds:

- local Key Name field
- Show/Hide API key button
- saves both key name and API key to the existing local OpenAiSettings.json file

### AI scaffold files command

Adds menu command:

```text
Tools -> AI Scaffold Files
```

This command:

1. Calls the real OpenAI scaffold service.
2. Uses the returned project/assembly/description.
3. Creates scaffold files through `AddinProjectScaffolder`.
4. Registers the generated project using `ExtensionProjectRegistrationStore`.
5. Opens the generated project folder.

## Test

1. Rebuild.
2. Run `CodexExpensa.ExtensionDevHost`.
3. Open:

```text
Project -> OpenAI API Key
```

4. Verify:
   - Key Name is visible/editable
   - API key can be shown/hidden

5. Run:

```text
Tools -> AI Scaffold Files
```

6. Expected:
   - files are created under `Extensions/Modules/[ProjectName]`
   - generated project is registered
   - folder opens after success
