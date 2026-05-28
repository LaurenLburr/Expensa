using Codex.CommandEngine.Core;

namespace WebsitesAddin;

public sealed class WebsitesAddinCommandMetadataProvider : ICommandMetadataProvider
{
    public IReadOnlyList<CommandMetadataRecord> GetCommandMetadata()
    {
        return
        [
            new CommandMetadataRecord
            {
                CommandName = WebsitesAddinSmokeCommandHandler.RegisteredCommandName,
                DisplayName = "Websites Add-in Smoke Test",
                Category = "Websites",
                Description = "Verifies that the Websites add-in can register metadata and execute through CommandEngine.",
                ParameterTemplateJson = """
                {
                  "message": "Hello from WebsitesAddin",
                  "includeDiagnostics": true
                }
                """,
                Notes = "The current smoke command does not require parameters, but this template proves the metadata/template path."
            }
        ];
    }
}
