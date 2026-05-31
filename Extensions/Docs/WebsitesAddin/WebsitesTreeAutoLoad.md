# Websites Tree Auto Load

`WebsitesTreeLoadVerificationForm` now loads automatically the first time it is shown.

Expected flow:

```text
Add-in Projects
  WebsitesAddin
    Test
```

Opening the Test node should immediately run the Websites tree load path.

The Load button remains available for manual reloads.
