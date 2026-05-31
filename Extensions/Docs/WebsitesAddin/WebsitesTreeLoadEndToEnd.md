# Websites Tree Load End-to-End

This slice makes the Websites tree loading path explicit:

```text
HostWebsiteRuntimeModuleInvoker
  -> websites.load
  -> CommandExecutionResult
  -> HostWebsiteLoadResult
  -> HostWebsiteTreeViewRenderer
  -> TreeView
```

It also adds a direct verification form:

```text
WebsitesTreeLoadVerificationForm
```

This form exists to make the loading path easy to see and debug before folding the behavior back into the broader Extension Manager test surface.
