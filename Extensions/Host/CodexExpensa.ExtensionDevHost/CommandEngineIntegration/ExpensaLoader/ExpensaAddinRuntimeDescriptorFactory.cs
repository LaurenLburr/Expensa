namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExpensaLoader;

public static class ExpensaAddinRuntimeDescriptorFactory
{
    public static ExpensaAddinRuntimeDescriptor Create(
        ExpensaAddinLoaderKind kind)
    {
        return kind switch
        {
            ExpensaAddinLoaderKind.Websites => new ExpensaAddinRuntimeDescriptor
            {
                Kind = kind,
                AddinName = "WebsitesAddin",
                AssemblyFileName = "WebsitesAddin.dll",
                SmokeRunnerTypeName = "WebsitesAddin.WebsiteLoadRuntimeSmokeRunner",
                RequestTypeName = "WebsitesAddin.WebsiteLoadRequest",
                DefaultCommandName = "Websites.LoadTree"
            },

            ExpensaAddinLoaderKind.Budgets => new ExpensaAddinRuntimeDescriptor
            {
                Kind = kind,
                AddinName = "BudgetsAddin",
                AssemblyFileName = "BudgetsAddin.dll",
                SmokeRunnerTypeName = "BudgetsAddin.BudgetLoadRuntimeSmokeRunner",
                RequestTypeName = "BudgetsAddin.BudgetLoadRequest",
                DefaultCommandName = "Budgets.LoadTree"
            },

            ExpensaAddinLoaderKind.Payees => new ExpensaAddinRuntimeDescriptor
            {
                Kind = kind,
                AddinName = "PayeesAddin",
                AssemblyFileName = "PayeesAddin.dll",
                SmokeRunnerTypeName = "PayeesAddin.PayeeLoadRuntimeSmokeRunner",
                RequestTypeName = "PayeesAddin.PayeeLoadRequest",
                DefaultCommandName = "Payees.LoadTree"
            },

            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unsupported Expensa add-in loader kind.")
        };
    }
}
