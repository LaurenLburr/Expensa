# Payees Loader Test and Database Layout Fix

## Fixes

- Payees selection in Expensa Add-in Loader Test now maps to `ExpensaAddinLoaderKind.Payees`.
- Load All now loads Websites, Budgets, and Payees.
- Payees database page uses a horizontal split: details on top, grid on bottom.

## Expected UI

```text
Expensa Add-in Loader Test
    Websites
    Budgets
    Payees
```

```text
Payees Database
    Details / summary
    -----------------
    Payees grid
```
