using System.Text.Json;
using Xunit;

namespace WebsitesAddin.Tests;

public sealed class WebsitesAddinManifestTests
{
    [Fact]
    public void ExtensionManifest_FileExistsInProjectFolder()
    {
        string projectFolder =
            FindProjectFolder();

        string manifestPath =
            Path.Combine(projectFolder, "extension.json");

        Assert.True(
            File.Exists(manifestPath),
            $"Expected manifest file was not found: {manifestPath}");
    }

    [Fact]
    public void ExtensionManifest_HasRequiredCommandEngineFields()
    {
        string projectFolder =
            FindProjectFolder();

        string manifestPath =
            Path.Combine(projectFolder, "extension.json");

        using JsonDocument document =
            JsonDocument.Parse(File.ReadAllText(manifestPath));

        JsonElement root =
            document.RootElement;

        Assert.Equal("WebsitesAddin", root.GetProperty("extensionId").GetString());
        Assert.Equal("WebsitesAddin.dll", root.GetProperty("assemblyFile").GetString());
        Assert.Equal("WebsitesAddin.WebsitesAddinCommandProvider", root.GetProperty("providerType").GetString());
        Assert.True(root.GetProperty("enabled").GetBoolean());
    }

    private static string FindProjectFolder()
    {
        DirectoryInfo? directory =
            new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            string manifestPath =
                Path.Combine(directory.FullName, "extension.json");

            string projectPath =
                Path.Combine(directory.FullName, "WebsitesAddin.csproj");

            if (File.Exists(manifestPath) && File.Exists(projectPath))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        string fallback =
            Path.GetFullPath(
                Path.Combine(
                    AppContext.BaseDirectory,
                    "..",
                    "..",
                    "..",
                    "..",
                    "..",
                    "Modules",
                    "WebsitesAddin"));

        return fallback;
    }
}
