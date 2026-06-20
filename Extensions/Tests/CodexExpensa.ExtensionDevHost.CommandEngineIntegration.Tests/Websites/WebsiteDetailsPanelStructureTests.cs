using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class WebsiteDetailsPanelStructureTests
{
    [Fact]
    public void DetailsPanel_LoadsWebsitePayloadDetails()
    {
        string panelText = ReadFile(
            "Host", "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration", "Websites", "WebsitesDetailsPanel.cs");

        string controlText = ReadFile(
            "Host", "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration", "Websites", "WebsiteDetailsWpfControl.xaml.cs");

        Assert.Contains("ElementHost", panelText);
        Assert.Contains("WebsiteDetailsWpfControl", panelText);
        Assert.Contains("ShowWebsite", panelText);
        Assert.Contains("HostWebsiteTreeNodePayload", panelText);
        Assert.Contains("CreateWebsiteCredentialKey", panelText);
        Assert.Contains("WebsiteId:", controlText);
        Assert.Contains("CredentialKey:", controlText);
        Assert.Contains("Url:", controlText);
        Assert.Contains("Tag:", controlText);
    }

    [Fact]
    public void DetailsPanel_UsesWpfEditorForWebsiteCredentials()
    {
        string xamlText = ReadFile(
            "Host", "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration", "Websites", "WebsiteDetailsWpfControl.xaml");

        string codeText = ReadFile(
            "Host", "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration", "Websites", "WebsiteDetailsWpfControl.xaml.cs");

        Assert.Contains("CheckBox", xamlText);
        Assert.Contains("Content=\"Editable\"", xamlText);
        Assert.Contains("NameTextBox", xamlText);
        Assert.Contains("CredentialKeyTextBox", xamlText);
        Assert.Contains("UrlTextBox", xamlText);
        Assert.Contains("UsernameTextBox", xamlText);
        Assert.Contains("PasswordTextBox", xamlText);
        Assert.Contains("PasswordMaskedBox", xamlText);
        Assert.Contains("SetEditable", codeText);
        Assert.Contains("CredentialKeyTextBox.IsReadOnly = true", codeText);
        Assert.Contains("UrlTextBox.IsReadOnly = true", codeText);
        Assert.Contains("NameTextBox.IsReadOnly", codeText);
        Assert.Contains("UsernameTextBox.IsReadOnly", codeText);
        Assert.Contains("PasswordTextBox.IsReadOnly", codeText);
        Assert.Contains("PasswordMaskedBox.Visibility", codeText);
        Assert.Contains("Windows Credential Manager", codeText);
    }

    [Fact]
    public void DetailsPanel_HasWpfPasswordGeneratorAndApplyLink()
    {
        string xamlText = ReadFile(
            "Host", "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration", "Websites", "WebsiteDetailsWpfControl.xaml");

        string codeText = ReadFile(
            "Host", "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration", "Websites", "WebsiteDetailsWpfControl.xaml.cs");

        string panelText = ReadFile(
            "Host", "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration", "Websites", "WebsitesDetailsPanel.cs");

        Assert.Contains("LowercaseCountTextBox", xamlText);
        Assert.Contains("UppercaseCountTextBox", xamlText);
        Assert.Contains("DigitCountTextBox", xamlText);
        Assert.Contains("SpecialCharactersTextBox", xamlText);
        Assert.Contains("SpecialCountTextBox", xamlText);
        Assert.Contains("MinimumLengthTextBox", xamlText);
        Assert.Contains("MaximumLengthTextBox", xamlText);
        Assert.Contains("GeneratePasswordButton", xamlText);
        Assert.Contains("GeneratedPasswordLabel", xamlText);
        Assert.Contains("SaveHyperlink_Click", xamlText);
        Assert.Contains(">save</Hyperlink>", xamlText);
        Assert.Contains("ImportHyperlink_Click", xamlText);
        Assert.Contains(">import</Hyperlink>", xamlText);
        Assert.Contains("apply to web", xamlText);
        Assert.Contains("RandomNumberGenerator", codeText);
        Assert.Contains("GeneratePassword", codeText);
        Assert.Contains("minimumLength", codeText);
        Assert.Contains("maximumLength", codeText);
        Assert.Contains("Required character counts exceed maximum length.", codeText);
        Assert.Contains("ApplyPasswordToWebRequested", codeText);
        Assert.Contains("SaveCredentialRequested", codeText);
        Assert.Contains("ImportCredentialRequested", codeText);
        Assert.Contains("TryCreateCredentialRequest", codeText);
        Assert.Contains("WebsiteCredentialApplyRequest", codeText);
        Assert.Contains("WebsiteCredentialImportRequest", codeText);
        Assert.Contains("credentialStore.Save", panelText);
        Assert.Contains("SaveCurrentCredential", panelText);
        Assert.Contains("OpenFileDialog", panelText);
        Assert.Contains("FindCredentialRecord", panelText);
        Assert.Contains("ParseCsvLine", panelText);
    }

    [Fact]
    public void DetailsPanel_CanRefreshCredentialsAndCopyCredentialValues()
    {
        string xamlText = ReadFile(
            "Host", "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration", "Websites", "WebsiteDetailsWpfControl.xaml");

        string codeText = ReadFile(
            "Host", "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration", "Websites", "WebsiteDetailsWpfControl.xaml.cs");

        string panelText = ReadFile(
            "Host", "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration", "Websites", "WebsitesDetailsPanel.cs");

        Assert.Contains("RefreshCredentialsButton", xamlText);
        Assert.Contains("CopyUrlHyperlink_Click", xamlText);
        Assert.Contains("CopyUsernameHyperlink_Click", xamlText);
        Assert.Contains("CopyPasswordHyperlink_Click", xamlText);
        Assert.Contains("RefreshCredentialsRequested", codeText);
        Assert.Contains("UpdateCredentialValues", codeText);
        Assert.Contains("ProcessStartInfo", codeText);
        Assert.Contains("UseShellExecute = true", codeText);
        Assert.Contains("System.Windows.Clipboard.SetText", codeText);
        Assert.Contains("GetCurrentPassword", codeText);
        Assert.Contains("detailsControl_RefreshCredentialsRequested", panelText);
        Assert.Contains("credentialStore.TryGet", panelText);
    }

    [Fact]
    public void Form_LoadsDetailsOnTreeSelection()
    {
        string formText = ReadFile(
            "Host", "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration", "Websites", "WebsitesTreeLoadVerificationForm.cs");

        string detailsText = ReadFile(
            "Host", "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration", "Websites", "WebsitesTreeLoadVerificationForm.Details.cs");

        Assert.Contains("websitesTreeView_AfterSelect", formText);
        Assert.Contains("ShowSelectedTreeNodeDetails", formText);
        Assert.Contains("WebsitesTreeLoadVerificationSettings.json", formText);
        Assert.Contains("splitContainer.SplitterMoved += splitContainer_SplitterMoved", formText);
        Assert.Contains("FormClosing += websitesTreeLoadVerificationForm_FormClosing", formText);
        Assert.Contains("RestoreSplitterDistance", formText);
        Assert.Contains("SaveSplitterDistance", formText);
        Assert.Contains("SplitterDistance", formText);
        Assert.Contains("EnsureWebsiteDetailsPanel", detailsText);
        Assert.Contains("splitContainer.FixedPanel = FixedPanel.Panel1", detailsText);
        Assert.Contains("_websitesDetailsPanel.SaveCurrentCredential()", detailsText);
        Assert.Contains("ShowWebsite", detailsText);
        Assert.Contains("ShowGroup", detailsText);
    }

    [Fact]
    public void TopWebsiteNode_ShowsOverviewGridAndDoubleClickSelectsTreeNode()
    {
        string overviewXaml = ReadFile(
            "Host", "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration", "Websites", "WebsiteOverviewWpfControl.xaml");

        string overviewCode = ReadFile(
            "Host", "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration", "Websites", "WebsiteOverviewWpfControl.xaml.cs");

        string panelText = ReadFile(
            "Host", "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration", "Websites", "WebsitesDetailsPanel.cs");

        string detailsText = ReadFile(
            "Host", "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration", "Websites", "WebsitesTreeLoadVerificationForm.Details.cs");

        Assert.Contains("DataGrid", overviewXaml);
        Assert.Contains("WebsiteId", overviewXaml);
        Assert.Contains("Url", overviewXaml);
        Assert.Contains("WebsiteRowDoubleClicked", overviewCode);
        Assert.Contains("SetWebsites", overviewCode);
        Assert.Contains("WebsiteOverviewGridRow", overviewCode);
        Assert.Contains("ShowOverview", panelText);
        Assert.Contains("WebsiteOverviewRowDoubleClicked", panelText);
        Assert.Contains("IsWebsitesRootNode", detailsText);
        Assert.Contains("_websitesDetailsPanel.ShowOverview(GetWebsitePayloadsFromTree())", detailsText);
        Assert.Contains("FindWebsiteTreeNodeByWebsiteId", detailsText);
        Assert.Contains("websitesTreeView.SelectedNode = matchingNode", detailsText);
    }

    private static string ReadFile(params string[] parts)
    {
        string repositoryRoot = FindRepositoryRoot();
        string path = Path.Combine([repositoryRoot, .. parts]);
        Assert.True(File.Exists(path), $"File was not found: {path}");
        return File.ReadAllText(path);
    }

    private static string FindRepositoryRoot()
    {
        return TestPathHelper.ExtensionsRoot;
    }
}
