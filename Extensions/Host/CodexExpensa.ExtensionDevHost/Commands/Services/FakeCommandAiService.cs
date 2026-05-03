using System.Threading;
using System.Threading.Tasks;

namespace CodexExpensa.ExtensionDevHost.Commands.Services;

public sealed class FakeCommandAiService : ICommandAiService
{
    public Task<string> GenerateScaffoldAsync(string prompt, CancellationToken cancellationToken = default)
    {
        string result = $"[AI GENERATED]{System.Environment.NewLine}Prompt: {prompt}";
        return Task.FromResult(result);
    }
}
