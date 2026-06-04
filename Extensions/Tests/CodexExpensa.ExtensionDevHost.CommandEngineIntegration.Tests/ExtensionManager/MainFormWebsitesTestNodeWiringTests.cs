using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class MainFormWebsitesTestNodeWiringTests
{
    [Fact]
    public void MainForm_WebsitesTestNodeOpensTreeLoadVerificationForm()
    {
        string text =
            ReadFile(
                "Extensions",
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "MainForm.cs");

        Assert.Contains("IsWebsitesAddinProject", text);
        Assert.Contains("OpenAddinTestNode", text);

        Assert.True(
            text.Contains("ShowEmbeddedForm(new WebsitesTreeLoadVerificationFormCommonTree())") ||
            text.Contains("ShowEmbeddedForm(new WebsitesTreeLoadVerificationForm())"),
            "Websites test node should open the CommonTree verification form or the legacy tree-load verification form.");
    }

    private static string ReadFile(params string[] parts)
    {
        string repositoryRoot =
            FindRepositoryRoot();

        string path =
            Path.Combine([repositoryRoot, .. parts]);

        Assert.True(
            File.Exists(path),
            $"File was not found: {path}");

        return File.ReadAllText(path);
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory =
            new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "Extensions")))
            {
                return directory.FullName;
            }

            directory =
                directory.Parent;
        }

        throw new DirectoryNotFoundException();
    }
}
