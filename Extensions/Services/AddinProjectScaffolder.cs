using System.Text;
using CodexExpensa.ExtensionDevHost.Models;

namespace CodexExpensa.ExtensionDevHost.Services;

public sealed class AddinProjectScaffolder
{
    public AddinProjectScaffolderResult Create(NewAddinProjectRequest request, int sortOrder)
    {
        ArgumentNullException.ThrowIfNull(request);

        string solutionRoot = request.SolutionRootFolder.Trim();
        string projectName = SanitizeIdentifier(request.ProjectName);
        string assemblyName = SanitizeIdentifier(string.IsNullOrWhiteSpace(request.AssemblyName) ? request.ProjectName : request.AssemblyName);

        if (string.IsNullOrWhiteSpace(solutionRoot) || !Directory.Exists(solutionRoot))
        {
            throw new InvalidOperationException("Solution root folder is required and must exist.");
        }

        if (string.IsNullOrWhiteSpace(projectName))
        {
            throw new InvalidOperationException("Project name is required.");
        }

        string projectFolder = Path.Combine(solutionRoot, "Modules", projectName);
        if (Directory.Exists(projectFolder))
        {
            throw new InvalidOperationException($"Project folder already exists: {projectFolder}");
        }

        Directory.CreateDirectory(projectFolder);

        List<string> created = new();

        string projectFilePath = Path.Combine(projectFolder, projectName + ".csproj");
        File.WriteAllText(projectFilePath, BuildProjectFile());
        created.Add(projectFilePath);

        string classFilePath = Path.Combine(projectFolder, projectName + "Extension.cs");
        File.WriteAllText(classFilePath, BuildExtensionFile(projectName, request.Description));
        created.Add(classFilePath);

        string readmePath = Path.Combine(projectFolder, "README.md");
        File.WriteAllText(readmePath, BuildReadme(projectName, request.Description, request.ChatGptPrompt, request.UseChatGpt));
        created.Add(readmePath);

        if (request.UseChatGpt && !string.IsNullOrWhiteSpace(request.ChatGptPrompt))
        {
            string promptPath = Path.Combine(projectFolder, "ScaffoldPrompt.md");
            File.WriteAllText(promptPath, request.ChatGptPrompt);
            created.Add(promptPath);
        }

        ExtensionProjectRegistration registration = new()
        {
            ProjectName = projectName,
            AssemblyName = assemblyName,
            RelativeBinPath = Path.Combine("Modules", projectName, "bin", "Debug", "net8.0-windows"),
            IsEnabled = true,
            SortOrder = sortOrder
        };

        return new AddinProjectScaffolderResult
        {
            ProjectFolder = projectFolder,
            CreatedPaths = created,
            Registration = registration
        };
    }

    private static string BuildProjectFile()
    {
        return @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net8.0-windows</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
";
    }

    private static string BuildExtensionFile(string projectName, string description)
    {
        string safeDescription = string.IsNullOrWhiteSpace(description)
            ? "Blank Expensa add-in scaffold."
            : description.Trim();

        return
            "namespace " + projectName + ";" + Environment.NewLine + Environment.NewLine +
            "/// <summary>" + Environment.NewLine +
            "/// " + safeDescription + Environment.NewLine +
            "/// </summary>" + Environment.NewLine +
            "public sealed class " + projectName + "Extension" + Environment.NewLine +
            "{" + Environment.NewLine +
            "}" + Environment.NewLine;
    }

    private static string BuildReadme(string projectName, string description, string prompt, bool usedAi)
    {
        StringBuilder sb = new();
        sb.AppendLine("# " + projectName);
        sb.AppendLine();
        sb.AppendLine(string.IsNullOrWhiteSpace(description) ? "Blank Expensa add-in scaffold." : description.Trim());

        if (usedAi && !string.IsNullOrWhiteSpace(prompt))
        {
            sb.AppendLine();
            sb.AppendLine("## ChatGPT prompt");
            sb.AppendLine();
            sb.AppendLine(prompt.Trim());
        }

        return sb.ToString();
    }

    private static string SanitizeIdentifier(string value)
    {
        string trimmed = value.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            return string.Empty;
        }

        char[] invalid = Path.GetInvalidFileNameChars();
        StringBuilder sb = new(trimmed.Length);

        foreach (char ch in trimmed)
        {
            if (invalid.Contains(ch))
            {
                continue;
            }

            if (char.IsLetterOrDigit(ch) || ch == '_' || ch == '.')
            {
                sb.Append(ch);
            }
            else if (char.IsWhiteSpace(ch) || ch == '-')
            {
                sb.Append('_');
            }
        }

        return sb.ToString().Trim('_');
    }
}

public sealed class AddinProjectScaffolderResult
{
    public required string ProjectFolder { get; init; }
    public required IReadOnlyList<string> CreatedPaths { get; init; }
    public required ExtensionProjectRegistration Registration { get; init; }
}
