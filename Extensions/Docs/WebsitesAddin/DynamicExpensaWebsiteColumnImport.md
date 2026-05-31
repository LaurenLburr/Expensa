# Dynamic Expensa Website Column Import

`Copy from Expensa Prod` now inspects the source table columns before building the import SQL.

This prevents errors like:

```text
SQLite Error 1: no such column: DisplayName
```

The importer checks candidate column names and only references columns that actually exist.

## Display name candidates

```text
DisplayName
Name
WebsiteName
Description
Title
```

## URL candidates

```text
Url
URL
WebsiteUrl
WebsiteURL
Link
Address
```

Missing values fall back to safe defaults.
