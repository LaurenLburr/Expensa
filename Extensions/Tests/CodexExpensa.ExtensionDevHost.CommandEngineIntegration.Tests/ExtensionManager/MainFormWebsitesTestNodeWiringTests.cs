using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class MainFormWebsitesTestNodeWiringTests
{
    [Fact]
    public void MainForm_WebsitesTestNodeOpensTreeLoadVerificationForm()
    {
        string repositoryRoot =
            FindRepositoryRoot();

        string mainFormPath =
            Path.Combine(
                repositoryRoot,
                "Extensions",
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "MainForm.cs");

        Assert.True(File.Exists(mainFormPath), $"File was not found: {mainFormPath}");

        string text =
            File.ReadAllText(mainFormPath);

        Assert.Contains("WebsitesTreeLoadVerificationForm", text);
        Assert.Contains("ShowEmbeddedForm(new WebsitesTreeLoadVerificationForm())", text);
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "Extensions")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException();
    }
}
