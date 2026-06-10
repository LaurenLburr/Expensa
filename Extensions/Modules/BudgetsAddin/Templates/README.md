# BudgetsAddin Templates

This folder is reserved for manually designed WinForms template forms used by the Budgets add-in.

## Purpose

Place hand-designed forms here when the Budgets add-in needs template-specific UI surfaces.

Examples may include:

- Budget template editor forms
- Template selection forms
- Template copy/preview forms
- Template maintenance forms

## Designer convention

Use the normal WinForms designer pattern:

- `SomeTemplateForm.cs` for behavior and event logic
- `SomeTemplateForm.Designer.cs` for controls, layout, and `InitializeComponent()`
- `SomeTemplateForm.resx` for designer resources

Do not runtime-build designer-style layouts in the main `.cs` file unless there is a specific reason.

## Database impact

This folder does not introduce any database or schema changes. It is only a source-code location for future manually designed forms.
