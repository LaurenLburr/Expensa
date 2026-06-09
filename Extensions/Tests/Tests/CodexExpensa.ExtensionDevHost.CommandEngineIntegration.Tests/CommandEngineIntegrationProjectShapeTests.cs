using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class CommandEngineIntegrationProjectShapeTests
{
    [Fact]
    public void Project_ReferencesCommandEngineCore()
    {
        string projectText =
            ReadHostProjectFile();

        Assert.Contains("Codex.CommandEngine.Core", projectText, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SmokeRuntimeProviderFactory_UsesCommandEngineSmokeProvider()
    {
        string factoryText =
            ReadHostIntegrationFile("SmokeRuntimeProviderFactory.cs");

        Assert.Contains("CommandEngineSmoke", factoryText, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Smoke", factoryText, StringComparison.OrdinalIgnoreCase);
    }

    private static string ReadHostProjectFile()
    {
        string extensionsRoot =
            FindExtensionsRoot();

        string[] candidateFiles =
        [
            Path.Combine(
                extensionsRoot,
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CodexExpensa.ExtensionDevHost.csproj"),

            Path.Combine(
                extensionsRoot,
                "Host",
                "CodexExpensa.ExtensionDevHost.CommandEngineIntegration",
                "CodexExpensa.ExtensionDevHost.CommandEngineIntegration.csproj")
        ];

        return ReadFirstExistingFile(candidateFiles);
    }

    private static string ReadHostIntegrationFile(
        string fileName)
    {
        string extensionsRoot =
            FindExtensionsRoot();

        string[] candidateFiles =
        [
            Path.Combine(
                extensionsRoot,
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                fileName),

            Path.Combine(
                extensionsRoot,
                "Host",
                "CodexExpensa.ExtensionDevHost.CommandEngineIntegration",
                fileName)
        ];

        return ReadFirstExistingFile(candidateFiles);
    }

    private static string ReadFirstExistingFile(
        IReadOnlyList<string> candidateFiles)
    {
        foreach (string candidateFile in candidateFiles)
        {
            if (File.Exists(candidateFile))
            {
                return File.ReadAllText(candidateFile);
            }
        }

        throw new FileNotFoundException(
            "Could not find any expected project-shape file. Tried:" +
            Environment.NewLine +
            string.Join(Environment.NewLine, candidateFiles));
    }

    private static string FindExtensionsRoot()
    {
        DirectoryInfo? directory =
            new(AppContext.BaseDirectory);

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

        DirectoryInfo? currentDirectory =
            new(Directory.GetCurrentDirectory());

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
