using System.Reflection;
using System.Text.Json;
using CodexExpensa.App.WinForms.Composition;

namespace CodexExpensa.App.WinForms.UI.Addins;

public sealed class AddinScreenRuntimeInvoker
{
    private readonly TreeAddinAssemblyLocator _assemblyLocator;

    public AddinScreenRuntimeInvoker()
        : this(new TreeAddinAssemblyLocator())
    {
    }

    public AddinScreenRuntimeInvoker(
        TreeAddinAssemblyLocator assemblyLocator)
    {
        _assemblyLocator =
            assemblyLocator
            ?? throw new ArgumentNullException(nameof(assemblyLocator));
    }

    public async Task<AddinScreenRuntimeResult> ExecuteAsync(
        TreeAddinDefinition definition,
        AddinScreenSelection selection,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(selection);

        Assembly assembly =
            _assemblyLocator.FindAssembly(definition);

        string assemblyPath =
            assembly.Location;

        Type runnerType =
            assembly.GetType(
                definition.ScreenRunnerTypeName,
                throwOnError: true)!;

        Type requestType =
            assembly.GetType(
                definition.ScreenRequestTypeName,
                throwOnError: true)!;

        object runner =
            Activator.CreateInstance(runnerType)
            ?? throw new InvalidOperationException(
                $"Could not create {definition.ScreenRunnerTypeName}.");

        object request =
            Activator.CreateInstance(requestType)
            ?? throw new InvalidOperationException(
                $"Could not create {definition.ScreenRequestTypeName}.");

        SetPropertyIfExists(
            request,
            "DatabasePath",
            AppPaths.DatabaseFilePath());

        SetPropertyIfExists(request, "NodeId", selection.NodeId);
        SetPropertyIfExists(request, "NodeType", selection.NodeType);
        SetPropertyIfExists(request, "EntityId", selection.EntityId);
        SetPropertyIfExists(request, "DisplayText", selection.DisplayText);
        SetPropertyIfExists(request, "Year", selection.Year);
        SetPropertyIfExists(request, "Month", selection.Month);
        SetPropertyIfExists(request, "Url", selection.Url);
        SetPropertyIfExists(request, "Category", selection.Category);
        SetPropertyIfExists(request, "IsActive", selection.IsActive);

        MethodInfo executeMethod =
            runnerType.GetMethod(
                "ExecuteAsync",
                BindingFlags.Public | BindingFlags.Instance,
                binder: null,
                types: [requestType, typeof(CancellationToken)],
                modifiers: null)
            ?? throw new MissingMethodException(
                runnerType.FullName,
                "ExecuteAsync");

        object? taskObject =
            executeMethod.Invoke(
                runner,
                [request, cancellationToken]);

        if (taskObject is not Task task)
        {
            throw new InvalidOperationException(
                $"{definition.AddinName} screen runner did not return a Task.");
        }

        await task.ConfigureAwait(true);

        PropertyInfo resultProperty =
            task.GetType().GetProperty("Result")
            ?? throw new InvalidOperationException(
                $"{definition.AddinName} screen task did not expose Result.");

        object result =
            resultProperty.GetValue(task)
            ?? throw new InvalidOperationException(
                $"{definition.AddinName} screen runner returned null.");

        string status =
            GetPropertyString(result, "Status");

        string message =
            GetPropertyString(result, "Message");

        string outputJson =
            GetPropertyString(result, "OutputJson");

        AddinScreenModel screen =
            ParseScreen(outputJson);

        AddinDeploymentMetadata deployment =
            AddinDeploymentMetadata.LoadForAssembly(
                assemblyPath);

        return new AddinScreenRuntimeResult
        {
            Definition = definition,
            Status = status,
            Message = message,
            Screen = screen,
            AssemblyPath = assemblyPath,
            AssemblyLastWriteTimeUtc =
                File.GetLastWriteTimeUtc(assemblyPath),
            AssemblyVersion =
                GetAssemblyVersion(assembly),
            DeployedUtc =
                deployment.DeployedUtc
        };
    }

    private static string GetAssemblyVersion(
        Assembly assembly)
    {
        string? informationalVersion =
            assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion;

        if (!string.IsNullOrWhiteSpace(informationalVersion))
        {
            int metadataSeparator =
                informationalVersion.IndexOf('+');

            return metadataSeparator >= 0
                ? informationalVersion[..metadataSeparator]
                : informationalVersion;
        }

        return assembly.GetName().Version?.ToString()
            ?? "Unknown";
    }

