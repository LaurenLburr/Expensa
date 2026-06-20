using Xunit;

namespace CodexExpensa.App.WinForms.Tests.UI.Addins;

public sealed class TreeAddinAssemblyLocatorStructureTests
{
    [Fact]
    public void AssemblyLocator_PrefersCompiledExpensaExtensionsFolder()
    {
        string text = TestRepositoryPath.ReadExpensaFile("UI", "Addins", "TreeAddinAssemblyLocator.cs");

        Assert.Contains("\"Expensa\"", text);
        Assert.Contains("\"Extensions\"", text);
        Assert.Contains("definition.AddinName", text);
        Assert.Contains("definition.ProjectAssemblyName", text);
        Assert.Contains("Assembly.LoadFrom(assemblyPath)", text);
    }

    [Fact]
    public void AssemblyLocator_ErrorListsSearchedPaths()
    {
        string text = TestRepositoryPath.ReadExpensaFile("UI", "Addins", "TreeAddinAssemblyLocator.cs");

        Assert.Contains("string searched", text);
        Assert.Contains("Could not find", text);
    }
}
