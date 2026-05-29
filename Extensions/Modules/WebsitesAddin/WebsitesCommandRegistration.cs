using Codex.CommandEngine.Abstractions;

namespace WebsitesAddin;

public static class WebsitesCommandRegistration
{
    public static IReadOnlyList<ICommandHandler> CreateHandlers()
    {
        return
        [
            new WebsiteLoadCommandHandler()
        ];
    }
}
