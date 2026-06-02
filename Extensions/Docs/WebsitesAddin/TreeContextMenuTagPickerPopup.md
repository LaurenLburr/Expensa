# Websites Tree Tag Picker Popup

The underscore menu item now opens a small popup form containing the tag picker instead of embedding a control inside an already-open context menu.

This is more reliable in WinForms because the popup is a normal form with a normal `TextBox` and `ListBox`.

## Flow

```text
Right-click tree
  -> click ________
  -> popup opens near click location
  -> type tag text
  -> list filters
  -> Enter/double-click selects tag node
```
