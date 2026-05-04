# Document Tree Selection Load Fix

Recreated zip package.

Intent of the fix:
- MainForm immediately routes selected tree documents to DocsEditorForm
- DocsEditorForm loads files synchronously
- Preview refresh occurs after layout completion
