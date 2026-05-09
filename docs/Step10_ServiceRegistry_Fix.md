# Step 10 Service Registry Fix

## Project

`CodexExpensa.ExtensionDevHost`

## What this fixes

The first Step 10 package was too aggressive and accidentally replaced `CommandRegistry` with a stripped-down version.

This fix restores:

- `CommandRegistry(ICommandMenuConfigStore? configStore = null)`
- `MenuDefinitionChanged`
- `LoadConfig()`
- JSON metadata behavior
- separator behavior
- menu ordering behavior

It also keeps the new typed service registry.

## Files included

```text
Commands/Abstractions/ICommandServiceProvider.cs
Commands/Runtime/CommandServiceProvider.cs
Commands/Abstractions/ICommandContext.cs
Commands/Runtime/DefaultCommandContext.cs
Commands/Abstractions/CommandContextExtensions.cs
Commands/CommandRegistry.cs
```

## Test

1. Apply this package.
2. Rebuild.
3. Run the host.
4. Test Command Catalog.
5. Test menu config/load behavior.
