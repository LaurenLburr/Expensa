using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Budgets;

public sealed class HostBudgetRuntimeDatabaseSelectionServiceTests
{
    [Fact]
    public void ResolveCanonicalDatabasePath_ReplacesLegacyCurrentDbWithBudgetsCurrentDb()
    {
        string folder =
            Path.Combine(
                Path.GetTempPath(),
                "CodexExpensa-tests",
                Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(folder);

        try
        {
            string legacyPath = Path.Combine(folder, "current.db");
            string canonicalPath = Path.Combine(folder, "budgets.current.db");

            File.WriteAllText(legacyPath, "legacy");
            File.WriteAllText(canonicalPath, "budgets");

            string actual =
                HostBudgetRuntimeDatabaseSelectionService
                    .ResolveCanonicalDatabasePath(legacyPath);

            Assert.Equal(canonicalPath, actual);
        }
        finally
        {
            Directory.Delete(folder, recursive: true);
        }
    }

    [Fact]
    public void ResolveCanonicalDatabasePath_KeepsBudgetsCurrentDb()
    {
        string path =
            Path.Combine("DevDatabase", "budgets.current.db");

        string actual =
            HostBudgetRuntimeDatabaseSelectionService
                .ResolveCanonicalDatabasePath(path);

        Assert.Equal(path, actual);
    }
}
