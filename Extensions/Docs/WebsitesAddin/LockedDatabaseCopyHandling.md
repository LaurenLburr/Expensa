# Locked Database Copy Handling

`Copy from Expensa Prod` now imports into a temporary staging database first.

Then it tries to replace:

```text
DevDatabase\websites.current.db
```

and:

```text
%APPDATA%\Expensa\Extensions\Runtime\WebsitesAddin\websites.current.db
```

If one target is locked, the operation records a warning instead of failing immediately.
