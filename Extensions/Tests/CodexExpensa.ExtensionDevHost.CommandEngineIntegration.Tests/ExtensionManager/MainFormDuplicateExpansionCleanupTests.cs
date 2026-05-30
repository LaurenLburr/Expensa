using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class MainFormDuplicateExpansionCleanupTests
{
    [Fact]
    public void MainForm_HasSinglePersistentNavigationExpansionImplementation()
    {
        string text = ReadMainForm();

        Assert.Equal(
            1,
            CountOccurrences(
                text,
                "private IReadOnlySet<string> _lastNavigationExpandedNodeNames"));

        Assert.Equal(
            1,
            CountOccurrences(
                text,
                "private IReadOnlySet<string> CaptureExpandedNavigationNodeNames()"));

        Assert.Equal(
            1,
            CountOccurrences(
                text,
                "private void RestoreExpandedNavigationNodeNames("));

        Assert.Equal(
            1,
            CountOccurrences(
                text,
                "private static string CreateSafeTreeNodeName("));

        Assert.Contains("NavigationExpandedNodeNames", text);
        Assert.Contains("SaveNavigationExpandedNodeNames", text);
    }

    private static int CountOccurrences(
        string text,
        string value)
    {
        int count = 0;
        int index = 0;

        while ((index = text.IndexOf(value, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += value.Length;
        }

        return count;
    }

    private static string ReadMainForm()
    {
        string repositoryRoot = FindRepositoryRoot();

        string mainFormPath = Path.Combine(
            repositoryRoot,
            "Extensions",
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "MainForm.cs");

        Assert.True(File.Exists(mainFormPath), $"File was not found: {mainFormPath}");

        return File.ReadAllText(mainFormPath);
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

        throw new DirectoryNotFoundException("Could not find repository root containing Extensions folder.");
    }
}
