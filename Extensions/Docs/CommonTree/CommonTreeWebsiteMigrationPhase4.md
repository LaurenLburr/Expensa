# CommonTree Website Migration Phase 4

This slice migrates Websites onto the shared CommonTree framework without removing the older Websites-specific path yet.

## Added

```text
WebsiteTreeProvider : AddinTreeProviderBase<WebsiteTreePayload>
CommonWebsiteTreeContributionLoader
WebsitesTreeLoadVerificationFormCommonTree
```

## Why this is separate

The existing `WebsitesTreeLoadVerificationForm` still exists.

This lets the CommonTree version be tested before replacing the older Websites-specific tree path.

## Next phase

After this builds and runs:

1. Point the Extension Manager Websites Tree Load Test node at `WebsitesTreeLoadVerificationFormCommonTree`.
2. Confirm the website context/tag work still makes sense under CommonTree.
3. Retire duplicate Website-specific renderer/provider code.
