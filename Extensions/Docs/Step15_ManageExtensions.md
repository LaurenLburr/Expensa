# Step 15 – Manage Extensions UI

## Why existing extensions did not show

The Dev Host had commands to create/register extensions, but no screen that listed the registrations.

This step adds:

```text
Tools -> Manage Extensions
```

## Project

`CodexExpensa.ExtensionDevHost`

## Files included

```text
Services/ExtensionProjectRegistrationStore.cs
UI/ManageExtensionsForm.cs
Commands/OpenManageExtensionsCommand.cs
MainForm.cs
```

## What the UI shows

- ProjectName
- AssemblyName
- RelativeBinPath
- IsEnabled
- SortOrder
- Manager DB path

## Actions

- Refresh
- Open Folder
- Unregister
- Delete Files + Unregister

## Important limitation

This shows registered/scaffolded extensions. It does not mean dynamic runtime DLL loading is complete yet.

The next milestone is:

```text
registered extension -> build -> copy DLL to load cache -> load extension -> show in host
```

## Test

1. Rebuild.
2. Run `CodexExpensa.ExtensionDevHost`.
3. Open:

```text
Tools -> Manage Extensions
```

4. Confirm generated/scaffolded extensions are listed.
