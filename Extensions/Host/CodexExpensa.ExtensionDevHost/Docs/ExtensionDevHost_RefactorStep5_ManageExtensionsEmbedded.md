# Extension Dev Host Refactor Step 5 - Embedded Manage Extensions

## Scope

Converts:

```text
Tools -> Manage Extensions
```

from a popup workflow into an embedded main-panel workspace.

## Behavior

Selecting the tree node now loads `ManageExtensionsForm` into the main content panel.

## Tree refresh

`ManageExtensionsForm` now exposes:

```csharp
RegistrationsChanged
```

`MainForm` subscribes and rebuilds the navigation tree after unregister/delete operations.

## Notes

The old command remains registered for menu compatibility, but tree selection now uses the embedded panel path.

## Files

```text
Extensions/MainForm.cs
UI/ManageExtensionsForm.cs
Services/ExtensionProjectRegistrationStore.cs
Models/ExtensionProjectRegistration.cs
```
