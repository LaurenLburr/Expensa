namespace Codex.CommandEngine.Abstractions;

public sealed class CommandHandlerDescriptor
{
    public required CommandDefinition Definition { get; init; }

    public required ICommandHandler Handler { get; init; }
}
