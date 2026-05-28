# Codex.CommandEngine Developer Tech Guide

## Purpose

This guide explains how developers should use the `Codex.CommandEngine` command system from ExtensionMgr and from individual add-ins.

The current system is designed around a simple idea:

```text
Extension/add-in code exposes command providers.
CommandEngine discovers providers.
Providers register command handlers.
The runtime lists and executes commands through one stable boundary.
ExtensionMgr owns hosting, diagnostics, reload, and UI.
```

This keeps add-ins small and keeps ExtensionMgr from knowing every command class directly.

## Current Architecture

### Main Projects

| Project | Purpose |
|---|---|
| `Codex.CommandEngine.Abstractions` | Shared lower-level abstractions, if needed by the engine. |
| `Codex.CommandEngine.Core` | Runtime, handlers, workflows, provider interfaces, diagnostics, host lifecycle. |
| `CodexExpensa.ExtensionDevHost.CommandEngineIntegration` | ExtensionMgr bridge: dashboard, runtime manager, discovery, manifest models. |
| `CodexExpensa.ExtensionDevHost.CommandEngineIntegration.TestHarness` | Temporary launcher for testing the dashboard without the full Host UI. |
| Add-in projects, such as `WebsitesAddin` | Provide real runtime command providers and handlers. |

### Runtime Flow

```text
ExtensionRuntimeDashboardForm
    -> ExtensionRuntimeDashboardController
        -> ExtensionRuntimeManager
            -> ExtensionRuntimeHost
                -> ExtensionCommandRuntime
                    -> CommandEngineRuntime
                        -> CommandDispatcher
                            -> ICommandHandler
```

That looks like a lot, but each layer has a specific job:

| Layer | Job |
|---|---|
| Dashboard form | WinForms UI for testing and diagnostics. |
| Dashboard controller | Keeps form code thin. |
| ExtensionRuntimeManager | ExtensionMgr-facing service. Starts, reloads, stops, discovers providers. |
| ExtensionRuntimeHost | Keeps one runtime alive and tracks state. |
| ExtensionCommandRuntime | ExtensionMgr-friendly wrapper around the core runtime. |
| CommandEngineRuntime | Core runtime facade. Registers and executes commands/workflows. |
| CommandDispatcher | Finds the correct handler by command name. |
| ICommandHandler | Actual command behavior. |

## Key Concepts

### Command Handler

A command handler performs one command.

```csharp
public sealed class WebsitesAddinSmokeCommandHandler : ICommandHandler
{
    public const string RegisteredCommandName = "websites.smoke.test";

    public string CommandName => RegisteredCommandName;

    public Task<CommandExecutionResult> ExecuteAsync(
        CommandExecutionRequest request,
        ICommandExecutionContext context)
    {
        context.WriteLog("Websites add-in smoke command executed.");

        return Task.FromResult(
            CommandExecutionResult.Succeeded(
                request.CommandName,
                request.CorrelationId,
                "Websites add-in smoke test completed.",
                """
                {"ok":true,"source":"WebsitesAddin"}
                """));
    }
}
```

Rules:

- `CommandName` must be unique across all loaded providers.
- Use stable lowercase dotted names, such as `websites.smoke.test`.
- Do not put UI code directly inside handlers unless the command is explicitly UI-only.
- Prefer returning structured JSON in `OutputJson`.
- Use the provided `CorrelationId` so execution history can connect related steps later.

### Command Registration

A registration describes a command handler and its metadata.

```csharp
new RuntimeCommandRegistration
{
    Handler = new WebsitesAddinSmokeCommandHandler(),
    DisplayName = "Websites Add-in Smoke Test",
    Description = "Verifies that the Websites add-in can execute through CommandEngine.",
    Category = "Websites",
    Version = 1,
    IsEnabled = true
}
```

The metadata is what the dashboard and future ExtensionMgr UI show.

### Command Provider

A provider exposes command registrations.

```csharp
public sealed class WebsitesAddinCommandProvider : IRuntimeCommandRegistrationProvider
{
    public IReadOnlyList<RuntimeCommandRegistration> GetRegistrations()
    {
        return
        [
            new RuntimeCommandRegistration
            {
                Handler = new WebsitesAddinSmokeCommandHandler(),
                DisplayName = "Websites Add-in Smoke Test",
                Description = "Verifies that the Websites add-in can execute through CommandEngine.",
                Category = "Websites",
                Version = 1,
                IsEnabled = true
            }
        ];
    }
}
```

