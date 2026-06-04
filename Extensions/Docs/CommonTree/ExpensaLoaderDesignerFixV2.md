# Expensa Loader Designer Fix V2

This refactors `ExpensaAddinLoaderTestForm` into the standard WinForms pattern:

```text
ExpensaAddinLoaderTestForm.cs
ExpensaAddinLoaderTestForm.Designer.cs
ExpensaAddinLoaderTestForm.resx
```

The designer file avoids modern C# constructs and keeps all layout/control creation inside `InitializeComponent`.

Behavior stays in the main `.cs` file.
