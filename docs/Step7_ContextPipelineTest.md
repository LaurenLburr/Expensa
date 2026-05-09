# Step 7 – Context Pipeline Test

## Project

`CodexExpensa.ExtensionDevHost`

## Files changed

```text
Commands/Abstractions/ICommandContext.cs
Commands/Abstractions/CommandContextExtensions.cs
Commands/Abstractions/CommandContextNodeExtensions.cs
Commands/Runtime/DefaultCommandContext.cs
Commands/Services/ICommandUiService.cs
Commands/Services/WinFormsCommandUiService.cs
Commands/IExtMgrCommand.cs
Commands/ExtMgrCommandBase.cs
Commands/CommandRegistry.cs
Commands/OpenCommandCatalogCommand.cs
```

## What this tests

This step proves the command call path is now:

```text
Menu click
→ CommandRegistry.Invoke(...)
→ DefaultCommandContext
→ command.Execute(ICommandContext context)
→ ICommandUiService
→ original command behavior
```

## Important note

The current `MainForm` does not expose a TreeView to `CommandRegistry`, so this package does **not** test selected tree nodes yet.

This test confirms that command context and command services are working.

## How to test

1. Apply the files.
2. Rebuild the solution.
3. Run `CodexExpensa.ExtensionDevHost`.
4. Open:

```text
Tools → Command Catalog
```

5. Expected:
   - First popup says `Context pipeline is active.`
   - Then the Command Catalog opens.

## Remove test popup later

After the test passes, remove the `MessageBox.Show(...)` block from:

```text
Commands/OpenCommandCatalogCommand.cs
```
