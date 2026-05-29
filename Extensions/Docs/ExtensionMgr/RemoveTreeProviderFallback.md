# Remove Tree Provider Fallback

This slice removes the hardcoded Websites tree provider fallback.

If no module providers are found, no providers are returned.

The Websites module now provides convention metadata:

```csharp
WebsitesTreeContributionProvider
```

This is the first step toward:

```text
drop DLL into Modules
restart host
new tree node appears
```
