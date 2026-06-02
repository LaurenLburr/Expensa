namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class WebsitesTreeTagPickerPanel : UserControl
{
    private readonly TextBox tagTextBox;
    private readonly ListBox tagListBox;
    private List<string> allTags = [];

    public WebsitesTreeTagPickerPanel()
    {
        tagTextBox = new TextBox();
        tagListBox = new ListBox();

        SuspendLayout();

        tagTextBox.Dock = DockStyle.Top;
        tagTextBox.PlaceholderText = "Type tag...";
        tagTextBox.TextChanged += tagTextBox_TextChanged;
        tagTextBox.KeyDown += tagTextBox_KeyDown;

        tagListBox.Dock = DockStyle.Fill;
        tagListBox.IntegralHeight = false;
        tagListBox.DoubleClick += tagListBox_DoubleClick;
        tagListBox.KeyDown += tagListBox_KeyDown;

        Controls.Add(tagListBox);
        Controls.Add(tagTextBox);

        MinimumSize = new Size(260, 300);
        Size = new Size(280, 600);

        ResumeLayout(false);
        PerformLayout();
    }

    public event EventHandler<string>? TypedTagAccepted;
    public event EventHandler<string>? ListedTagAccepted;

    public void SetTags(IEnumerable<string> tags)
    {
        ArgumentNullException.ThrowIfNull(tags);

        allTags = tags
            .Where(static tag => !string.IsNullOrWhiteSpace(tag))
            .Select(static tag => tag.Trim())
            .Distinct(StringComparer.CurrentCultureIgnoreCase)
            .OrderBy(static tag => tag, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        RefreshTagList();
    }

    public void FocusTextBox()
    {
        tagTextBox.Focus();
        tagTextBox.SelectAll();
    }

    private void tagTextBox_TextChanged(object? sender, EventArgs e)
    {
        RefreshTagList();
    }

    private void tagTextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Down && tagListBox.Items.Count > 0)
        {
            tagListBox.Focus();
            tagListBox.SelectedIndex = 0;
            e.Handled = true;
        }

        if (e.KeyCode == Keys.Enter)
        {
            AcceptTypedTag();
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
    }

    private void tagListBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            AcceptListedTag();
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
    }

    private void tagListBox_DoubleClick(object? sender, EventArgs e)
    {
        AcceptListedTag();
    }

    private void RefreshTagList()
    {
        string typedText = tagTextBox.Text.Trim();

        IEnumerable<string> filteredTags = string.IsNullOrWhiteSpace(typedText)
            ? allTags
            : allTags.Where(tag => tag.Contains(typedText, StringComparison.CurrentCultureIgnoreCase));

        tagListBox.BeginUpdate();

        try
        {
            tagListBox.Items.Clear();

            foreach (string tag in filteredTags)
            {
                tagListBox.Items.Add(tag);
            }

            if (tagListBox.Items.Count > 0)
            {
                tagListBox.SelectedIndex = 0;
            }
        }
        finally
        {
            tagListBox.EndUpdate();
        }
    }

    private void AcceptTypedTag()
    {
        string typedTag = tagTextBox.Text.Trim();

        if (!string.IsNullOrWhiteSpace(typedTag))
        {
            TypedTagAccepted?.Invoke(this, typedTag);
        }
    }

    private void AcceptListedTag()
    {
        string selectedTag = Convert.ToString(tagListBox.SelectedItem) ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(selectedTag))
        {
            ListedTagAccepted?.Invoke(this, selectedTag);
        }
    }
}
