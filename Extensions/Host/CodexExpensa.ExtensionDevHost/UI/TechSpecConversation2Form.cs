using CodexExpensa.ExtensionDevHost.Services;

namespace CodexExpensa.ExtensionDevHost.UI;

public sealed class TechSpecConversation2Form : UserControl
{
    private readonly TechSpecBuilderService _service;

    private TextBox? _implementationTextBox;
    private TextBox? _conversationHistoryBox;
    private TextBox? _userInputBox;
    private Button? _analyzeButton;
    private TextBox? _previewBox;
    private Label? _statusLabel;

    public TechSpecConversation2Form(TechSpecBuilderService service)
    {
        _service = service;
        InitializeUi();
    }

    public bool ValidateAndSave()
    {
        if (string.IsNullOrWhiteSpace(_service.CurrentSpec.Section2_ImplementationStrategy))
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
            ColumnCount = 2,
            RowCount = 5,
            Padding = new Padding(15)
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

        // Title
        Label titleLabel = new()
        {
            Text = "Conversation 2: Implementation Strategy",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 10)
        };
        layout.Controls.Add(titleLabel, 0, 0);
        layout.SetColumnSpan(titleLabel, 2);

        // Left column: Implementation input and conversation
        Label implementationLabel = new()
        {
            Text = "Implementation approach:",
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 5)
        };

        _implementationTextBox = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            AcceptsReturn = true,
            AcceptsTab = true,
            ScrollBars = ScrollBars.Vertical,
            Font = new Font("Segoe UI", 10),
            Margin = new Padding(5, 0, 5, 10)
        };

        layout.Controls.Add(implementationLabel, 0, 1);
        layout.SetRow(_implementationTextBox, 1);
        layout.SetRowSpan(_implementationTextBox, 1);
        layout.Controls.Add(_implementationTextBox, 0, 1);

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
            Margin = new Padding(5, 0, 5, 10)
        };

        layout.Controls.Add(historyLabel, 0, 2);
        layout.SetRow(_conversationHistoryBox, 2);
        layout.SetRowSpan(_conversationHistoryBox, 1);
        layout.Controls.Add(_conversationHistoryBox, 0, 2);

        Label inputLabel = new()
        {
            Text = "Your response:",
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
            Margin = new Padding(5, 0, 5, 10)
        };

        layout.Controls.Add(inputLabel, 0, 3);
        layout.SetRow(_userInputBox, 3);
        layout.SetRowSpan(_userInputBox, 1);
        layout.Controls.Add(_userInputBox, 0, 3);

        // Right column: Vision reference and tech spec preview
        Label visionRefLabel = new()
        {
            Text = "Section 1 Reference (Vision & Requirements):",
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 5)
        };

        TextBox visionRefBox = new()
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Font = new Font("Courier New", 8),
            Text = _service.CurrentSpec.Section1_VisionAndRequirements,
            Margin = new Padding(5, 0, 5, 10)
        };

        layout.Controls.Add(visionRefLabel, 1, 1);
        layout.SetRow(visionRefBox, 1);
        layout.SetRowSpan(visionRefBox, 1);
        layout.Controls.Add(visionRefBox, 1, 1);

        Label previewLabel = new()
        {
            Text = "Tech Spec Preview:",
            AutoSize = true,
            Margin = new Padding(0, 10, 0, 5)
        };

        _previewBox = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Both,
            Font = new Font("Courier New", 8),
            Margin = new Padding(5, 0, 5, 10)
        };

        layout.Controls.Add(previewLabel, 1, 2);
        layout.SetRow(_previewBox, 2);
        layout.SetRowSpan(_previewBox, 3);
        layout.Controls.Add(_previewBox, 1, 2);

        // Buttons row
        FlowLayoutPanel buttonPanel = new()
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight
        };

        _analyzeButton = new Button
        {
            Text = "Generate Implementation Section",
            Width = 200,
            Height = 30,
            Margin = new Padding(0, 0, 10, 0)
        };
        _analyzeButton.Click += AnalyzeButton_Click;

        _statusLabel = new Label
        {
            Text = "Ready to generate",
            AutoSize = true,
            ForeColor = Color.Blue,
            Margin = new Padding(10, 5, 0, 0)
        };

        buttonPanel.Controls.Add(_analyzeButton);
        buttonPanel.Controls.Add(_statusLabel);

        layout.Controls.Add(buttonPanel, 0, 4);
        layout.SetColumnSpan(buttonPanel, 2);

        Controls.Add(layout);
    }

    private async void AnalyzeButton_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_implementationTextBox?.Text))
        {
            MessageBox.Show("Please enter your implementation approach first.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_analyzeButton == null || _statusLabel == null)
            return;

        _analyzeButton.Enabled = false;
        _statusLabel.Text = "Generating implementation section...";
        _statusLabel.ForeColor = Color.Orange;

        try
        {
            string implementationText = _implementationTextBox.Text;

            _service.AddConversation2Message("User", implementationText);

            string aiResponse = $"""
Based on your implementation approach, here are some follow-up questions:

1. **Architecture & Design:**
   - What design patterns will you use?
   - What's the overall system architecture?

2. **Data Persistence:**
   - How will data be stored and retrieved?
   - What database schema is needed?

3. **Dependencies & Integration:**
   - What external libraries or services are needed?
   - How does this integrate with existing systems?

4. **Performance & Scalability:**
   - What are the performance requirements?
   - How will the system scale?

5. **Testing & Quality:**
   - What testing strategy will you use?
   - How will you ensure code quality?

Please elaborate on these aspects in your response.
""";

            _service.AddConversation2Message("AI", aiResponse);
            _conversationHistoryBox.AppendText($"AI: {aiResponse}\r\n\r\n");

            // Generate Section 2
            _service.SetSection2($"## Implementation Strategy\r\n\r\n{implementationText}\r\n\r\n### AI Recommendations\r\n\r\n{aiResponse}");

            // Update preview
            _previewBox.Text = _service.GetMarkdownPreview();

            _statusLabel.Text = "Implementation section generated - review preview";
            _statusLabel.ForeColor = Color.Green;
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