Rules:

- Provider classes must be `public`.
- Provider classes must implement `IRuntimeCommandRegistrationProvider`.
- Provider classes must have a public parameterless constructor for discovery.
- Providers should not perform expensive work in their constructor.
- Providers should return registrations quickly.

### Runtime Host

`ExtensionRuntimeHost` owns the runtime lifecycle.

Supported states:

| State | Meaning |
|---|---|
| `NotStarted` | Runtime has not been created yet. |
| `Started` | Runtime started without bootstrap errors. |
| `Failed` | Runtime started with errors or failed during bootstrap. |
| `Stopped` | Runtime was intentionally stopped. |

Main operations:

```csharp
host.Start(request);
host.Reload(request);
host.Stop();
host.GetSnapshot();
host.ExecuteCommandAsync(request);
```

`Reload` replaces the old runtime with a new one. This is important for add-in development.

### Runtime Manager

`ExtensionRuntimeManager` is the service ExtensionMgr should talk to.

Typical usage:

```csharp
ExtensionRuntimeManager manager = new();

manager.Start(SmokeRuntimeProviderFactory.Create());

ExtensionRuntimeManagerSnapshot snapshot = manager.GetSnapshot();

CommandExecutionResult result = await manager.ExecuteCommandAsync("extension.smoke.test");
```

Discovery usage:

```csharp
manager.StartFromFolder(folderPath, recursive: false);
manager.ReloadFromFolder(folderPath, recursive: false);
manager.StartFromLoadedAssemblies();
manager.ReloadFromLoadedAssemblies();
```

## Discovery

### Loaded Assembly Discovery

`StartFromLoadedAssemblies()` scans assemblies already loaded into the current AppDomain.

Use this when:

- you know the target add-in assembly is already loaded
- you are testing built-in providers
- you want quick diagnostics

### Folder Discovery

`StartFromFolder(folderPath, recursive)` scans DLLs in a folder.

Use this when:

- testing an add-in output folder
- loading external add-ins
- verifying a compiled module DLL

Example folder for `WebsitesAddin`:

```text
D:\Git\CodexExpensa\Extensions\Modules\WebsitesAddin\bin\Debug\net8.0-windows
```

The dashboard should find `WebsitesAddin.dll` and show:

```text
websites.smoke.test
```

## Dashboard Verification

Open the Host app and navigate to:

```text
Tools -> CommandEngine Runtime
```

### Smoke Runtime Test

Click:

```text
Start Smoke
```

Expected command:

```text
extension.smoke.test
```

Select it and click:

```text
Execute Selected
```

Expected result:

```text
Succeeded: Extension runtime smoke test completed.
```

### WebsitesAddin Discovery Test

Build `WebsitesAddin`, then point the folder textbox to:

```text
D:\Git\CodexExpensa\Extensions\Modules\WebsitesAddin\bin\Debug\net8.0-windows
```

Click:

```text
Start Folder
```

Expected commands:

```text
extension.smoke.test
websites.smoke.test
```

Depending on the current discovery setup, the smoke command may appear from the integration assembly and the websites command from the add-in DLL.

Select:

```text
websites.smoke.test
```

Click:

```text
Execute Selected
```

Expected result:

```text
Succeeded: Websites add-in smoke test completed.
```

## Extension Manifest System

The manifest system is the metadata layer for future safer discovery.

Example manifest:

```json
{
  "extensionId": "WebsitesAddin",
  "displayName": "Websites Add-in",
  "version": "1.0.0",
  "assemblyFile": "WebsitesAddin.dll",
  "providerType": "WebsitesAddin.WebsitesAddinCommandProvider",
  "minimumHostVersion": "1.0.0",
  "enabled": true,
  "description": "CommandEngine-enabled Websites add-in."
}
```

Current manifest classes:

| Type | Purpose |
|---|---|
| `ExtensionManifest` | Manifest model. |
| `ExtensionManifestLoader` | Reads JSON from disk. |
| `ExtensionManifestValidator` | Checks required fields and version format. |
| `ExtensionManifestWriter` | Writes manifest JSON. |
| `ExtensionManifestTemplate` | Creates starter manifests. |

The next expected step is to connect manifests to discovery:

