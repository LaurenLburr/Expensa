using System;
using System.Windows.Forms;

namespace CodexExpensa.App.WinForms.UI.Common;

public abstract class DetailsFormBase : Form
{
    private readonly Action _onSaved;

    protected Panel RootPanel { get; }
    protected Label TitleLabel { get; }
    protected Button SaveButton { get; }

    protected DetailsFormBase(string titleText, Action onSaved)
    {
        if (string.IsNullOrWhiteSpace(titleText))
            throw new ArgumentException("titleText is required.", nameof(titleText));

        _onSaved = onSaved ?? throw new ArgumentNullException(nameof(onSaved));

        Text = titleText;
        FormBorderStyle = FormBorderStyle.None;

        RootPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12)
        };
        Controls.Add(RootPanel);

        TitleLabel = new Label
        {
            Text = titleText,
            AutoSize = true,
            Left = 12,
            Top = 12,
            Parent = RootPanel
        };

        SaveButton = new Button
        {
            Text = "Save",
            Width = 100,
            Height = 30,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
            Parent = RootPanel
        };
        SaveButton.Click += (_, _) => OnSaveRequested();

        RootPanel.Resize += (_, _) => LayoutBaseControls();
        LayoutBaseControls();
    }

    protected int TitleBottom => TitleLabel.Bottom;

    protected virtual void LayoutBaseControls()
    {
        SaveButton.Left = 12;
        SaveButton.Top = RootPanel.Height - SaveButton.Height - 12;
    }

    protected void NotifySaved()
    {
        _onSaved();
    }

    protected abstract void OnSaveRequested();
}
