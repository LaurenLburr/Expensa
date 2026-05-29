# Websites Runtime Explorer

The Websites Runtime Explorer is the first DevHost UI surface where the command engine visibly drives a UI control.

## Flow

```text
Load Websites button
  -> WebsiteLoadRuntimeCommandHandler
  -> CommandExecutionResult.OutputJson
  -> HostWebsiteLoadExecutionResultAdapter
  -> TreeView render
```

## Important boundary

`WebsitesAddin` remains UI-neutral.

The WinForms rendering lives in:

```text
Extensions\Host\CodexExpensa.ExtensionDevHost\CommandEngineIntegration\Websites
```

## Usage

Open from the dashboard menu:

```text
Websites -> Open Websites Runtime Explorer
```

Then click:

```text
Load Websites
```

Double-clicking a website node with a URL opens that URL.
