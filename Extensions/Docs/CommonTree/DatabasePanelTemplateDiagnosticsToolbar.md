# DatabasePanelTemplate Diagnostics Toolbar

This adds the diagnostics toolbar to the shared `DatabasePanelTemplate`.

## Why

`WebsitesDatabasePanelForm.DiagnosticsLayout.cs` still expects:

```csharp
diagnosticsToolStrip
```

After converting Websites to inherit from the shared template, that control must live on the shared template instead of inside the Websites designer.

## Added to template

```csharp
protected ToolStrip diagnosticsToolStrip;
```

and helper members:

```csharp
DiagnosticsToolStrip
ClearDiagnosticsToolStrip()
AddDiagnosticsToolStripItem(...)
```

## Result

Websites can keep its diagnostics partial class, and future database forms can share the same toolbar instead of cloning it.
