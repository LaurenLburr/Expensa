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
        return TestPathHelper.ExtensionsRoot;
    }
}
