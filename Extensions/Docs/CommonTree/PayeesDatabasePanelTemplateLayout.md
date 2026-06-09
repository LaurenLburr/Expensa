# Payees Database Panel Template Layout

This updates `PayeesDatabasePanelForm` to follow the new `DatabasePanelTemplate` layout.

## Template controls used

```text
panel1
linkUpdate_from_Dev
linkUpdate_from_Prod
linkDb_filename
labelDatabase_file_Name
label1
labelAdd_in_Name
splitContainer1
text_Data_
gridDataView
statusStrip1
labelNumRows
```

## Layout

The panel now uses the same top section and horizontal split as `DatabasePanelTemplate`:

```text
Header / database links
-----------------------
Details text
-----------------------
Data grid + status strip
```

## Behavior

The Payees page still loads:

```text
Payee
Payees
```

and displays rows in `gridDataView`.

The status strip shows the loaded row count.
