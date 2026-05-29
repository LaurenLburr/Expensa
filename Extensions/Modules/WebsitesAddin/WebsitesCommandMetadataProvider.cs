using Codex.CommandEngine.Core;

namespace WebsitesAddin;

public sealed class WebsitesCommandMetadataProvider : ICommandMetadataProvider
{
    public IReadOnlyList<CommandMetadataRecord> GetCommandMetadata()
    {
        return
        [
            new CommandMetadataRecord
            {
                CommandName = "websites.load",
                DisplayName = "Load Websites",
                Category = "Websites",
                Description = "Loads websites into a UI-neutral tree model for Expensa.",
                ParameterTemplateJson = """
                {
                  "searchText": "",
                  "includeDisabled": false,
                  "maximumRows": 500
                }
                """,
                Notes = "This is the first bridge command for loading Websites from Expensa through CommandEngine."
            }
        ];
    }
}
