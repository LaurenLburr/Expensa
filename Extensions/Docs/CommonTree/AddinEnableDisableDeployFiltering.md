# Add-in Enable/Disable and Deploy Filtering

Add-in project nodes now have a right-click context menu:

```text
Enable Add-in
Disable Add-in
```

Disabled add-ins show as:

```text
BudgetsAddin (Disabled)
```

and use gray text.

The Deploy menu includes only enabled add-ins:

```text
Deploy
    Deploy All Enabled Add-ins to Expensa
    ----------------------------
    Deploy WebsitesAddin to Expensa
```

Enable/disable state is stored in `MainFormSettings`, avoiding a registration database schema change.
