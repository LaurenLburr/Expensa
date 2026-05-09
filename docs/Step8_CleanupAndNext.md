# Step 8 – Cleanup + Standard Pattern

## Goal
Remove test popup and lock in the correct pattern for all commands.

## Update File

Project:
CodexExpensa.ExtensionDevHost

File:
Commands/OpenCommandCatalogCommand.cs

## Replace Execute(ICommandContext)

REMOVE the MessageBox test block.

Final version should be:

```csharp
public override void Execute(ICommandContext context)
{
    if (context == null)
        throw new ArgumentNullException(nameof(context));

    var ui = context.GetRequiredService<ICommandUiService>();

    Execute(ui.Owner);
}
```

## Result

- Context pipeline remains active
- Command uses new system
- No debug/test noise

## Pattern for ALL future commands

```csharp
public override void Execute(ICommandContext context)
{
    var ui = context.GetRequiredService<ICommandUiService>();

    // optional: use context later for data
    Execute(ui.Owner);
}
```

## Why this matters

You now have:

- UI access via service (not Form parameter)
- Backward compatibility
- Clean migration path

## Next Step Preview

Next we will:

- Convert CommandRegistry to stop calling Execute(Form)
- Move fully to context-only execution