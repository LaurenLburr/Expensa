using Xunit;

namespace CodexExpensa.App.WinForms.Tests.UI.Addins;

public sealed class AddinProjectReferenceTests
{
    [Fact]
    public void WinFormsApp_DoesNotReferenceAddinProjects()
    {
        string projectText =
            File.ReadAllText(
                Path.Combine(
                    TestRepositoryPath.FindRepositoryRoot(),
                    "Expensa",
                    "CodexExpensa.App.WinForms",
                    "CodexExpensa.App.WinForms.csproj"));

        Assert.DoesNotContain(
            @"..\..\Extensions\Modules\BudgetsAddin\BudgetsAddin.csproj",
            projectText,
            StringComparison.Ordinal);
        Assert.DoesNotContain(
            @"..\..\Extensions\Modules\PayeesAddin\PayeesAddin.csproj",
            projectText,
            StringComparison.Ordinal);
        Assert.DoesNotContain(
            @"..\..\Extensions\Modules\WebsitesAddin\WebsitesAddin.csproj",
            projectText,
            StringComparison.Ordinal);
    }
}
