# DocsEditorForm AI Review Highlight and Save Prompt

## Purpose

Update `DocsEditorForm` so AI Review:

1. Stores the original text before AI review.
2. Applies AI-reviewed text to the editor.
3. Highlights changed lines in yellow in the rendered preview.
4. Asks whether to save the reviewed changes.
5. Allows Cancel to restore the original text.

## Manual code changes

### 1. Add this field to `DocsEditorForm`

Add near the existing private fields:

```csharp
private HashSet<int> _highlightedPreviewLineIndexes = new();
```

---

### 2. Replace `ReviewCurrentDocumentAsync`

Replace your current `ReviewCurrentDocumentAsync` method with this:

```csharp
private async Task ReviewCurrentDocumentAsync()
{
    if (string.IsNullOrWhiteSpace(_currentFilePath))
    {
        MessageBox.Show(this, "Select a document first.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
    }

    if (string.IsNullOrWhiteSpace(sourceEditor.Text))
    {
        MessageBox.Show(this, "The selected document is empty.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
    }

    DialogResult confirm = MessageBox.Show(
        this,
        "AI Review will rewrite the current document for spelling, grammar, clarity, and professional formatting. It will not save automatically.\r\n\r\nContinue?",
        "AI Review",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question);

    if (confirm != DialogResult.Yes)
    {
        return;
    }

    string originalText = sourceEditor.Text;

    try
    {
        UseWaitCursor = true;
        aiReviewButton.Enabled = false;

        OpenAiDocumentReviewService reviewService = new(new OpenAiApiKeyStore());
        string reviewedText = await reviewService.ReviewMarkdownAsync(
            Path.GetFileName(_currentFilePath),
            sourceEditor.Text);

        string normalizedReviewedText = NormalizeMarkdownLineEndings(reviewedText);

        _highlightedPreviewLineIndexes = FindChangedLineIndexes(originalText, normalizedReviewedText);
        sourceEditor.Text = normalizedReviewedText;
        showSourceButton.Checked = false;
        RenderPreview();

        DialogResult saveResult = MessageBox.Show(
            this,
            "AI review complete. Changed lines are highlighted in yellow in the preview.\r\n\r\nSave AI-reviewed changes to disk?\r\n\r\nYes = Save changes\r\nNo = Keep changes in editor only\r\nCancel = Revert to the original text",
            "Save AI Review?",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Question);

        switch (saveResult)
        {
            case DialogResult.Yes:
                File.WriteAllText(_currentFilePath, sourceEditor.Text);
                _highlightedPreviewLineIndexes.Clear();
                RenderPreview();

                MessageBox.Show(
                    this,
                    "AI-reviewed document saved.",
                    "AI Review",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                break;

            case DialogResult.No:
                MessageBox.Show(
                    this,
                    "AI-reviewed changes are still in the editor but have not been saved.",
                    "AI Review",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                break;

            case DialogResult.Cancel:
                sourceEditor.Text = originalText;
                _highlightedPreviewLineIndexes.Clear();
                RenderPreview();

                MessageBox.Show(
                    this,
                    "AI-reviewed changes were discarded.",
                    "AI Review",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                break;
        }
    }
    catch (Exception ex)
    {
        sourceEditor.Text = originalText;
        _highlightedPreviewLineIndexes.Clear();
        RenderPreview();

        MessageBox.Show(this, ex.Message, "AI Review Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    finally
    {
        aiReviewButton.Enabled = true;
        UseWaitCursor = false;
    }
}
```

---

### 3. Add changed-line helpers

Add these methods anywhere inside `DocsEditorForm`:

