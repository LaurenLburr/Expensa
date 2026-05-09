using System.Threading;
using System.Threading.Tasks;

namespace CodexExpensa.ExtensionDevHost.Commands.Services;

public interface ICommandAiService
{
    Task<string> GenerateScaffoldAsync(string prompt, CancellationToken cancellationToken = default);
}
