using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class ExtensionRuntimeDashboardMenuFieldTests
{
    [Fact]
    public void ExtensionManagerMenuFieldsFile_DefinesMissingDesignerFields()
    {
        string repositoryRoot =
            FindRepositoryRoot();

        string filePath =
            Path.Combine(
                repositoryRoot,
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "ExtensionRuntimeDashboardForm.ExtensionManagerMenuFields.cs");

        Assert.True(File.Exists(filePath), $"File was not found: {filePath}");

        string text =
            File.ReadAllText(filePath);

        Assert.Contains("extensionManagerMenuItem", text);
        Assert.Contains("openExtensionManagerAddinTestSurfaceMenuItem", text);
    }

    private static string FindRepositoryRoot()
    {
        return TestPathHelper.ExtensionsRoot;
    }
}
