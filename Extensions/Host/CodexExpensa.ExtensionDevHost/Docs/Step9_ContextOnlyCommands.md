# Step 9 – Convert commands to context-only execution

## Project

`CodexExpensa.ExtensionDevHost`

## Goal

Remove the old command execution contract:

```csharp
void Execute(Form owner);
```

Commands now run through:

```csharp
void Execute(ICommandContext context);
```

## Files included

```text
Commands/Abstractions/ICommandContext.cs
Commands/Abstractions/CommandContextExtensions.cs
Commands/Runtime/DefaultCommandContext.cs
Commands/Services/ICommandUiService.cs
Commands/Services/WinFormsCommandUiService.cs
Commands/IExtMgrCommand.cs
Commands/ExtMgrCommandBase.cs
Commands/CommandRegistry.cs
Commands/ExitApplicationCommand.cs
Commands/MenuSpacerCommand.cs
Commands/NewAddinProjectCommand.cs
Commands/NewProjectSpaceCommand.cs
Commands/OpenAiApiKeyCommand.cs
Commands/OpenCommandCatalogCommand.cs
Commands/OpenQueryCatalogCommand.cs
```

## Test steps

1. Rebuild the solution.
2. Run `CodexExpensa.ExtensionDevHost`.
3. Test each menu command:
   - `Project Space → New Project Space`
   - `Project → New Add-in Project`
   - `Project → OpenAI API Key`
   - `Tools → Command Catalog`
   - `Tools → Query Catalog`
   - `File → Exit`

## Expected result

Everything should behave the same as before, except command execution is now context-only internally.

## Important

This Extension Manager does not currently have a TreeView, so `SelectedNode` remains unused.
