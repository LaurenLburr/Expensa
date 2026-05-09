# Step 9 Compile Fix

## Project

`CodexExpensa.ExtensionDevHost`

## Fix

The compile error means one file is still calling:

```csharp
Execute(owner)
```

after the interface changed to:

```csharp
Execute(ICommandContext context)
```

This package replaces the two most likely files:

```text
Commands/CommandRegistry.cs
Commands/OpenCommandCatalogCommand.cs
```

## Verify

Search the project for:

```text
.Execute(
```

The command invocation should be:

```csharp
command.Execute(context);
```

There should be no command call like:

```csharp
command.Execute(owner);
```

Also search for:

```text
Execute(ui.Owner)
```

That should be gone.
