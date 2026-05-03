using CodexExpensa.ExtensionDevHost.Models;
using CodexExpensa.ExtensionDevHost.Services;

namespace CodexExpensa.ExtensionDevHost.UI;

public sealed class TechSpecBuilderForm : Form
{
    private readonly string _addonName;
    private readonly string _projectFolder;
    private readonly TechSpecBuilderService _service;

    private TabControl? _tabControl;
    private TechSpecConversation1Form? _conversation1Form;
    private TechSpecConversation2Form? _conversation2Form;

    private Button? _nextButton;
    private Button? _prevButton;
    private Button? _finishButton;
    private Button? _cancelButton;

    public TechSpecBuilderForm(string addonName, string projectFolder)
    {
        _addonName = addonName;
        _projectFolder = projectFolder;
        _service = new TechSpecBuilderService();
        _service.Initialize(addonName, projectFolder);

        Text = $"Build Tech Spec - {addonName}";
        Width = 900;
        Height = 700;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        InitializeUi();
    }

    private void InitializeUi()
    {
        TableLayoutPanel rootLayout = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(10)
        };

        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

        // Tab control for conversations
        _tabControl = new TabControl
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 0, 10)
        };

        _conversation1Form = new TechSpecConversation1Form(_service);
        TabPage conversation1Tab = new("Conversation 1: Vision & Requirements")
        {
            Controls = { _conversation1Form }
        };
        _conversation1Form.Dock = DockStyle.Fill;

        _conversation2Form = new TechSpecConversation2Form(_service);
        TabPage conversation2Tab = new("Conversation 2: Implementation")
        {
            Controls = { _conversation2Form }
        };
        _conversation2Form.Dock = DockStyle.Fill;

        _tabControl.TabPages.Add(conversation1Tab);
        _tabControl.TabPages.Add(conversation2Tab);

        rootLayout.Controls.Add(_tabControl, 0, 0);

        // Button panel
        FlowLayoutPanel buttonPanel = new()
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true
        };

        _finishButton = new Button
        {
            Text = "Finish & Save",
            Width = 100,
            Height = 35,
            Enabled = false
        };
        _finishButton.Click += FinishButton_Click;

        _nextButton = new Button
        {
            Text = "Next →",
            Width = 80,
            Height = 35
        };
        _nextButton.Click += NextButton_Click;

        _prevButton = new Button
        {
            Text = "← Previous",
            Width = 80,
            Height = 35,
            Enabled = false
        };
        _prevButton.Click += PrevButton_Click;

        _cancelButton = new Button
        {
            Text = "Cancel",
            Width = 80,
            Height = 35
        };
        _cancelButton.Click += (_, _) => Close();

        buttonPanel.Controls.Add(_finishButton);
        buttonPanel.Controls.Add(_nextButton);
        buttonPanel.Controls.Add(_prevButton);
        buttonPanel.Controls.Add(_cancelButton);

        rootLayout.Controls.Add(buttonPanel, 0, 1);

        Controls.Add(rootLayout);

        _tabControl.SelectedIndexChanged += (_, _) => UpdateButtonStates();
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        if (_tabControl == null || _nextButton == null || _prevButton == null || _finishButton == null)
            return;

        int currentTab = _tabControl.SelectedIndex;
        _prevButton.Enabled = currentTab > 0;
        _nextButton.Enabled = currentTab < _tabControl.TabPages.Count - 1;
        _finishButton.Enabled = currentTab == _tabControl.TabPages.Count - 1;
    }

    private void NextButton_Click(object? sender, EventArgs e)
    {
        if (_tabControl == null)
            return;

        if (_tabControl.SelectedIndex < _tabControl.TabPages.Count - 1)
        {
            // Validate Conversation 1 before moving to Conversation 2
            if (_tabControl.SelectedIndex == 0 && _conversation1Form != null)
            {
                if (!_conversation1Form.ValidateAndSave())
                {
                    MessageBox.Show(this, "Please complete Conversation 1 first.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            _tabControl.SelectedIndex++;
            UpdateButtonStates();
        }
    }

    private void PrevButton_Click(object? sender, EventArgs e)
    {
        if (_tabControl == null)
            return;

        if (_tabControl.SelectedIndex > 0)
        {
            _tabControl.SelectedIndex--;
            UpdateButtonStates();
        }
    }

    private void FinishButton_Click(object? sender, EventArgs e)
    {
        if (_conversation2Form == null || !_conversation2Form.ValidateAndSave())
        {
            MessageBox.Show(this, "Please complete Conversation 2 first.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        try
        {
            _service.SaveTechSpec();
            MessageBox.Show(this, $"Tech Spec saved successfully to {_projectFolder}\\Docs\\TechSpec.md", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Error saving tech spec: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
