# Expensa Website Tree Bridge

This slice extracts the reusable host-side bridge for loading Websites into a real WinForms `TreeView`.

## Main service

```csharp
IHostWebsiteTreeLoader
HostWebsiteTreeLoader
```

## Use from a WinForms form

```csharp
private readonly IHostWebsiteTreeLoader _websiteTreeLoader = new HostWebsiteTreeLoader();

private async Task LoadWebsitesAsync()
{
    await _websiteTreeLoader.LoadIntoTreeViewAsync(
        websitesTreeView,
        new HostWebsiteTreeLoadOptions
        {
            SearchText = "",
            IncludeDisabled = false,
            MaximumRows = 500,
            ExpandAll = true
        });
}
```

## Selection details

```csharp
detailsTextBox.Text =
    HostWebsiteTreeSelectionFormatter.FormatSelectedNode(websitesTreeView);
```

## Boundary

The bridge lives in the host layer and owns WinForms rendering.

`WebsitesAddin` remains UI-neutral.
