# Step 9 CommandExecutor Fix

## Project

`CodexExpensa.ExtensionDevHost`

## File

```text
Commands/Runtime/CommandExecutor.cs
```

## Problem

The old bridge version still called:

```csharp
command.Execute(_owner);
```

That worked while commands still used:

```csharp
Execute(Form owner)
```

But Step 9 converted commands to:

```csharp
Execute(ICommandContext context)
```

So `CommandExecutor` must now call:

```csharp
command.Execute(context);
```

## Test

1. Apply this file.
2. Rebuild.
3. Run the host.
4. Test the menu commands.
