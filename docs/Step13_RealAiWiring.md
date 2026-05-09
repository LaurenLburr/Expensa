# Step 13 – Real AI wiring

## Project

`CodexExpensa.ExtensionDevHost`

## Files added or updated

```text
Commands/Services/ICommandAiService.cs
Commands/Services/FakeCommandAiService.cs
Commands/Services/OpenAiCommandAiService.cs
Commands/AiTestCommand.cs
Services/Ai/IAddinScaffoldAiService.cs
Services/Ai/OpenAiAddinScaffoldAiService.cs
```

## Required manual change

Update:

```text
Commands/CommandRegistry.cs
```

Inside `Invoke(...)`, replace:

```csharp
services.Register<ICommandAiService>(new FakeCommandAiService());
```

with:

```csharp
services.Register<ICommandAiService>(
    new OpenAiCommandAiService(new OpenAiApiKeyStore()));
```

Make sure these usings exist at the top:

```csharp
using CodexExpensa.ExtensionDevHost.Commands.Services;
using CodexExpensa.ExtensionDevHost.Services.Ai;
```

## Test

1. Rebuild.
2. Run `CodexExpensa.ExtensionDevHost`.
3. Make sure an API key is saved through:

```text
Project → OpenAI API Key
```

4. Run:

```text
Tools → AI Test
```

## Expected result

The AI Test command should call the real OpenAI service and display scaffold suggestions.

## Notes

This keeps the command layer isolated from OpenAI details by using `OpenAiCommandAiService` as an adapter.
