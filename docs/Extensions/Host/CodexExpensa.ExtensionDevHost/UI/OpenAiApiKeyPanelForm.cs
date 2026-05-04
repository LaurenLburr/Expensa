using CodexExpensa.ExtensionDevHost.Services.Ai;

namespace CodexExpensa.ExtensionDevHost.UI;

public partial class OpenAiApiKeyPanelForm : Form
{
    private readonly OpenAiApiKeyStore _store = new();

    public OpenAiApiKeyPanelForm()
    {
        InitializeComponent();
        LoadCurrentSettings();
    }

    private void LoadCurrentSettings()
    {
        keyNameTextBox.Text = _store.GetKeyName();
        apiKeyTextBox.Text = _store.GetApiKey();
        settingsPathTextBox.Text = _store.SettingsPath;
        SetStatus("Loaded local OpenAI API key settings.");
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        _store.SaveApiKey(keyNameTextBox.Text, apiKeyTextBox.Text);
        SetStatus("Saved OpenAI API key settings.");
    }

    private void ReloadButton_Click(object? sender, EventArgs e)
    {
        LoadCurrentSettings();
    }

    private void ShowHideButton_Click(object? sender, EventArgs e)
    {
        apiKeyTextBox.UseSystemPasswordChar = !apiKeyTextBox.UseSystemPasswordChar;
        showHideButton.Text = apiKeyTextBox.UseSystemPasswordChar ? "Show" : "Hide";
    }

    private void TestButton_Click(object? sender, EventArgs e)
    {
        string key = apiKeyTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(key))
        {
            SetStatus("No API key is currently entered.");
            MessageBox.Show(this, "Enter or save an API key before testing.", "OpenAI API Key", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        // This is intentionally a local validation for now. The old AI Test command can still be
        // used separately until the full AI test workflow is embedded into this panel.
        SetStatus("API key is present. Full live API test will be wired into this panel in the next AI cleanup step.");
        MessageBox.Show(
            this,
            "API key is present. The live AI Test command will be embedded here in a later step.",
            "OpenAI API Key",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void SetStatus(string message)
    {
        statusLabel.Text = message;
    }
}
