# Make Active Copies to Runtime Database

`Make Active` now copies the selected database into the canonical runtime database path:

```text
%APPDATA%\Expensa\Extensions\Runtime\WebsitesAddin\websitesaddin.db
```

The active settings file points to that canonical runtime database.

This gives the add-in a stable runtime database path while still allowing copied databases to be selected from the UI.
