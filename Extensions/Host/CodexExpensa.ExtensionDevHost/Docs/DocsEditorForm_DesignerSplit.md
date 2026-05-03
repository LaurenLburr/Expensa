# DocsEditorForm Designer Split

The docs editor now uses the standard WinForms designer pattern:

```text
UI/DocsEditorForm.cs
UI/DocsEditorForm.Designer.cs
UI/DocsEditorForm.resx
```

Controls and layout are in the designer file. Runtime behavior remains in `DocsEditorForm.cs`.
