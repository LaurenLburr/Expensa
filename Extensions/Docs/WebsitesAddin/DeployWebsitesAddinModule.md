# Deploy WebsitesAddin Module

The Websites Runtime Explorer loads `WebsitesAddin.dll` dynamically.

That means the module must be copied into the DevHost runtime output folder:

```text
Extensions\Host\CodexExpensa.ExtensionDevHost\bin\Debug\net8.0-windows\Modules\WebsitesAddin
```

## Debug deployment

Run:

```text
Extensions\Scripts\Deploy_WebsitesAddin_Module.cmd
```

## Release deployment

Run:

```text
Extensions\Scripts\Deploy_WebsitesAddin_Module_Release.cmd
```

## Why this exists

The host project should not directly reference `WebsitesAddin`.

The correct boundary is:

```text
Host
  loads module DLL at runtime

WebsitesAddin
  owns command implementation
```

This keeps module isolation intact.
