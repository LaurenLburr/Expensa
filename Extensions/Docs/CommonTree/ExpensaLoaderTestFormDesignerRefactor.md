# Expensa Loader Test Form Designer Refactor

`ExpensaAddinLoaderTestForm` now follows the standard WinForms designer pattern.

## Split

```text
ExpensaAddinLoaderTestForm.cs
    Behavior, event handlers, async loading logic

ExpensaAddinLoaderTestForm.Designer.cs
    Controls, layout, event wiring, InitializeComponent()

ExpensaAddinLoaderTestForm.resx
    Resource placeholder
```

This keeps the form consistent with the project rule that WinForms layout belongs in `.Designer.cs`.
