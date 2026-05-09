# MainForm and DocsEditor Splitter / Wait Fix

## MainForm

- Saves `mainSplitContainer.SplitterDistance` to:

```text
%AppData%\Expensa\Extensions\MainFormSettings.json
```

- Restores the splitter position when the form is shown.
- Saves when the splitter moves and when the form closes.

## DocsEditorForm

- Saves/restores the source preview splitter position.
- Shows wait cursor and hourglass toolbar icon during AI Review.
- Also shows a yellow wait banner inside the rendered preview while waiting for the AI reply.
- Keeps AI-change highlighting until the toolbar Save button is clicked.
