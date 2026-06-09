using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteRuntimeModuleInvokerResultMappingTests
{
    [Fact]
    public void Invoker_MapsSmokeRunnerResultByPropertyInsteadOfStrictCast()
    {
        string text =
            ReadFile(
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "HostWebsiteRuntimeModuleInvoker.cs");

        Assert.Contains("MapCommandExecutionResult(result)", text);
        Assert.Contains("CommandName = GetPropertyString(result, \"CommandName\", DefaultCommandName)", text);
        Assert.Contains("Status = GetPropertyEnum(result, \"Status\", CommandExecutionStatus.Failed)", text);
        Assert.Contains("GetPropertyString(result, \"CorrelationId\"", text);
        Assert.Contains("GetPropertyString(result, \"Message\"", text);
        Assert.Contains("GetPropertyString(result, \"OutputJson\"", text);
        Assert.DoesNotContain("throw new InvalidOperationException(\"Website smoke runner did not return CommandExecutionResult.\")", text);
    }

    [Fact]
    public void Invoker_ParsesStatusAsCommandExecutionStatusEnum()
    {
        string text =
            ReadFile(
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "HostWebsiteRuntimeModuleInvoker.cs");

        Assert.Contains("private static CommandExecutionStatus GetPropertyEnum", text);
        Assert.Contains("Enum.TryParse(", text);
        Assert.Contains("out CommandExecutionStatus parsedStatus", text);
    }

    [Fact]
    public void Invoker_StillUsesDependencyAwareLoadContext()
    {
        string text =
            ReadFile(
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "HostWebsiteRuntimeModuleInvoker.cs");

        Assert.Contains("AssemblyDependencyResolver", text);
        Assert.Contains("WebsiteAddinDependencyLoadContext", text);
        Assert.Contains("LoadMainAssembly", text);
    }

    private static string ReadFile(params string[] parts)
    {
        string repositoryRoot =
            FindRepositoryRoot();

        string path =
            Path.Combine([repositoryRoot, .. parts]);

        Assert.True(
            File.Exists(path),
            $"File was not found: {path}");

        return File.ReadAllText(path);
    }

    private static string FindRepositoryRoot()
    {
        return TestPathHelper.ExtensionsRoot;
    }
}
