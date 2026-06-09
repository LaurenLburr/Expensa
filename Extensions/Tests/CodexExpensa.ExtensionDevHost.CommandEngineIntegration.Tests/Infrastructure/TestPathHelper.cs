using System;
using System.IO;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

internal static class TestPathHelper
{
    public static string RepositoryRoot =>
        FindRepositoryRoot();

    public static string ExtensionsRoot =>
        Path.Combine(RepositoryRoot, "Extensions");

    public static string HostRoot =>
        Path.Combine(ExtensionsRoot, "Host");

    public static string ModulesRoot =>
        Path.Combine(ExtensionsRoot, "Modules");

    public static string TestsRoot =>
        Path.Combine(ExtensionsRoot, "Tests");

    public static string HostPath(params string[] parts)
    {
        return Combine(HostRoot, parts);
    }

    public static string ModulePath(params string[] parts)
    {
        return Combine(ModulesRoot, parts);
    }

    public static string TestsPath(params string[] parts)
    {
        return Combine(TestsRoot, parts);
    }

    public static string ExtensionsPath(params string[] parts)
    {
        return Combine(ExtensionsRoot, parts);
    }

    public static string RepositoryPath(params string[] parts)
    {
        return Combine(RepositoryRoot, parts);
    }

    public static string ReadHostFile(params string[] parts)
    {
        return ReadRequiredFile(HostPath(parts));
    }

    public static string ReadModuleFile(params string[] parts)
    {
        return ReadRequiredFile(ModulePath(parts));
    }

    public static string ReadTestsFile(params string[] parts)
    {
        return ReadRequiredFile(TestsPath(parts));
    }

    public static string ReadExtensionsFile(params string[] parts)
    {
        return ReadRequiredFile(ExtensionsPath(parts));
    }

    public static string ReadRepositoryFile(params string[] parts)
    {
        return ReadRequiredFile(RepositoryPath(parts));
    }

    public static string ReadRequiredFile(string path)
    {
        Assert.True(File.Exists(path), $"File was not found: {path}");
        return File.ReadAllText(path);
    }

    public static string RequireFile(string path)
    {
        Assert.True(File.Exists(path), $"File was not found: {path}");
        return path;
    }

    public static string RequireDirectory(string path)
    {
        Assert.True(Directory.Exists(path), $"Directory was not found: {path}");
        return path;
    }

    private static string FindRepositoryRoot()
    {
        string? rootFromBaseDirectory =
            FindRepositoryRootStartingAt(AppContext.BaseDirectory);

        if (!string.IsNullOrWhiteSpace(rootFromBaseDirectory))
        {
            return rootFromBaseDirectory;
        }

        string? rootFromCurrentDirectory =
            FindRepositoryRootStartingAt(Directory.GetCurrentDirectory());

        if (!string.IsNullOrWhiteSpace(rootFromCurrentDirectory))
        {
            return rootFromCurrentDirectory;
        }

        throw new DirectoryNotFoundException(
            "Could not find repository root containing the Extensions folder with Host, Modules, and Tests folders.");
    }

    private static string? FindRepositoryRootStartingAt(string startPath)
    {
        DirectoryInfo? directory = new(startPath);

        while (directory is not null)
        {
            if (IsRepositoryRoot(directory.FullName))
            {
                return directory.FullName;
            }

            if (IsExtensionsRoot(directory.FullName))
            {
                return directory.Parent?.FullName;
            }

            directory = directory.Parent;
        }

        return null;
    }

    private static bool IsRepositoryRoot(string path)
    {
        string extensionsRoot = Path.Combine(path, "Extensions");
        return IsExtensionsRoot(extensionsRoot);
    }

    private static bool IsExtensionsRoot(string path)
    {
        return Directory.Exists(path)
            && Directory.Exists(Path.Combine(path, "Host"))
            && Directory.Exists(Path.Combine(path, "Modules"))
            && Directory.Exists(Path.Combine(path, "Tests"));
    }

    private static string Combine(string root, params string[] parts)
    {
        if (parts.Length == 0)
        {
            return root;
        }

        string path = root;

        foreach (string part in parts)
        {
            path = Path.Combine(path, part);
        }

        return path;
    }
}
