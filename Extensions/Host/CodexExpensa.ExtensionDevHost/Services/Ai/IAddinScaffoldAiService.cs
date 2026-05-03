using CodexExpensa.ExtensionDevHost.Models;

namespace CodexExpensa.ExtensionDevHost.Services.Ai;

public interface IAddinScaffoldAiService
{
    Task<AddinScaffoldAiResult> GenerateAsync(AddinScaffoldAiRequest request, CancellationToken cancellationToken = default);
}
