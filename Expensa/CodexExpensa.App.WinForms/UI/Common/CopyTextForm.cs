using System;
using System.Windows.Forms;

namespace CodexExpensa.App.WinForms.UI.Common;

public sealed class CopyTextForm : Form
{
    private readonly TextBox _txt;
    private readonly Button _btnCopy;
    private readonly Button _btnClose;

    public CopyTextForm(string title, string content)
    {
        if (string.IsNullOrWhiteSpace(title)) title = "Copy";
        content ??= string.Empty;

        Text = title;
        StartPosition = FormStartPosition.CenterParent;
        Width = 900;
        Height = 650;

        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12) };
        Controls.Add(panel);

        _txt = new TextBox
        {
            Multiline = true,
            ScrollBars = ScrollBars.Both,
            WordWrap = false,
            Dock = DockStyle.Fill,
            ReadOnly = true,
            Text = content
        };

        var bottom = new Panel { Dock = DockStyle.Bottom, Height = 44 };
        _btnCopy = new Button { Text = "Copy", Width = 90, Height = 28, Left = 0, Top = 8 };
        _btnClose = new Button { Text = "Close", Width = 90, Height = 28, Left = 0, Top = 8 };

        bottom.Controls.Add(_btnCopy);
        bottom.Controls.Add(_btnClose);

        bottom.Resize += (_, _) =>
        {
            _btnClose.Left = bottom.Width - _btnClose.Width;
            _btnCopy.Left = _btnClose.Left - _btnCopy.Width - 8;
        };

        _btnCopy.Click += (_, _) =>
        {
            try
            {
                Clipboard.SetText(_txt.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Copy failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };

        _btnClose.Click += (_, _) => Close();

        panel.Controls.Add(_txt);
        panel.Controls.Add(bottom);
    }
}