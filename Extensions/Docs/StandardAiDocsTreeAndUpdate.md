# Standard AI Docs Tree and Update Panel

## Scope

Adds standard AI instruction documents into the main tree and adds a workspace panel for regenerating them from AI.

## Tree

Adds:

```text
Docs
└── AI Instructions / General Rules
    ├── General Coding Rules
    ├── General Coding Spec
    ├── AI Add-in Design Workflow
    ├── Add-in Design Spec Template
    ├── Expensa Integration Spec Template
    └── Update Standard AI Docs
```

## Update Panel

`Update Standard AI Docs` opens an embedded workspace panel that can:

- preview each standard instruction document
- edit the regeneration prompt
- update the selected document from AI
- update all standard AI documents from AI
- open the WorkspaceDocs folder

## Storage

Documents are stored in:

```text
%AppData%\Expensa\Extensions\WorkspaceDocs
```

## Files

```text
MainForm.cs
UI/StandardAiDocsUpdateForm.cs
UI/StandardAiDocsUpdateForm.Designer.cs
UI/StandardAiDocsUpdateForm.resx
```
