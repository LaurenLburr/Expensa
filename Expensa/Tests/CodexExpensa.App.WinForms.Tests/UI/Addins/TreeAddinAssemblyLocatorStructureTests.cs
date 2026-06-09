using Xunit;

namespace CodexExpensa.App.WinForms.Tests.UI.Addins;

public sealed class TreeAddinAssemblyLocatorStructureTests
{
    [Fact]
    public void AssemblyLocator_SearchesDeployedModulesFolder()
    {
        string text = TestRepositoryPath.ReadExpensaFile("UI", "Addins", "TreeAddinAssemblyLocator.cs");

        Assert.Contains("AppContext.BaseDirectory", text);
        Assert.Contains("\"Modules\"", text);
        Assert.Contains("definition.ModuleFolderName", text);
        Assert.Contains("definition.AssemblyFileName", text);
    }

    [Fact]
    public void AssemblyLocator_ErrorTellsUserToDeployAddin()
    {
        string text = TestRepositoryPath.ReadExpensaFile("UI", "Addins", "TreeAddinAssemblyLocator.cs");

        Assert.Contains("Deploy", text);
        Assert.Contains("to Expensa first", text);
    }
}
