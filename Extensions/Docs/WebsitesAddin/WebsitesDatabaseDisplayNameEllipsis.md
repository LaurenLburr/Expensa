# Websites Database Display Name Ellipsis

The database name label now uses explicit text shortening instead of relying only on `LinkLabel.AutoEllipsis`.

Why:

```text
websitesaddin.db
```

is short enough to fit, so WinForms will not automatically ellipsize it.

The UI now deliberately displays a shortened database name while keeping the full database path in the textbox.
