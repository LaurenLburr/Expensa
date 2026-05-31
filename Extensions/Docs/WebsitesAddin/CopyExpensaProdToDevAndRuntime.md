# Copy Expensa Production Data to Websites Dev and Runtime

Use this script when you want the Websites test tree to immediately show Expensa production website data.

## Run

```text
Extensions\Modules\WebsitesAddin\Scripts\Copy_Expensa_Prod_To_Websites_Dev_And_Runtime.cmd
```

## It updates both

```text
Extensions\Modules\WebsitesAddin\DevDatabase\websites.dev.db
```

and

```text
%APPDATA%\Expensa\Extensions\Runtime\WebsitesAddin\websitesaddin.db
```

It also writes:

```text
%APPDATA%\Expensa\Extensions\Runtime\WebsitesAddin\active-runtime-db.txt
```

That is why this script affects the visible Test tree immediately.
