# Payees Loader Test and Single-Click Fix

This fixes two problems:

```text
Payees is missing from the Expensa Add-in Loader Test dropdown
The Expensa Add-in Loader Test still needs double-click
```

## Patch method

This slice includes a PowerShell patch script instead of whole-file replacements because `MainForm.cs` and the loader test form have been changing quickly.

## Apply

From the repository root:

```powershell
.\Apply_PayeesLoaderTestAndSingleClickFix.ps1
```

or:

```powershell
.\Apply_PayeesLoaderTestAndSingleClickFix.ps1 -Root D:\Git\CodexExpensa
```

## Expected result

```text
Tools
└── Expensa Add-in Loader Test
```

opens on single-click/selection.

The loader dropdown should include:

```text
Websites
Budgets
Payees
```
