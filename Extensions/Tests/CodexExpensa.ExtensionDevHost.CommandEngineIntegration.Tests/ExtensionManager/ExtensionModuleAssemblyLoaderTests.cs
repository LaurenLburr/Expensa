using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class ExtensionModuleAssemblyLoaderTests
{
    [Fact]
    public void LoadAssemblies_WhenFolderDoesNotExist_ReturnsEmpty()
    {
        ExtensionModuleAssemblyLoader loader = new();

        IReadOnlyList<ExtensionModuleAssemblyLoadResult> results =
            loader.LoadAssemblies(
                Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));

        Assert.Empty(results);
    }

    [Fact]
    public void LoadAssemblies_WhenFolderContainsCurrentTestAssembly_LoadsAssembly()
    {
        string folder =
            Path.Combine(
                Path.GetTempPath(),
                "ExtensionModuleAssemblyLoaderTests",
                Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(folder);

        string currentAssemblyPath =
            typeof(ExtensionModuleAssemblyLoaderTests).Assembly.Location;

        string targetAssemblyPath =
            Path.Combine(folder, Path.GetFileName(currentAssemblyPath));

        File.Copy(currentAssemblyPath, targetAssemblyPath);

        ExtensionModuleAssemblyLoader loader = new();

        IReadOnlyList<ExtensionModuleAssemblyLoadResult> results =
            loader.LoadAssemblies(folder);

        Assert.NotEmpty(results);
        Assert.Contains(results, result => result.Succeeded);
    }
}