    private static AddinScreenModel ParseScreen(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new AddinScreenModel();
        }

        using JsonDocument document =
            JsonDocument.Parse(json);

        JsonElement root =
            document.RootElement;

        List<string> columns = [];

        if (root.TryGetProperty(
                "columns",
                out JsonElement columnsElement)
            &&
            columnsElement.ValueKind == JsonValueKind.Array)
        {
            columns.AddRange(
                columnsElement.EnumerateArray()
                    .Where(static item =>
                        item.ValueKind == JsonValueKind.String)
                    .Select(static item =>
                        item.GetString() ?? string.Empty));
        }

        List<IReadOnlyDictionary<string, object?>> rows = [];

        if (root.TryGetProperty(
                "rows",
                out JsonElement rowsElement)
            &&
            rowsElement.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement rowElement in
                     rowsElement.EnumerateArray())
            {
                if (rowElement.ValueKind != JsonValueKind.Object)
                {
                    continue;
                }

                Dictionary<string, object?> row =
                    new(StringComparer.OrdinalIgnoreCase);

                foreach (JsonProperty property in
                         rowElement.EnumerateObject())
                {
                    row[property.Name] =
                        ConvertJsonValue(property.Value);
                }

                rows.Add(row);
            }
        }

        List<string> hiddenColumns = [];

        if (root.TryGetProperty(
                "hiddenColumns",
                out JsonElement hiddenColumnsElement)
            &&
            hiddenColumnsElement.ValueKind == JsonValueKind.Array)
        {
            hiddenColumns.AddRange(
                hiddenColumnsElement.EnumerateArray()
                    .Where(static item =>
                        item.ValueKind == JsonValueKind.String)
                    .Select(static item =>
                        item.GetString() ?? string.Empty));
        }

        return new AddinScreenModel
        {
            FormatVersion =
                GetInt(root, "formatVersion", 1),
            Title =
                GetString(root, "title"),
            Subtitle =
                GetString(root, "subtitle"),
            Columns = columns,
            Rows = rows,
            IsHierarchical =
                GetBool(root, "isHierarchical"),
            IdColumnName =
                GetString(root, "idColumnName"),
            ParentIdColumnName =
                GetString(root, "parentIdColumnName"),
            HiddenColumns = hiddenColumns,
            AllowAddTransaction =
                GetBool(root, "allowAddTransaction")
        };
    }

    private static object? ConvertJsonValue(
        JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.Null => null,
            JsonValueKind.String => value.GetString(),
            JsonValueKind.Number when value.TryGetInt64(out long integer) =>
                integer,
            JsonValueKind.Number when value.TryGetDecimal(out decimal number) =>
                number,
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            _ => value.ToString()
        };
    }

    private static string GetString(
        JsonElement element,
        string propertyName)
    {
        return element.TryGetProperty(
                   propertyName,
                   out JsonElement property)
               &&
               property.ValueKind == JsonValueKind.String
            ? property.GetString() ?? string.Empty
            : string.Empty;
    }

    private static bool GetBool(
        JsonElement element,
        string propertyName)
    {
        return element.TryGetProperty(
                   propertyName,
                   out JsonElement property)
               &&
               property.ValueKind is JsonValueKind.True or JsonValueKind.False
            ? property.GetBoolean()
            : false;
    }

    private static int GetInt(
        JsonElement element,
        string propertyName,
        int fallback)
    {
        return element.TryGetProperty(
                   propertyName,
                   out JsonElement property)
               &&
               property.TryGetInt32(out int value)
            ? value
            : fallback;
    }

    private static void SetPropertyIfExists(
        object instance,
        string propertyName,
        object? value)
    {
        PropertyInfo? property =
            instance.GetType().GetProperty(propertyName);

        if (property is null ||
            !property.CanWrite)
        {
            return;
        }

        property.SetValue(instance, value);
    }

    private static string GetPropertyString(
        object instance,
        string propertyName)
    {
        PropertyInfo? property =
            instance.GetType().GetProperty(propertyName);

        return Convert.ToString(
                   property?.GetValue(instance))
               ?? string.Empty;
    }

}