```text
find extension.json
 -> validate manifest
 -> load listed assembly
 -> instantiate listed provider type
 -> register commands
```

## Recommended Command Naming

Use lowercase dotted names:

```text
area.feature.action
```

Examples:

```text
websites.smoke.test
websites.open.manager
websites.import.bookmarks
budget.recalculate.current
extension.smoke.test
```

Avoid:

```text
Run
Test
Open
WebsiteThing
```

Those names will collide later. Future-you will be annoyed. Future-you is already busy.

## Recommended Provider Layout

For an add-in project:

```text
WebsitesAddin
|-- WebsitesAddin.csproj
|-- WebsitesAddinExtension.cs
|-- WebsitesAddinCommandProvider.cs
|-- WebsitesAddinSmokeCommandHandler.cs
|-- extension.json        (future/manifest-based discovery)
```

The provider owns registration. The handlers own behavior.

## Testing Strategy

### Unit-Level Tests

For a handler:

```csharp
[Fact]
public async Task SmokeCommand_ExecutesSuccessfully()
{
    WebsitesAddinSmokeCommandHandler handler = new();

    CommandExecutionResult result =
        await handler.ExecuteAsync(
            new CommandExecutionRequest
            {
                CommandName = WebsitesAddinSmokeCommandHandler.RegisteredCommandName,
                CorrelationId = "websites-001"
            },
            new CommandExecutionContext());

    Assert.Equal(CommandExecutionStatus.Succeeded, result.Status);
}
```

For a provider:

```csharp
[Fact]
public void GetRegistrations_ReturnsSmokeCommandRegistration()
{
    WebsitesAddinCommandProvider provider = new();

    IReadOnlyList<RuntimeCommandRegistration> registrations =
        provider.GetRegistrations();

    Assert.Single(registrations);
}
```

### Integration-Level Tests

Use the runtime manager:

```csharp
ExtensionRuntimeManager manager = new();
manager.Start([new WebsitesAddinCommandProvider()]);

CommandExecutionResult result =
    await manager.ExecuteCommandAsync("websites.smoke.test");
```

### Manual UI Tests

Use the dashboard:

1. Open `Tools -> CommandEngine Runtime`.
2. Start smoke runtime.
3. Execute smoke command.
4. Start folder discovery against an add-in output folder.
5. Execute discovered add-in command.
6. Reload folder.
7. Stop runtime.

## Error Handling and Diagnostics

### Duplicate Command Names

If two providers register the same command name, bootstrap diagnostics should report an error.

Fix by renaming one command.

### Provider Has No Parameterless Constructor

Discovery requires a public parameterless constructor.

Bad:

```csharp
public MyProvider(IService service)
```

Good:

```csharp
public MyProvider()
```

If a provider needs services later, add a proper host-provided provider factory. Do not hide service construction in reflection magic.

### DLL Not Found

Use the project output folder, not the project source folder.

Correct:

```text
bin\Debug\net8.0-windows
```

Wrong:

```text
Extensions\Modules\WebsitesAddin
```

### Command Does Not Execute

Check:

- command is listed in the dashboard
- `IsEnabled` is true
- command name matches exactly
- provider returns the handler registration
- handler does not throw

## Development Rules

- No SQL changes are required for basic command registration/discovery.
- Keep commands small and deterministic.
- Keep UI out of command handlers unless the command is explicitly UI-bound.
- Add tests with every new provider/handler.
- Prefer integration-style tests over mocks.
- Do not use repository schema mutation in CommandEngine data code.
- Use the dashboard diagnostics before guessing.

## Current Milestone Status

Completed:

- core runtime facade
- command registration metadata
- provider model
- ExtensionMgr runtime manager
- runtime host lifecycle
- dashboard UI
- provider discovery
- WebsitesAddin discovered command
- manifest model/loader/validator

Next recommended implementation:

```text
Manifest-based discovery
```

That means discovery should prefer manifest files over brute-force DLL scanning.

## Manifest-Based Discovery Target Flow

```text
Folder
 -> find extension.json files
 -> load and validate each manifest
 -> skip disabled manifests
 -> check host version compatibility
 -> load AssemblyFile
 -> resolve ProviderType
 -> instantiate provider
 -> register provider commands
 -> show manifest and provider diagnostics
```

This is the next step toward a real extension platform.
