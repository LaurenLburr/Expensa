# Payees Database Page Fix

This replaces the fake Payees database page that inherited the generic fallback panel.

## What changed

`PayeesDatabasePanelForm` is now a real WinForms form with a designer file.

It shows:

```text
Database path
Payee table name
Payee row count
Payee grid
Status text
```

## Tables supported

```text
Payee
Payees
```

## Columns supported

For ID:

```text
PayeeId
Id
PayeeId derived from table name
```

For name:

```text
PayeeName
Name
DisplayName
```

For active status:

```text
IsActive
```

If `IsActive` does not exist, the page treats rows as active.

## Note

This only fixes the Extension Manager Payees database panel. It does not change PayeesAddin runtime loading.
