# Payees Loader Test and Database Layout Full Fix

This slice uses full replacement files from the current uploaded project.

## Fixes

```text
Payees appears in the Expensa Add-in Loader Test dropdown
Payees maps to ExpensaAddinLoaderKind.Payees
Load All includes Payees
Expensa Add-in Loader Test opens on single-click/selection
Payees Database page uses horizontal split
```

## Notes

The uploaded code already had some of this work partially present. This zip normalizes the full files so the project state is consistent.

## Expected UI

### Expensa Add-in Loader Test

```text
Add-in dropdown:
    Websites
    Budgets
    Payees
```

`Load All` should load:

```text
Websites
Budgets
Payees
```

### Payees Database

The split should be horizontal:

```text
Details / summary
-----------------
Payees grid
```
