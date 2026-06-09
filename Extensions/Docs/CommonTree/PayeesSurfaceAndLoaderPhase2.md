# Payees Surface and Loader Phase 2

Fixes the missing Payees wiring after importing `PayeesAddin`.

## Database/Test Nodes

Payees now has convention forms:

```text
PayeesDatabasePanelForm
PayeesTreeLoadVerificationFormCommonTree
```

They inherit generic add-in panels, so `MainForm` still does not know Payees specifically.

## Loader Convention

Adds runtime convention logic so imported add-ins can derive:

```text
PayeesAddin.PayeeLoadRuntimeSmokeRunner
PayeesAddin.PayeeLoadRequest
```

from:

```text
PayeesAddin
```

## Direction

MainForm knows only:

```text
add-in
```

not:

```text
Websites
Budgets
Payees
```
