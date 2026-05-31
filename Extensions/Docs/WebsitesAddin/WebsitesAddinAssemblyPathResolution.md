# WebsitesAddin Assembly Path Resolution

The host now searches for `WebsitesAddin.dll` in more places.

## Search locations include

```text
AppContext.BaseDirectory\Modules\WebsitesAddin\WebsitesAddin.dll
CurrentDirectory\Modules\WebsitesAddin\WebsitesAddin.dll
<ancestor>\Modules\WebsitesAddin\WebsitesAddin.dll
<ancestor>\Modules\WebsitesAddin\bin\Debug\net8.0-windows\WebsitesAddin.dll
<ancestor>\Modules\WebsitesAddin\bin\Debug\net8.0\WebsitesAddin.dll
<ancestor>\Modules\WebsitesAddin\bin\Release\net8.0-windows\WebsitesAddin.dll
<ancestor>\Modules\WebsitesAddin\bin\Release\net8.0\WebsitesAddin.dll
```

The failure message now also lists every searched path.
