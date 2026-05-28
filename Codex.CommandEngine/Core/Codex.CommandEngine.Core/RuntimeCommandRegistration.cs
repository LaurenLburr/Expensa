namespace Codex.CommandEngine.Core;

public sealed class RuntimeCommandRegistration
{
    public required ICommandHandler Handler { get; init; }

    public string DisplayName { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public int Version { get; init; } = 1;

    public bool IsEnabled { get; init; } = true;

    public RuntimeCommandDescriptor ToDescriptor()
    {
        ArgumentNullException.ThrowIfNull(Handler);
        ArgumentException.ThrowIfNullOrWhiteSpace(Handler.CommandName);

        return new RuntimeCommandDescriptor
        {
            CommandName = Handler.CommandName,
            DisplayName = string.IsNullOrWhiteSpace(DisplayName)
                ? Handler.CommandName
                : DisplayName,
            Description = Description,
            Category = Category,
            Version = Version,
            IsEnabled = IsEnabled
        };
    }
}
