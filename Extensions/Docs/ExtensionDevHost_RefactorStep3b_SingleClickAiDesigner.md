# Extension Dev Host Refactor Step 3b - Single Click AI Designer

## Problem

`Project -> AI Add-in Designer` still required a double-click.

## Cause

`ShowCommandNode(...)` handled `Project.OpenAiApiKey`, but did not yet route `Tools.AiAddinDesigner` on tree selection.

## Fix

Added this case to `ShowCommandNode(...)`:

```csharp
case "Tools.AiAddinDesigner":
    ShowEmbeddedForm(new AiAddinDesignerPanelForm());
    break;
```

Now the AI Add-in Designer loads on single-click selection.
