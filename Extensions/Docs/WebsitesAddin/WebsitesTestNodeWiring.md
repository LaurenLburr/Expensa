# Websites Test Node Wiring

The main Extension Dev Host tree now routes:

```text
Add-in Projects
  WebsitesAddin
    Test
```

to:

```csharp
WebsitesTreeLoadVerificationForm
```

This gives a direct visual test of:

```text
active runtime DB -> websites.load -> HostWebsiteLoadResult -> TreeView
```
