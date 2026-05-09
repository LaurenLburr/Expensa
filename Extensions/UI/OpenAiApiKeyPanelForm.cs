using CodexExpensa.ExtensionDevHost.Commands.Services;
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

    private async void TestButton_Click(object? sender, EventArgs e)
    {
        string keyName = keyNameTextBox.Text.Trim();
        string key = apiKeyTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(key))
        {
            SetStatus("No API key is currently entered.");
            MessageBox.Show(
                this,
                "Enter or save an API key before testing.",
                "OpenAI API Key",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        try
        {
            testButton.Enabled = false;
            UseWaitCursor = true;
            SetStatus("Saving key and sending live AI test request...");

            _store.SaveApiKey(keyName, key);

            OpenAiCommandAiService ai = new(_store);
            string result = await ai.GenerateScaffoldAsync(
                "Create a concise sample extension idea for tracking useful websites by tag.");

            SetStatus("AI test completed successfully.");

            MessageBox.Show(
                this,
                result,
                "AI Test Result",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            SetStatus("AI test failed.");

            MessageBox.Show(
                this,
                ex.Message,
                "AI Test Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            UseWaitCursor = false;
            testButton.Enabled = true;
        }
    }

    private void SetStatus(string message)
    {
        statusLabel.Text = message;
    }
}
