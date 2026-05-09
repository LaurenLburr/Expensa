using CodexExpensa.ExtensionDevHost.Services.Ai;
using System.Windows.Forms;

namespace CodexExpensa.ExtensionDevHost.UI;

public sealed class OpenAiApiKeyForm : Form
{
    private readonly OpenAiApiKeyStore _store = new();
    private readonly TextBox _txtKeyName;
    private readonly TextBox _txtApiKey;
    private readonly Button _showHideButton;

    public OpenAiApiKeyForm()
    {
        Text = "OpenAI API Key";
        Width = 760;
        Height = 230;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        TableLayoutPanel layout = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 4,
            Padding = new Padding(12)
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));

        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));

        layout.Controls.Add(new Label { Text = "Key Name", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
        _txtKeyName = new TextBox
        {
            Dock = DockStyle.Fill,
            Text = _store.GetKeyName()
        };
        layout.Controls.Add(_txtKeyName, 1, 0);
        layout.SetColumnSpan(_txtKeyName, 2);

        layout.Controls.Add(new Label { Text = "API Key", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
        _txtApiKey = new TextBox
        {
            Dock = DockStyle.Fill,
            UseSystemPasswordChar = true,
            Text = _store.GetApiKey()
        };
        layout.Controls.Add(_txtApiKey, 1, 1);

        _showHideButton = new Button { Text = "Show", Dock = DockStyle.Fill };
        _showHideButton.Click += (_, _) => ToggleApiKeyVisibility();
        layout.Controls.Add(_showHideButton, 2, 1);

        Label note = new()
        {
            Dock = DockStyle.Fill,
            Text = "Key Name is only a local label to help you identify which key is saved. The API key is still stored in the existing local settings file.",
            AutoSize = false
        };
        layout.Controls.Add(note, 1, 2);
        layout.SetColumnSpan(note, 2);

        FlowLayoutPanel buttons = new()
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true
        };

        Button save = new() { Text = "Save", AutoSize = true };
        save.Click += (_, _) =>
        {
            _store.SaveApiKey(_txtKeyName.Text, _txtApiKey.Text);
            DialogResult = DialogResult.OK;
            Close();
        };

        Button cancel = new() { Text = "Cancel", AutoSize = true };
        cancel.Click += (_, _) => Close();

        buttons.Controls.Add(save);
        buttons.Controls.Add(cancel);

        layout.Controls.Add(buttons, 1, 3);
        layout.SetColumnSpan(buttons, 2);

        Controls.Add(layout);
    }

    private void ToggleApiKeyVisibility()
    {
        _txtApiKey.UseSystemPasswordChar = !_txtApiKey.UseSystemPasswordChar;
        _showHideButton.Text = _txtApiKey.UseSystemPasswordChar ? "Show" : "Hide";
    }
}
