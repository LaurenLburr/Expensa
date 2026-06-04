using Codex.CommandEngine.Core;

namespace BudgetsAddin;

public sealed class BudgetsAddinCommandMetadataProvider : ICommandMetadataProvider
{
    public IReadOnlyList<CommandMetadataRecord> GetCommandMetadata()
    {
        return
        [
            new CommandMetadataRecord
            {
                CommandName = "Budgets.LoadTree",
                DisplayName = "Load Budgets Tree",
                Description = "Loads budget year/month nodes from the Expensa database.",
                Category = "Budgets"
            }
        ];
    }
}
