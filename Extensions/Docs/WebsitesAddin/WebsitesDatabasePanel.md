# Websites Database Panel

This panel is the first real UI target for the add-in project `Database` node.

It loads Websites data through the existing module runtime path:

```text
HostWebsiteRuntimeModuleInvoker
  -> websites.load
  -> WebsiteLoadResult JSON
  -> flat row display
```

It does not directly open SQLite from the host.
