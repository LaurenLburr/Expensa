using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class CommandEngineIntegrationProjectShapeTests
{
    [Fact]
    public void CommandEngineIntegration_ProjectFileExists()
    {
        string projectFile = FindCommandEngineIntegrationProjectFile();

        Assert.True(File.Exists(projectFile), $"File was not found: {projectFile}");
    }

    [Fact]
    public void CommandEngineIntegration_ProjectTargetsNet8Windows()
    {
        string projectFile = FindCommandEngineIntegrationProjectFile();
        XDocument document = XDocument.Load(projectFile);

        string? targetFramework =
            document.Descendants("TargetFramework")
                .Select(static element => element.Value.Trim())
                .FirstOrDefault();

        Assert.Equal("net8.0-windows", targetFramework);
    }

    [Fact]
    public void CommandEngineIntegration_ProjectReferencesCommandEngineCore()
    {
        string projectFile = FindCommandEngineIntegrationProjectFile();
        string projectText = File.ReadAllText(projectFile);

        Assert.Contains("Codex.CommandEngine.Core", projectText, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CommandEngineIntegration_SmokeRuntimeProviderFactoryExists()
    {
        string factoryFile = FindHostFile(
            "CommandEngineIntegration",
            "SmokeRuntimeProviderFactory.cs");

        Assert.True(File.Exists(factoryFile), $"File was not found: {factoryFile}");
    }

    [Fact]
    public void CommandEngineIntegration_SmokeRuntimeProviderFactoryCreatesRuntimeProvider()
    {
        string factoryFile = FindHostFile(
            "CommandEngineIntegration",
            "SmokeRuntimeProviderFactory.cs");

        string text = File.ReadAllText(factoryFile);

        Assert.Contains("SmokeRuntimeProviderFactory", text, StringComparison.Ordinal);
        Assert.Contains("ExtensionRuntimeProvider", text, StringComparison.Ordinal);
    }

    internal static string FindExtensionsRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (IsExtensionsRoot(directory.FullName))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        DirectoryInfo? currentDirectory = new(Directory.GetCurrentDirectory());

        while (currentDirectory is not null)
        {
            if (IsExtensionsRoot(currentDirectory.FullName))
            {
                return currentDirectory.FullName;
            }

            currentDirectory = currentDirectory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not find Extensions solution root containing Extensions.sln or Host/Modules/Tests folders.");
    }

    private static bool IsExtensionsRoot(
        string folder)
    {
        if (File.Exists(Path.Combine(folder, "Extensions.sln")))
        {
            return true;
        }

        return Directory.Exists(Path.Combine(folder, "Host"))
            && Directory.Exists(Path.Combine(folder, "Modules"))
            && Directory.Exists(Path.Combine(folder, "Tests"));
    }

    private static string FindCommandEngineIntegrationProjectFile()
    {
        string extensionsRoot = FindExtensionsRoot();

        string[] candidateFiles =
        [
            Path.Combine(
                extensionsRoot,
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "CodexExpensa.ExtensionDevHost.CommandEngineIntegration.csproj"),

            Path.Combine(
                extensionsRoot,
                "Host",
                "CodexExpensa.ExtensionDevHost.CommandEngineIntegration",
                "CodexExpensa.ExtensionDevHost.CommandEngineIntegration.csproj")
        ];

        foreach (string candidateFile in candidateFiles)
        {
            if (File.Exists(candidateFile))
            {
                return candidateFile;
            }
        }

        string hostRoot = Path.Combine(extensionsRoot, "Host");

        string? discoveredProject =
            Directory.Exists(hostRoot)
                ? Directory.GetFiles(
                        hostRoot,
                        "CodexExpensa.ExtensionDevHost.CommandEngineIntegration.csproj",
                        SearchOption.AllDirectories)
                    .OrderBy(static path => path, StringComparer.OrdinalIgnoreCase)
                    .FirstOrDefault()
                : null;

        if (!string.IsNullOrWhiteSpace(discoveredProject))
        {
            return discoveredProject;
        }

        throw new FileNotFoundException(
            $"Could not find CodexExpensa.ExtensionDevHost.CommandEngineIntegration.csproj under Host folder: {hostRoot}");
    }

    private static string FindHostFile(
        params string[] relativeParts)
    {
        string extensionsRoot = FindExtensionsRoot();

        string[] candidateRoots =
        [
            Path.Combine(extensionsRoot, "Host", "CodexExpensa.ExtensionDevHost"),
            Path.Combine(extensionsRoot, "Host", "CodexExpensa.ExtensionDevHost.CommandEngineIntegration")
        ];

        foreach (string candidateRoot in candidateRoots)
        {
            string candidateFile =
                Path.Combine(
                    candidateRoot,
                    Path.Combine(relativeParts));

            if (File.Exists(candidateFile))
            {
                return candidateFile;
            }
        }

        string hostRoot = Path.Combine(extensionsRoot, "Host");
        string fileName = relativeParts.Length == 0 ? "" : relativeParts[^1];

        string? discoveredFile =
            Directory.Exists(hostRoot) && !string.IsNullOrWhiteSpace(fileName)
                ? Directory.GetFiles(hostRoot, fileName, SearchOption.AllDirectories)
                    .OrderBy(static path => path, StringComparer.OrdinalIgnoreCase)
                    .FirstOrDefault()
                : null;

        if (!string.IsNullOrWhiteSpace(discoveredFile))
        {
            return discoveredFile;
        }

        string expectedFile =
            Path.Combine(
                candidateRoots[0],
                Path.Combine(relativeParts));

        throw new FileNotFoundException(
            $"Could not find host file. First expected path: {expectedFile}");
    }
}
