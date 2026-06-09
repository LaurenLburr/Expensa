using CodexExpensa.ExtensionDevHost.Models;
using System.Reflection;
using System.Xml.Linq;

namespace CodexExpensa.ExtensionDevHost.Services;

public sealed class ExistingAddinProjectImportService
{
    private readonly ExtensionProjectRegistrationStore registrationStore;

    public ExistingAddinProjectImportService()
        : this(new ExtensionProjectRegistrationStore())
    {
    }

    public ExistingAddinProjectImportService(ExtensionProjectRegistrationStore registrationStore)
    {
        ArgumentNullException.ThrowIfNull(registrationStore);
        this.registrationStore = registrationStore;
    }

    public ExistingAddinProjectImportResult ImportProject(string projectFilePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectFilePath);
        string fullProjectFilePath = Path.GetFullPath(projectFilePath);

        if (!File.Exists(fullProjectFilePath))
        {
            throw new FileNotFoundException($"Project file was not found: {fullProjectFilePath}", fullProjectFilePath);
        }

        if (!string.Equals(Path.GetExtension(fullProjectFilePath), ".csproj", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Select a C# project file (.csproj).");
        }

        string projectFolder = Path.GetDirectoryName(fullProjectFilePath) ?? throw new DirectoryNotFoundException(fullProjectFilePath);
        string projectName = Path.GetFileNameWithoutExtension(fullProjectFilePath);
        string targetFramework = ReadTargetFramework(fullProjectFilePath);
        string assemblyName = ReadAssemblyName(fullProjectFilePath, projectName);
        string extensionsRoot = FindExtensionsRoot(projectFolder);
        string outputDllPath = Path.Combine(projectFolder, "bin", "Debug", targetFramework, assemblyName);
        string relativeBinPath = Path.GetRelativePath(extensionsRoot, outputDllPath);

        ExtensionProjectRegistration registration = CreateRegistration(projectName, relativeBinPath);
        UpsertRegistration(registration);

        return new ExistingAddinProjectImportResult
        {
            ProjectName = projectName,
            ProjectFilePath = fullProjectFilePath,
            ProjectFolder = projectFolder,
            AssemblyName = assemblyName,
            TargetFramework = targetFramework,
            RelativeBinPath = relativeBinPath
        };
    }

    private void UpsertRegistration(ExtensionProjectRegistration registration)
    {
        MethodInfo? method = registrationStore.GetType().GetMethod("Upsert", BindingFlags.Public | BindingFlags.Instance, binder: null, types: [typeof(ExtensionProjectRegistration)], modifiers: null);
        if (method is null)
        {
            throw new MissingMethodException(registrationStore.GetType().FullName, "Upsert");
        }
        method.Invoke(registrationStore, [registration]);
    }

    private static ExtensionProjectRegistration CreateRegistration(string projectName, string relativeBinPath)
    {
        Type type = typeof(ExtensionProjectRegistration);
        object? instance = Activator.CreateInstance(type);

        if (instance is null)
        {
            ConstructorInfo? constructor = type.GetConstructors().OrderBy(static candidate => candidate.GetParameters().Length).FirstOrDefault();
            if (constructor is null)
            {
                throw new MissingMethodException(type.FullName, ".ctor");
            }

            object?[] args = constructor.GetParameters().Select(parameter =>
                string.Equals(parameter.Name, "projectName", StringComparison.OrdinalIgnoreCase) ? projectName :
                string.Equals(parameter.Name, "relativeBinPath", StringComparison.OrdinalIgnoreCase) ? relativeBinPath :
                GetDefaultValue(parameter.ParameterType)).ToArray();

            instance = constructor.Invoke(args);
        }

        SetPropertyIfExists(instance, "ProjectName", projectName);
        SetPropertyIfExists(instance, "RelativeBinPath", relativeBinPath);
        return (ExtensionProjectRegistration)instance;
    }

    private static void SetPropertyIfExists(object instance, string propertyName, object value)
    {
        PropertyInfo? property = instance.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        if (property is null || !property.CanWrite)
        {
            return;
        }
        property.SetValue(instance, value);
    }

    private static object? GetDefaultValue(Type type) => type.IsValueType ? Activator.CreateInstance(type) : null;

    private static string ReadTargetFramework(string projectFilePath)
    {
        XDocument document = XDocument.Load(projectFilePath);
        string? targetFramework = document.Descendants().FirstOrDefault(static element => string.Equals(element.Name.LocalName, "TargetFramework", StringComparison.OrdinalIgnoreCase))?.Value?.Trim();
        if (!string.IsNullOrWhiteSpace(targetFramework)) return targetFramework;
        string? firstTargetFramework = document.Descendants().FirstOrDefault(static element => string.Equals(element.Name.LocalName, "TargetFrameworks", StringComparison.OrdinalIgnoreCase))?.Value?.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).FirstOrDefault();
        return string.IsNullOrWhiteSpace(firstTargetFramework) ? "net8.0-windows" : firstTargetFramework;
    }

    private static string ReadAssemblyName(string projectFilePath, string defaultProjectName)
    {
        XDocument document = XDocument.Load(projectFilePath);
        string? assemblyName = document.Descendants().FirstOrDefault(static element => string.Equals(element.Name.LocalName, "AssemblyName", StringComparison.OrdinalIgnoreCase))?.Value?.Trim();
        if (string.IsNullOrWhiteSpace(assemblyName)) assemblyName = defaultProjectName;
        return assemblyName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ? assemblyName : assemblyName + ".dll";
    }

    private static string FindExtensionsRoot(string startFolder)
    {
        DirectoryInfo? directory = new(startFolder);
        while (directory is not null)
        {
            if (string.Equals(directory.Name, "Extensions", StringComparison.OrdinalIgnoreCase)) return directory.FullName;
            directory = directory.Parent;
        }
        throw new DirectoryNotFoundException("The selected add-in project must be under the Extensions folder.");
    }
}
