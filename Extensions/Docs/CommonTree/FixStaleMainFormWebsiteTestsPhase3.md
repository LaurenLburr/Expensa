# Fix Stale MainForm Website Tests Phase 3

Two MainForm wiring tests were still asserting the legacy exact form call:

```csharp
ShowEmbeddedForm(new WebsitesTreeLoadVerificationForm())
```

The code has moved toward CommonTree, so the current valid target is:

```csharp
ShowEmbeddedForm(new WebsitesTreeLoadVerificationFormCommonTree())
```

The updated tests accept either the CommonTree form or the legacy form while the migration is still in progress.

Once the old Websites-specific path is removed, these tests can be tightened to require only the CommonTree form.
