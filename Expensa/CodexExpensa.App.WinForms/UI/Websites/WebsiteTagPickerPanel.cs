namespace CodexExpensa.App.WinForms.UI.Websites;

public sealed class WebsiteTagPickerPanel : UserControl
{
    private readonly TextBox _tagTextBox = new();
    private readonly ListBox _tagListBox = new();
    private List<string> _allTags = [];

    public WebsiteTagPickerPanel()
    {
        SuspendLayout();

        _tagTextBox.Dock = DockStyle.Top;
        _tagTextBox.PlaceholderText = "Type tag...";
        _tagTextBox.TextChanged += (_, _) => RefreshTagList();
        _tagTextBox.KeyDown += TagTextBox_KeyDown;

        _tagListBox.Dock = DockStyle.Fill;
        _tagListBox.IntegralHeight = false;
        _tagListBox.DoubleClick += (_, _) => AcceptListedTag();
        _tagListBox.KeyDown += TagListBox_KeyDown;

        Controls.Add(_tagListBox);
        Controls.Add(_tagTextBox);

        MinimumSize = new Size(260, 300);
        Size = new Size(280, 420);

        ResumeLayout(false);
        PerformLayout();
    }

    public event EventHandler<string>? TypedTagAccepted;
    public event EventHandler<string>? ListedTagAccepted;

    public void SetTags(IEnumerable<string> tags)
    {
        ArgumentNullException.ThrowIfNull(tags);

        _allTags = tags
            .Where(static tag => !string.IsNullOrWhiteSpace(tag))
            .Select(static tag => tag.Trim())
            .Distinct(StringComparer.CurrentCultureIgnoreCase)
            .OrderBy(static tag => tag, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        RefreshTagList();
    }

    public void FocusTextBox()
    {
        _tagTextBox.Focus();
        _tagTextBox.SelectAll();
    }

    private void TagTextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Down && _tagListBox.Items.Count > 0)
        {
            _tagListBox.Focus();
            _tagListBox.SelectedIndex = 0;
            e.Handled = true;
        }

        if (e.KeyCode == Keys.Enter)
        {
            AcceptTypedTag();
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
    }

    private void TagListBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            AcceptListedTag();
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
    }

    private void RefreshTagList()
    {
        string typedText = _tagTextBox.Text.Trim();

        IEnumerable<string> filteredTags = string.IsNullOrWhiteSpace(typedText)
            ? _allTags
            : _allTags.Where(tag => tag.Contains(typedText, StringComparison.CurrentCultureIgnoreCase));

        _tagListBox.BeginUpdate();

        try
        {
            _tagListBox.Items.Clear();

            foreach (string tag in filteredTags)
            {
                _tagListBox.Items.Add(tag);
            }

            if (_tagListBox.Items.Count > 0)
            {
                _tagListBox.SelectedIndex = 0;
            }
        }
        finally
        {
            _tagListBox.EndUpdate();
        }
    }

    private void AcceptTypedTag()
    {
        string typedTag = _tagTextBox.Text.Trim();

        if (!string.IsNullOrWhiteSpace(typedTag))
        {
            TypedTagAccepted?.Invoke(this, typedTag);
        }
    }

    private void AcceptListedTag()
    {
        string selectedTag = Convert.ToString(_tagListBox.SelectedItem) ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(selectedTag))
        {
            ListedTagAccepted?.Invoke(this, selectedTag);
        }
    }
}
