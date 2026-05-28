namespace Codex.CommandEngine.Abstractions;

public sealed record AiProviderDescriptor(
    string ProviderKey,
    string DisplayName,
    string ProviderKind,
    string Description);
