using System.Windows.Forms;
using CodexExpensa.ExtensionDevHost.Services.Ai;

namespace CodexExpensa.ExtensionDevHost.UI;

public sealed partial class NewAddinProjectForm
{
    private async Task ApplyChatGptAsync()
    {
        if (!_chkUseChatGpt.Checked)
        {
            return;
        }

        try
        {
            _btnGenerate.Enabled = false;
            _btnGenerate.Text = "Generating...";

            OpenAiApiKeyStore apiKeyStore = new();
            IAddinScaffoldAiService service = new OpenAiAddinScaffoldAiService(apiKeyStore);

            CodexExpensa.ExtensionDevHost.Models.AddinScaffoldAiResult result =
                await service.GenerateAsync(new CodexExpensa.ExtensionDevHost.Models.AddinScaffoldAiRequest
                {
                    ProjectName = _txtProjectName.Text.Trim(),
                    AssemblyName = string.IsNullOrWhiteSpace(_txtAssemblyName.Text)
                        ? _txtProjectName.Text.Trim()
                        : _txtAssemblyName.Text.Trim(),
                    Description = _txtDescription.Text.Trim(),
                    Prompt = _txtChatGptPrompt.Text.Trim()
                });

            if (!string.IsNullOrWhiteSpace(result.SuggestedProjectName))
            {
                _txtProjectName.Text = result.SuggestedProjectName.Trim();
            }

            if (!string.IsNullOrWhiteSpace(result.SuggestedAssemblyName))
            {
                _txtAssemblyName.Text = result.SuggestedAssemblyName.Trim();
            }

            if (!string.IsNullOrWhiteSpace(result.SuggestedDescription))
            {
                _txtDescription.Text = result.SuggestedDescription.Trim();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "ChatGPT Suggestions", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _btnGenerate.Enabled = _chkUseChatGpt.Checked;
            _btnGenerate.Text = "Generate Suggestions";
        }
    }
}
