# Copy From Expensa Prod UI

The Websites Database panel now includes:

```text
Copy from Expensa Prod
```

It copies website data from:

```text
%LOCALAPPDATA%\CodexExpensa\db\codexexpensa.db
```

into:

```text
Extensions\Modules\WebsitesAddin\DevDatabase\websites.current.db
```

and:

```text
%APPDATA%\Expensa\Extensions\Runtime\WebsitesAddin\websites.current.db
```

It also writes:

```text
%APPDATA%\Expensa\Extensions\Runtime\WebsitesAddin\active-runtime-db.txt
```
