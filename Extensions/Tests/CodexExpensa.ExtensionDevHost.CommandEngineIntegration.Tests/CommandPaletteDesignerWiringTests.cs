using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class CommandPaletteDesignerWiringTests
{
    [Fact]
    public void DashboardDesigner_IncludesCommandPaletteMenuItem()
    {
        string repositoryRoot = FindExtensionsRoot();

        string designerPath =
            Path.Combine(
                repositoryRoot,
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "ExtensionRuntimeDashboardForm.Designer.cs");

        Assert.True(File.Exists(designerPath), $"File was not found: {designerPath}");

        string text = File.ReadAllText(designerPath);

        Assert.Contains("commandPaletteMenuItem", text, StringComparison.Ordinal);
        Assert.Contains("Command &Palette", text, StringComparison.Ordinal);
        Assert.Contains("ShowCommandPalette", text, StringComparison.Ordinal);
        Assert.Contains("Keys.Control | Keys.P", text, StringComparison.Ordinal);
    }

    private static string FindRepositoryRoot()
    {
        return TestPathHelper.ExtensionsRoot;
    }

    private static string FindExtensionsRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Extensions.sln")))
            {
                return directory.FullName;
            }

            if (Directory.Exists(Path.Combine(directory.FullName, "Host"))
                && Directory.Exists(Path.Combine(directory.FullName, "Modules"))
                && Directory.Exists(Path.Combine(directory.FullName, "Tests")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        DirectoryInfo? currentDirectory = new(Directory.GetCurrentDirectory());

        while (currentDirectory is not null)
        {
            if (File.Exists(Path.Combine(currentDirectory.FullName, "Extensions.sln")))
            {
                return currentDirectory.FullName;
            }

            if (Directory.Exists(Path.Combine(currentDirectory.FullName, "Host"))
                && Directory.Exists(Path.Combine(currentDirectory.FullName, "Modules"))
                && Directory.Exists(Path.Combine(currentDirectory.FullName, "Tests")))
            {
                return currentDirectory.FullName;
            }

            currentDirectory = currentDirectory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not find Extensions solution root containing Extensions.sln or Host/Modules/Tests folders.");
    }
}
