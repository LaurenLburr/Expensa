using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class CommandExecutionQueueDesignerWiringTests
{
    [Fact]
    public void DashboardDesigner_IncludesQueueMenuItems()
    {
        string repositoryRoot = FindRepositoryRoot();

        string designerPath =
            Path.Combine(
                repositoryRoot,
                "Extensions",
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "ExtensionRuntimeDashboardForm.Designer.cs");

        Assert.True(File.Exists(designerPath), $"File was not found: {designerPath}");

        string text = File.ReadAllText(designerPath);

        Assert.Contains("queueMenuItem", text, StringComparison.Ordinal);
        Assert.Contains("&Queue", text, StringComparison.Ordinal);
        Assert.Contains("QueueSelectedCommand", text, StringComparison.Ordinal);
        Assert.Contains("ShowExecutionQueue", text, StringComparison.Ordinal);
        Assert.Contains("CancelSelectedQueueItem", text, StringComparison.Ordinal);
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

        DirectoryInfo? currentDirectory = new(Directory.GetCurrentDirectory());

        while (currentDirectory is not null)
        {
            if (Directory.Exists(Path.Combine(currentDirectory.FullName, "Extensions")))
            {
                return currentDirectory.FullName;
            }

            currentDirectory = currentDirectory.Parent;
        }

        throw new DirectoryNotFoundException("Could not find repository root containing Extensions folder.");
    }
}
