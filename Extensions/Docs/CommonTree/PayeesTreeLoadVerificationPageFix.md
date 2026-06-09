# Payees Tree Load Verification Page Fix

This makes the Payees test page work like the Websites test page pattern.

## What changed

`PayeesTreeLoadVerificationFormCommonTree` now:

```text
Uses ExpensaAddinTreeViewLoaderService
Uses ExpensaAddinLoaderKind.Payees
Accepts database path
Supports search text
Supports include inactive
Supports max rows
Supports expand all
Renders the returned tree into a TreeView
Shows loader result status and JSON
Shows selected node details
Auto-loads when the form opens
```

## Expected behavior

In Extension Manager:

```text
Add-in Projects
└── PayeesAddin
    └── Test
```

opens a working Payees tree verification page.

## Requirements

`PayeesAddin` must be built/deployed where the loader expects it.

The current loader still uses descriptor-based add-in loading, so `ExpensaAddinRuntimeDescriptorFactory` must include:

```text
ExpensaAddinLoaderKind.Payees
```
