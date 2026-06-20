using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class ExtensionProjectDisplaySortUiTests
{
    [Fact]
    public void ManageExtensionsForm_ExposesDisplaySortEditor()
    {
        string formText = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "UI",
            "ManageExtensionsForm.cs");

        Assert.Contains("HeaderText = \"Display Sort\"", formText);
        Assert.Contains("nameof(ExtensionProjectRegistration.DisplaySort)", formText);
        Assert.Contains("Set Display Sort...", formText);
        Assert.Contains("SetSelectedDisplaySort", formText);
    }

    [Fact]
    public void MainForm_AddinProjectContextMenuExposesDisplaySortEditor()
    {
        string formText = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "MainForm.cs");

        Assert.Contains("Set Display Sort...", formText);
        Assert.Contains("SetContextAddinDisplaySort", formText);
        Assert.Contains("_registrationStore.Upsert(registration)", formText);
        Assert.Contains("BuildNavigationTree()", formText);
    }

    [Fact]
    public void ProjectRegistration_ModelProvidesDisplaySortAlias()
    {
        string modelText = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "Models",
            "ExtensionProjectRegistration.cs");

        Assert.Contains("public int DisplaySort", modelText);
        Assert.Contains("get => SortOrder", modelText);
        Assert.Contains("set => SortOrder = value", modelText);
    }
}
