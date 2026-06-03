# Website Smoke Runner Result Mapping

The Extension Manager was throwing:

```text
Website smoke runner did not return CommandExecutionResult.
```

The likely cause is .NET assembly-load-context type identity.

The add-in returns a type named:

```text
Codex.CommandEngine.Core.CommandExecutionResult
```

but if the add-in loaded its own copy of the command engine assembly, the host-side `as CommandExecutionResult` check fails.

The invoker now maps the result by property names instead of requiring exact runtime type identity.

Mapped properties:

```text
CorrelationId
Status
Message
OutputJson
```

This avoids the false failure while keeping the add-in loaded with dependency resolution for SQLite.
