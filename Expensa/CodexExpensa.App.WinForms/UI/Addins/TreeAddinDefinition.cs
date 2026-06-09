namespace CodexExpensa.App.WinForms.UI.Addins;

public sealed class TreeAddinDefinition
{
    public required string AddinName { get; init; }

    public required string ModuleFolderName { get; init; }

    public required string AssemblyFileName { get; init; }

    public required string SmokeRunnerTypeName { get; init; }

    public required string RequestTypeName { get; init; }

    public required TreeAddinKind Kind { get; init; }

    public static IReadOnlyList<TreeAddinDefinition> DefaultTreeAddins()
    {
        return
        [
            new TreeAddinDefinition
            {
                AddinName = "WebsitesAddin",
                ModuleFolderName = "WebsitesAddin",
                AssemblyFileName = "WebsitesAddin.dll",
                SmokeRunnerTypeName = "WebsitesAddin.WebsiteLoadRuntimeSmokeRunner",
                RequestTypeName = "WebsitesAddin.WebsiteLoadRequest",
                Kind = TreeAddinKind.Websites
            },
            new TreeAddinDefinition
            {
                AddinName = "BudgetsAddin",
                ModuleFolderName = "BudgetsAddin",
                AssemblyFileName = "BudgetsAddin.dll",
                SmokeRunnerTypeName = "BudgetsAddin.BudgetLoadRuntimeSmokeRunner",
                RequestTypeName = "BudgetsAddin.BudgetLoadRequest",
                Kind = TreeAddinKind.Budgets
            }
        ];
    }
}
