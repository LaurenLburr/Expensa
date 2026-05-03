# DocsEditorForm Splitter, Wait Icon, and Highlight Persistence

## Changes

- Saves the source preview/editor splitter distance to:

```text
%AppData%\Expensa\Extensions\DocsEditorSettings.json
```

- Restores the splitter distance when `Show Source` is enabled.
- Shows a wait cursor and hourglass-style toolbar icon while waiting for AI Review.
- Keeps AI change highlighting after AI Review until the toolbar Save button is clicked.
- If AI Review asks to save and the user clicks Yes, the file is saved, but yellow highlighting remains visible until toolbar Save is clicked.
- Toolbar Save clears the yellow AI-change highlights.
