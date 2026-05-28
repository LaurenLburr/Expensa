namespace Codex.CommandEngine.Abstractions;

public interface IAiProvider
{
    AiProviderDescriptor Descriptor { get; }

    Task<AiProviderResponse> ExecuteAsync(AiProviderRequest request, CancellationToken cancellationToken);
}
