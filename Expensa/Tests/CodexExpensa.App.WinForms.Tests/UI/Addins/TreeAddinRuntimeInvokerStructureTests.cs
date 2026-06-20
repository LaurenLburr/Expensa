using Xunit;

namespace CodexExpensa.App.WinForms.Tests.UI.Addins;

public sealed class TreeAddinRuntimeInvokerStructureTests
{
    [Fact]
    public void RuntimeInvoker_UsesLocatedCompiledAddinAssembly()
    {
        string text = TestRepositoryPath.ReadExpensaFile("UI", "Addins", "TreeAddinRuntimeInvoker.cs");

        Assert.Contains("assemblyLocator.FindAssembly(definition)", text);
        Assert.Contains("assembly.Location", text);
        Assert.DoesNotContain("AssemblyDependencyResolver", text);
        Assert.DoesNotContain("AssemblyLoadContext", text);
    }

    [Fact]
    public void RuntimeInvoker_SetsCommonRequestPropertiesWhenAvailable()
    {
        string text = TestRepositoryPath.ReadExpensaFile("UI", "Addins", "TreeAddinRuntimeInvoker.cs");

        Assert.Contains("SetPropertyIfExists(loadRequest, \"SearchText\"", text);
        Assert.Contains("SetPropertyIfExists(loadRequest, \"IncludeDisabled\"", text);
        Assert.Contains("SetPropertyIfExists(loadRequest, \"IncludeClosed\"", text);
        Assert.Contains("SetPropertyIfExists(loadRequest, \"MaximumRows\"", text);
        Assert.Contains("SetPropertyIfExists(loadRequest, \"DatabasePath\"", text);
        Assert.Contains("AppPaths.DatabaseFilePath()", text);
    }

    [Fact]
    public void RuntimeInvoker_InvokesExecuteAsyncByReflection()
    {
        string text = TestRepositoryPath.ReadExpensaFile("UI", "Addins", "TreeAddinRuntimeInvoker.cs");

        Assert.Contains("\"ExecuteAsync\"", text);
        Assert.Contains("types: [requestType, typeof(CancellationToken)]", text);
        Assert.Contains("executeMethod.Invoke", text);
        Assert.Contains("Result", text);
    }
}
