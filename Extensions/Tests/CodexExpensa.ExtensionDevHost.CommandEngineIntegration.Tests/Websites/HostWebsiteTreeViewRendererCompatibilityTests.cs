using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteTreeViewRendererCompatibilityTests
{
    [Fact]
    public void Renderer_KeepsLoadResultOverloadWithoutExpandAllArgument()
    {
        string text =
            ReadFile(
                "Extensions",
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "HostWebsiteTreeViewRenderer.cs");

        Assert.Contains("Render(\r\n        TreeView treeView,\r\n        HostWebsiteLoadResult result)", NormalizeLineEndings(text));
        Assert.Contains("expandAll: false", text);
    }

    private static string NormalizeLineEndings(
        string text)
    {
        return text
            .Replace("\r\n", "\n")
            .Replace("\n", "\r\n");
    }

    private static string ReadFile(
        params string[] parts)
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
