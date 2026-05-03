using CodexExpensa.ExtensionDevHost.Services;

namespace CodexExpensa.ExtensionDevHost.UI;

public sealed class TechSpecConversation1Form : UserControl
{
    private readonly TechSpecBuilderService _service;

    private TextBox? _visionTextBox;
    private TextBox? _conversationHistoryBox;
    private TextBox? _userInputBox;
    private Button? _analyzeButton;
    private Label? _statusLabel;

    private bool _conversation1Complete = false;

    public TechSpecConversation1Form(TechSpecBuilderService service)
    {
        _service = service;
        InitializeUi();
    }

    public bool ValidateAndSave()
    {
        if (string.IsNullOrWhiteSpace(_visionTextBox?.Text))
        {
            return false;
        }

        _service.SetVisionStatement(_visionTextBox.Text);

        if (string.IsNullOrWhiteSpace(_service.CurrentSpec.Section1_VisionAndRequirements))
        {
            return false;
        }

        return true;
    }

    private void InitializeUi()
    {
        TableLayoutPanel layout = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            Padding = new Padding(15)
        };

        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // Title
        Label titleLabel = new()
        {
            Text = "Conversation 1: Vision & Requirements",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 10)
        };
        layout.Controls.Add(titleLabel, 0, 0);

        // Vision input section
        Label visionLabel = new()
        {
            Text = "Describe your addon vision:",
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 5)
        };

        _visionTextBox = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            AcceptsReturn = true,
            AcceptsTab = true,
            ScrollBars = ScrollBars.Vertical,
            Font = new Font("Segoe UI", 10),
            Text = _service.CurrentSpec.VisionStatement,
            Margin = new Padding(0, 0, 0, 10)
        };

        layout.Controls.Add(visionLabel, 0, 1);
        layout.SetRow(_visionTextBox, 1);
        layout.SetRowSpan(_visionTextBox, 1);
        layout.Controls.Add(_visionTextBox, 0, 1);

        // Conversation history
        Label historyLabel = new()
        {
            Text = "AI Conversation:",
            AutoSize = true,
            Margin = new Padding(0, 10, 0, 5)
        };

        _conversationHistoryBox = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Font = new Font("Courier New", 9),
            Margin = new Padding(0, 0, 0, 10)
        };

        layout.Controls.Add(historyLabel, 0, 2);
        layout.SetRow(_conversationHistoryBox, 2);
        layout.SetRowSpan(_conversationHistoryBox, 1);
        layout.Controls.Add(_conversationHistoryBox, 0, 2);

        // User input for follow-up
        Label inputLabel = new()
        {
            Text = "Your response to AI questions:",
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 5)
        };

        _userInputBox = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            AcceptsReturn = true,
            ScrollBars = ScrollBars.Vertical,
            Font = new Font("Segoe UI", 10),
            Margin = new Padding(0, 0, 0, 10)
        };

        layout.Controls.Add(inputLabel, 0, 3);
        layout.SetRow(_userInputBox, 3);
        layout.SetRowSpan(_userInputBox, 1);
        layout.Controls.Add(_userInputBox, 0, 3);

        // Button and status section
        FlowLayoutPanel bottomPanel = new()
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight
        };

        _analyzeButton = new Button
        {
            Text = "Analyze Vision with AI",
            Width = 150,
            Height = 30,
            Margin = new Padding(0, 0, 10, 0)
        };
        _analyzeButton.Click += AnalyzeButton_Click;

        _statusLabel = new Label
        {
            Text = "Ready to analyze",
            AutoSize = true,
            ForeColor = Color.Blue,
            Margin = new Padding(10, 5, 0, 0)
        };

        bottomPanel.Controls.Add(_analyzeButton);
        bottomPanel.Controls.Add(_statusLabel);

        layout.Controls.Add(bottomPanel, 0, 4);

        Controls.Add(layout);
    }

    private async void AnalyzeButton_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_visionTextBox?.Text))
        {
            MessageBox.Show("Please enter your addon vision first.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_analyzeButton == null || _statusLabel == null)
            return;

        _analyzeButton.Enabled = false;
        _statusLabel.Text = "Analyzing with AI...";
        _statusLabel.ForeColor = Color.Orange;

        try
        {
            // For now, create a placeholder response
            // TODO: Integrate with ICommandAiService
            string visionText = _visionTextBox.Text;

            _service.AddConversation1Message("User", visionText);

            string aiResponse = $"""
Based on your vision, here are some clarifying questions:

1. **Primary Purpose:** 
   - What is the main problem this addon solves?
   - Who are the target users?

2. **Core Functionality:**
   - What are the 3-5 key features you want to build?
   - What makes this addon unique?

3. **Integration Points:**
   - How does this addon integrate with the main application?
   - What data does it need access to?

4. **User Experience:**
   - How should users interact with this addon?
   - What are the key workflows?

Please provide answers to these questions in the response box below.
""";

            _service.AddConversation1Message("AI", aiResponse);
            _conversationHistoryBox.AppendText($"AI: {aiResponse}\r\n\r\n");

            _statusLabel.Text = "AI questions displayed - please answer above";
            _statusLabel.ForeColor = Color.Green;
            _conversation1Complete = true;

            // Generate Section 1
            _service.SetSection1($"## Vision & Requirements\r\n\r\n{visionText}\r\n\r\n### AI Analysis\r\n\r\n{aiResponse}");
        }
        catch (Exception ex)
        {
            _statusLabel.Text = $"Error: {ex.Message}";
            _statusLabel.ForeColor = Color.Red;
        }
        finally
        {
            _analyzeButton.Enabled = true;
        }
    }
}
