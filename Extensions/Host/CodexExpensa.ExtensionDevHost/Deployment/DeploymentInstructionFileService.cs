using System.Text.Json;

namespace CodexExpensa.ExtensionDevHost.Deployment;

public sealed class DeploymentInstructionFileService
{
    private static readonly JsonSerializerOptions JsonOptions =
        new()
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            WriteIndented = true
        };

    public DeploymentInstructionDocument Load(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                "The deployment instruction file was not found.",
                path);
        }

        string json =
            File.ReadAllText(path);

        return JsonSerializer.Deserialize<DeploymentInstructionDocument>(
                   json,
                   JsonOptions)
               ?? throw new InvalidDataException(
                   "The deployment instruction file did not contain a document.");
    }

    public void Save(
        string path,
        DeploymentInstructionDocument document)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(document);

        string? folder =
            Path.GetDirectoryName(
                Path.GetFullPath(path));

        if (!string.IsNullOrWhiteSpace(folder))
        {
            Directory.CreateDirectory(folder);
        }

        File.WriteAllText(
            path,
            JsonSerializer.Serialize(
                document,
                JsonOptions));
    }
}