```csharp
private static HashSet<int> FindChangedLineIndexes(string originalText, string revisedText)
{
    string[] originalLines = SplitNormalizedLines(originalText);
    string[] revisedLines = SplitNormalizedLines(revisedText);

    HashSet<int> changedIndexes = new();

    int max = Math.Max(originalLines.Length, revisedLines.Length);
    for (int index = 0; index < max; index++)
    {
        string original = index < originalLines.Length ? originalLines[index] : string.Empty;
        string revised = index < revisedLines.Length ? revisedLines[index] : string.Empty;

        if (!string.Equals(original, revised, StringComparison.Ordinal))
        {
            if (index < revisedLines.Length && !string.IsNullOrWhiteSpace(revised))
            {
                changedIndexes.Add(index);
            }
        }
    }

    return changedIndexes;
}

private static string[] SplitNormalizedLines(string text)
{
    return NormalizeMarkdownLineEndings(text)
        .Split(new[] { Environment.NewLine }, StringSplitOptions.None);
}
```

---

### 4. Update `SaveCurrentDocument`

Inside `SaveCurrentDocument`, after writing the file, add:

```csharp
_highlightedPreviewLineIndexes.Clear();
RenderPreview();
```

So the method should end like this:

```csharp
File.WriteAllText(_currentFilePath, sourceEditor.Text);

_highlightedPreviewLineIndexes.Clear();
RenderPreview();

MessageBox.Show(this, "Document saved.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
```

---

### 5. Update `ReloadCurrentDocument`

After loading text from disk, add:

```csharp
_highlightedPreviewLineIndexes.Clear();
RenderPreview();
```

---

### 6. Change `RenderPreview`

Replace:

```csharp
previewBrowser.DocumentText = BuildHtmlDocument(sourceEditor.Text);
```

with:

```csharp
previewBrowser.DocumentText = BuildHtmlDocument(sourceEditor.Text, _highlightedPreviewLineIndexes);
```

---

### 7. Change `BuildHtmlDocument`

Change the method signature from:

```csharp
private static string BuildHtmlDocument(string markdown)
```

to:

```csharp
private static string BuildHtmlDocument(string markdown, IReadOnlySet<int> highlightedLineIndexes)
```

Then change:

```csharp
string body = MarkdownToHtml(markdown);
```

to:

```csharp
string body = MarkdownToHtml(markdown, highlightedLineIndexes);
```

Inside the CSS block, add:

```css
.ai-change { background: #fff3a3; border-left: 4px solid #e0b400; padding-left: 8px; }
.ai-change-inline { background: #fff3a3; padding: 2px 4px; }
```

---

### 8. Change `MarkdownToHtml`

Change the method signature from:

```csharp
private static string MarkdownToHtml(string markdown)
```

to:

```csharp
private static string MarkdownToHtml(string markdown, IReadOnlySet<int> highlightedLineIndexes)
```

Then change your loop from:

```csharp
foreach (string rawLine in lines)
```

to:

```csharp
for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
{
    string rawLine = lines[lineIndex];
    bool highlight = highlightedLineIndexes.Contains(lineIndex);
```

Then, wherever you emit a paragraph, list item, heading, quote, or table row, add the `ai-change` CSS class when `highlight` is true.

Minimum practical paragraph replacement:

```csharp
html.AppendLine(highlight
    ? $"<p class=\"ai-change\">{InlineMarkdownToHtml(trimmed)}</p>"
    : $"<p>{InlineMarkdownToHtml(trimmed)}</p>");
```

Minimum practical list item replacement:

```csharp
html.AppendLine(highlight
    ? $"<li class=\"ai-change\">{InlineMarkdownToHtml(trimmed[2..])}</li>"
    : $"<li>{InlineMarkdownToHtml(trimmed[2..])}</li>");
```

Minimum practical heading replacement:

```csharp
html.AppendLine(highlight
    ? $"<div class=\"ai-change\"><h1>{InlineMarkdownToHtml(trimmed[2..])}</h1></div>"
    : $"<h1>{InlineMarkdownToHtml(trimmed[2..])}</h1>");
```

## Behavior

After this change:

```text
AI Review
  ↓
AI updates editor
  ↓
Changed lines are highlighted yellow in preview
  ↓
Prompt asks whether to save
```

Save options:

```text
Yes    saves the AI-reviewed text
No     keeps AI changes in editor only
Cancel reverts to original text
```

Highlights are preview-only. They are not saved into the Markdown file.
