# Step 5 – Add typed command service access

## Goal

Replace repeated casts like this:

```csharp
if (context.Services is not ICommandUiService ui)
    throw new InvalidOperationException("Command UI service is not available.");
```

with this:

```csharp
var ui = context.GetRequiredService<ICommandUiService>();
```

## New file

Add:

```text
Commands/Abstractions/CommandContextExtensions.cs
```

## Update OpenCommandCatalogCommand

Make sure these usings exist:

```csharp
using CodexExpensa.ExtensionDevHost.Commands.Abstractions;
using CodexExpensa.ExtensionDevHost.Commands.Services;
```

Then replace:

```csharp
if (context.Services is not ICommandUiService ui)
    throw new InvalidOperationException("Command UI service is not available.");

var form = new CommandCatalogForm(_registry, _configPath, _rebuildMenu);
ui.Show(form);
```

with:

```csharp
var ui = context.GetRequiredService<ICommandUiService>();

var form = new CommandCatalogForm(_registry, _configPath, _rebuildMenu);
ui.Show(form);
```

## Test

1. Rebuild solution.
2. Run the host.
3. Open Command Catalog.
4. Confirm it opens normally.

## Why this matters

This keeps command code from spreading raw `context.Services as Something` casts everywhere.

Tiny cleanup now. Fewer gremlins later.
