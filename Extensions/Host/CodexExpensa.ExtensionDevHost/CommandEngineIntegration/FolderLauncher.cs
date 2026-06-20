using System.Diagnostics;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

internal static class FolderLauncher
{
    public static void OpenContainingFolder(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        string? folderPath = Path.GetDirectoryName(filePath);

        if (string.IsNullOrWhiteSpace(folderPath))
        {
            throw new InvalidOperationException(
                $"Could not determine the folder for '{filePath}'.");
        }

        if (!Directory.Exists(folderPath))
        {
            throw new DirectoryNotFoundException(
                $"The database folder does not exist:{Environment.NewLine}{folderPath}");
        }

        Process.Start(
            new ProcessStartInfo
            {
                FileName = folderPath,
                UseShellExecute = true
            });
    }
}
