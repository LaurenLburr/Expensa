# Step 4 – Add a real UI service for commands

## Goal

Remove this temporary hack:

```csharp
var owner = context.Services as Form ?? Application.OpenForms[0];
```

Replace it with a small command UI service.

## New files

Add these files to the project:

```text
Commands/Services/ICommandUiService.cs
Commands/Services/WinFormsCommandUiService.cs
```

## Update MainForm context creation

Where you build `DefaultCommandContext`, change:

```csharp
Services = null
```

to:

```csharp
Services = new WinFormsCommandUiService(this)
```

Add this using if needed:

```csharp
using CodexExpensa.ExtensionDevHost.Commands.Services;
```

## Update OpenCommandCatalogCommand

Replace the temporary owner lookup:

```csharp
var owner = context.Services as Form ?? Application.OpenForms[0];

var form = new CommandCatalogForm(_registry, _configPath, _rebuildMenu);
form.Show(owner);
```

with:

```csharp
if (context.Services is not ICommandUiService ui)
    throw new InvalidOperationException("Command UI service is not available.");

var form = new CommandCatalogForm(_registry, _configPath, _rebuildMenu);
ui.Show(form);
```

Add this using if needed:

```csharp
using CodexExpensa.ExtensionDevHost.Commands.Services;
```

## Test

1. Rebuild solution.
2. Run `CodexExpensa.ExtensionDevHost`.
3. Open Command Catalog.
4. Confirm it opens normally.
5. Confirm existing commands still work.

## Notes

This still uses the existing `Services` property on `ICommandContext`.

Later, we can rename that to something cleaner or replace it with typed service access.
