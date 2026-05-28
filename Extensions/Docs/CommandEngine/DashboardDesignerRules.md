# Dashboard Designer Rules

`ExtensionRuntimeDashboardForm` must stay in the standard WinForms designer pattern.

## Required Structure

```text
ExtensionRuntimeDashboardForm.cs
ExtensionRuntimeDashboardForm.Designer.cs
ExtensionRuntimeDashboardForm.resx
```

## Responsibilities

`ExtensionRuntimeDashboardForm.cs` owns behavior:

- constructor dependency setup
- event handlers
- runtime operations
- manifest operations
- command execution
- history operations
- diagnostics display logic

`ExtensionRuntimeDashboardForm.Designer.cs` owns UI setup:

- `InitializeComponent()`
- controls
- layout panels
- menu strip
- menu items
- list view
- diagnostics textbox
- basic event hookups

`ExtensionRuntimeDashboardForm.resx` owns designer resource metadata.

## Avoid in Code-Behind

Do not put these in `ExtensionRuntimeDashboardForm.cs`:

```csharp
new MenuStrip()
new ToolStripMenuItem()
private void BuildMenu(...)
private void BuildLayout(...)
```

Those belong in the designer file.

## Why

Keeping the dashboard designer-friendly matters because:

- layout can be inspected visually
- controls can be renamed safely
- future UI changes are easier
- runtime behavior is easier to review
- the form does not become WinForms spaghetti with reflection croutons
